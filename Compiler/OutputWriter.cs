// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

#endregion

namespace SharpNative.Compiler
{
    public class OutputWriter : IDisposable
    {
        private readonly TextWriter _writer;
        private readonly string _path;
        public int Indent;
        private StringBuilder _builder = new StringBuilder(5000);


        public readonly List<ITypeSymbol> Imports = new List<ITypeSymbol>();

        public void AddInclude(ITypeSymbol import)
        {
            if (Imports.All(o => o != import))
                Imports.Add(import);
        }

        public OutputWriter(string typeNamespace, string typeName, bool isHeader = false)
        {
            var str = typeNamespace.RemoveFromEndOfString(".Namespace");
            var dir = Path.Combine(Program.OutDir, str.Replace(".", Path.DirectorySeparatorChar.ToString()));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
           
            _path = Path.Combine(dir, typeName + ".d");
            _writer = new StringWriter(_builder);
        }

        public void WriteLine(string s)
        {
            WriteIndent();
            _writer.WriteLine(s);
        }

        public void WriteLine(string format, params object[] args)
        {
            WriteIndent();

            var sb = new StringBuilder();
            sb.AppendFormat(format, args);
            _writer.WriteLine(sb);
        }

        public void WriteLine()
        {
            _writer.WriteLine();
        }

        public void Write(string s, params object[] objects)
        {
            if (objects != null && objects.Length > 0)
                _writer.Write(String.Format(s, objects));
            else
            {
                _writer.Write((s));
            }
        }

        public void Write(OutputWriter s)
        {
            _writer.Write(s._builder);
        }

        public void WriteLine(OutputWriter s)
        {
            WriteIndent();
            _writer.WriteLine(s._builder);
        }


        public bool IsNamespace;
        public bool FileExists;

        public void Dispose()
        {
            try
            {

            
                if (_path == null)
                    return;

                if (!IsNamespace)
                {
                    Finalize();
                }
                else
                {
                    if (!(this is TempWriter))
                    {
                        File.WriteAllText(_path, _builder.ToString());
                    }
                }
                _writer.Dispose();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Failed with exception: " + ex.Message + ex.StackTrace +
                    ((ex.InnerException != null)
                                    ? ex.InnerException.Message + ex.InnerException.StackTrace
                                    : ""));
            }
        }

