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
        private readonly StringBuilder _builder = new StringBuilder(5000);


        public readonly List<string> Imports = new List<string>();

        public void AddInclude(string import)
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

        public void Write(string s)
        {
            _writer.Write(s);
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


        private string ConvertBasicType(string importName)
        {
            switch (importName)
            {
                case "void":
                    return "Void";

                case "bool":
                    return "Boolean";

                case "object":
                    return "NObject";

                case "long":
                    return "Int64";

                case "double":
                    return "Double";

                case "float":
                    return "Single";

                case "string":
                    return "String";

                case "int":
                    return "Int32";

                case "byte":
                    return "Byte";

                case "short":
                    return "Int16";

                case "char":
                    return "Char";

                case "System.Namespace.Array":
                    return "Array_T";
            }

            return importName;
        }

        public bool IsNamespace;

        public void Dispose()
        {
            if (_path == null)
                return;

            if (!IsNamespace)
            {
                //				if (!IsInterface) 
                {
                    //Remove read only so we can write it
                    if (File.Exists(_path))
                        File.SetAttributes(_path, FileAttributes.Normal);
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

                        var importName = usedType.GetFullNameD();

                        importName = ConvertBasicType(importName);

                        if (usedType is IArrayTypeSymbol)
                            importName = ("System.Namespace.Array_T");

                        //Remove Specializations
                        importName = Regex.Replace(importName, @" ?!\(.*?\)", string.Empty)
                            .Replace("(", "")
                            .Replace(")", "");

                        if (!Imports.Contains(importName))
                            Imports.Add(importName);
                    }

                    if (!Imports.Contains("System.Namespace"))
                        Imports.Add("System.Namespace");

                    var @namespace =
                        Context.Instance.Type.ContainingNamespace.FullName().RemoveFromEndOfString(".Namespace");
                    var fullModuleName = Context.Instance.Type.ContainingNamespace.FullName() + "." +
                                         Context.Instance.TypeName;
                    var moduleName = @namespace + "." + Context.Instance.TypeName;
                    finalBuilder.Append("module " + moduleName + ";\n\n");

                    Imports.Add(fullModuleName);

                    if (Context.Instance.Type.TypeKind == TypeKind.Struct)
                    {
                        Imports.Add(Context.Instance.Type.ContainingNamespace.FullName() + "." + "__Boxed_" +
                                    Context.Instance.TypeName); //Starting to reach the limit of mere strings
                    }

                    if (Imports != null)
                    {
                        finalBuilder.Append("\n");
                        IEnumerable<string> imports = Imports;

                        var importGroups =
                            imports.Where(k => k.LastIndexOf('.') != -1)
                                .GroupBy(j => j.EndsWith("Namespace") ? j : j.Substring(0, j.LastIndexOf('.')));

                        foreach (var import in importGroups)
                        {
                            if (import.Key.EndsWith("Namespace", StringComparison.Ordinal))
                            {
                                if (Context.Namespaces.ContainsKey(import.Key))
                                {
                                    var types = ((import).Union(Context.Namespaces[import.Key])).Distinct().ToArray();
                                    Context.Namespaces[import.Key] = types;
                                }
                                else
                                    Context.Namespaces[import.Key] = import.Distinct().ToArray();

                                if (import.Key != fullModuleName)
                                    finalBuilder.Append("import " + import.Key + ";\n");
                            }
                        }
                    }

                    if (forwardDeclarations.Count > 0)
                        finalBuilder.Append(forwardDeclarations.Aggregate((a, b) => a + "\n" + b));

                    finalBuilder.AppendLine();

                    finalBuilder.Append(_builder);

                    File.WriteAllText(_path, finalBuilder.ToString());

                    //Set read-only on generated files
                    File.SetAttributes(_path, FileAttributes.ReadOnly);
                }
            }
            else
            {
                File.WriteAllText(_path, _builder.ToString());

                //Set read-only on generated files
                File.SetAttributes(_path, FileAttributes.ReadOnly);
            }
            _writer.Dispose();
        }

        public void OpenBrace()
        {
            WriteLine("{");
            Indent++;
        }

        public void CloseBrace()
        {
            Indent--;
            WriteLine("}");
        }

        public void WriteIndent()
        {
            var c = Indent*2;
            while (c > 0)
            {
                _writer.Write(' ');
                c--;
            }
            //_writer.Write (new string (' ', Indent * 2)); // Weird error on System.Globalization.CultureInfo ... hangs
        }

        //        public bool IsInterface;

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}