        public void Finalize()
        {
//              if (!IsInterface) 
            {
                if (!(this is TempWriter))
                {
                    //Remove read only so we can write it
                    if (File.Exists(_path))
                        File.SetAttributes(_path, FileAttributes.Normal);
                }

                var finalBuilder = new StringBuilder();

                var forwardDeclarations = new List<string>();

                //                var myHeader = TypeProcessor.GetHeaderName(TypeState.Instance.Namespace + "." + TypeState.Instance.TypeName);
                //Process includes from types

                var cleanedupList = new List<ITypeSymbol>();
                foreach (var usedtype in Context.Instance.UsedTypes)
                {
                    if (!cleanedupList.Contains(usedtype.OriginalDefinition) &&
                        usedtype.TypeKind != TypeKind.TypeParameter)
                        cleanedupList.Add(usedtype.OriginalDefinition);
                }

                //                var specializations = new List<string>();
                foreach (var usedType in cleanedupList)
                {
                    if (!String.IsNullOrEmpty(usedType.Name))
                    {
                    }

                    if (usedType.IsSubclassOf(Context.Instance.Type))
                        continue;

                    if (usedType.TypeKind == TypeKind.PointerType) //No need to import pointer types
                        continue;

                    if (!Imports.Contains(usedType)) //TODO: change this when "Assemblies" are implemented
                        Imports.Add(usedType);
                }

                if (!Imports.Contains(Context.Object))
                    Imports.Add(Context.Object); //TODO: change this when "Assemblies" are implemented

                var @namespace =
                    Context.Instance.Type.ContainingNamespace.FullName().RemoveFromEndOfString(".Namespace");
                var moduleName = @namespace + "." + Context.Instance.TypeName;

                if (!FileExists && !(this is TempWriter))
                    finalBuilder.Append("module " + moduleName + ";\n\n");

                Imports.Add(Context.Instance.Type);

                if (Imports != null)
                {
                    finalBuilder.Append("\n");
                    IEnumerable<ITypeSymbol> imports = Imports;

                    var importGroups =
                        imports.GroupBy(k => k.ContainingNamespace).Where(j => j.Key != null);
                        //.Where(k => k.LastIndexOf('.') != -1)
                    List<string> currentImports = new List<string>();
                    foreach (IGrouping<INamespaceSymbol, ITypeSymbol> import in importGroups)
                    {
                        //if (import.Key.EndsWith("Namespace", StringComparison.Ordinal))
                        {
                            if (Context.Namespaces.ContainsKey(import.Key))
                            {
                                var types = ((import).Union(Context.Namespaces[import.Key])).Distinct().ToArray();
                                Context.Namespaces[import.Key] = types;
                            }
                            else
                                Context.Namespaces[import.Key] = import.Distinct().ToArray();

                            if (import.Key != Context.Instance.Type)
                            {
                                var name = import.Key.GetModuleName();

                                if (!currentImports.Contains(name))
                                {
                                    finalBuilder.Append(WriteIndentToString() + "import " + name + ";\n");
                                    currentImports.Add(name);
                                }
                            }
                        }
                    }

                    //Add usings
                    foreach (var @anamespace in Context.Instance.UsingDeclarations)
                    {
                        if (@anamespace.Alias == null) //Aliases are not imports
                        {
                            var name = @anamespace.Name.ToFullString() + ".Namespace";
                            if (!currentImports.Contains(name))
                            {
                                finalBuilder.Append("import " + name + ";\n");
                                currentImports.Add(name);
                            }
                        }
                    }

                    if(!Context.TypeImports.ContainsKey(Context.Instance.Type))
                    Context.TypeImports.Add(Context.Instance.Type,currentImports);
                    
                    
                    foreach (var type in Context.Instance.NamespaceAliases)
                    {
                        var name = type.Key.FullName(true, false);
                        if (name != type.Value)
                        {
                            finalBuilder.Append("alias " + name + " " + type.Value + ";\n");
                            // currentImports.Add(name);
                        }
                    }

                    //Clear used types first
                    //TypeProcessor.ClearUsedTypes();
                    //Add aliases
                    foreach (var type in Context.Instance.Aliases)
                    {
                        var name = type.Key.GetModuleName(false);
                        // if (!currentImports.Contains(name))
                        {
                            finalBuilder.Append("alias " + name + " " + type.Value + ";\n");
                            // currentImports.Add(name);
                        }
                    }
                }

                if (forwardDeclarations.Count > 0)
                    finalBuilder.Append(forwardDeclarations.Aggregate((a, b) => a + "\n" + b));

                finalBuilder.AppendLine();

                if (!(this is TempWriter))
                    finalBuilder.Append(_builder);
                else
                {
                    _builder = finalBuilder.Append(_builder);
                }

                if (!(this is TempWriter))
                {
                    if (!FileExists)
                        File.WriteAllText(_path, finalBuilder.ToString());
                    else
                        File.AppendAllText(_path, finalBuilder.ToString());

                    //Set read-only on generated files
                    //                        File.SetAttributes(_path, FileAttributes.ReadOnly);
                }
            }
        }

        private static string FullModuleName(ITypeSymbol module)
        {
            return module.ContainingNamespace.FullName() + "." +
            module.Name;
        }

        public void OpenBrace(bool showBrace = true)
        {
            if (showBrace)
                WriteLine("{");

            Indent++;
        }

        public void CloseBrace(bool showBrace = true)
        {
            Indent--;

            if (showBrace)
                WriteLine("}");
        }



        public void WriteIndent()
        {
            var c = Indent * 2;
            while (c > 0)
            {
                _writer.Write(' ');
                c--;
            }
            //_writer.Write (new string (' ', Indent * 2)); // Weird error on System.Globalization.CultureInfo ... hangs
        }

        public string WriteIndentToString()
        {
            var str = "";
            var c = Indent * 2;
            while (c > 0)
            {
                str+=(' ');
                c--;
            }
            //_writer.Write (new string (' ', Indent * 2)); // Weird error on System.Globalization.CultureInfo ... hangs
            return str;
        }



        //        public bool IsInterface;

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}