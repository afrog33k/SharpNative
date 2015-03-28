// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CSharp;
using Microsoft.CSharp.RuntimeBinder;
using SharpNative.Compiler.DlangAst;
using SharpNative.Compiler.YieldAsync;

#endregion

namespace SharpNative.Compiler
{
    public static class Program
    {
        public static Compilation Compilation
        {
            get { return _compilation; }
        }

        private static readonly ConcurrentDictionary<SyntaxTree, SemanticModel> _models =
            new ConcurrentDictionary<SyntaxTree, SemanticModel>();

        private static Compilation _compilation;


        public static SemanticModel GetModel(SyntaxNode node)
        {
            var tree = node.SyntaxTree;

            SemanticModel ret;
            if (_models.TryGetValue(tree, out ret))
                return ret;

            ret = _compilation.GetSemanticModel(tree);

            _models.TryAdd(tree, ret);

            return ret;
        }

        public static ConcurrentDictionary<SyntaxNode, object> DoNotWrite =
            new ConcurrentDictionary<SyntaxNode, object>();

        public static ConcurrentDictionary<ISymbol, object> RefOutSymbols = new ConcurrentDictionary<ISymbol, object>();
        public static string OutDir;


        private static void WriteNamespaces()
        {
            //We dont need namespace aliases at this point
            Context.Instance.NamespaceAliases = new Dictionary<INamespaceSymbol, string>();

            //For now ignore system namespace to prevent conflicts, later we should just check if the module is defined in our sources
            var namespaces = Context.Namespaces.ToArray();
            foreach (var @namespace in namespaces)
            {
//                if (!@namespace.Key.StartsWith("System") && !isCorlib)

                var anyExtern = !@namespace.Key.DeclaringSyntaxReferences.Any() ||
                                @namespace.Key.Name == "System";//.All(o => o.MetadataName != ("__YieldIterator`1"));
                if (!anyExtern) // This should take care of corlib and external assemblies at once
                {
                    // TODO: this should write nses for only types defined in binary, this should be a switch, we could be compiling corlib
                    using (var writer = new OutputWriter(@namespace.Key.GetModuleName(), "Namespace") { IsNamespace = true })
                    {
                        writer.WriteLine("module " + @namespace.Key.GetModuleName(false) + ";");
                        List<string> addedTypes = new List<string>();
                        
                        StringBuilder namespaceImports = new StringBuilder();
                        foreach (var type in @namespace.Value)
                        {
                            if (type.ContainingType != null)
                                continue;

                            var properClassName = type.GetModuleName(false);

                            if (addedTypes.Contains(properClassName))
                                continue;

                           

                            var className = type.GetNameD();

                            if (properClassName.Trim() != String.Empty && className.Trim() != String.Empty)
                                writer.WriteLine("alias " + properClassName + " " + className + ";");

                            addedTypes.Add(properClassName);
                        }

                        //Reflection Info
                        writer.WriteLine("\n\nimport System.Namespace;");
                        writer.WriteLine("\n\nimport System.Reflection.Namespace;");

                        //To be on the safe side, import all namespaces except us... this allows us to create correct reflectioninfo
                        //gtest-053.cs
                        //                        foreach (var @name in namespaces.DistinctBy(o=>o.Key).Except(@namespace)) //Cant work leads to cycles
                        //                        {
                        //                            writer.WriteLine("import {0};", @name.Key.GetModuleName(false));
                        //                        }
                        writer.WriteLine("public class __ReflectionInfo");
                        writer.OpenBrace();
                        writer.WriteLine("static this()");
                        writer.OpenBrace();
                        foreach (var type in @namespace.Value)
                        {
                            if(type.TypeKind==TypeKind.Error)
                                continue;
                            
                            if (((INamedTypeSymbol) type).IsGenericType)
                            {

                                //Get its specializations
                                foreach (var specialization in GetGenericSpecializations(type).DistinctBy(k=>
                                    k))
                                {
                                    //TODO fix nested types
                                    //if (specialization.ContainingType == null)
                                    {

                                        var allNamespaces = GetAllNamespaces(specialization);
                                        foreach (var @name in allNamespaces.DistinctBy(o => o).Where(p=>p!=null).Except(@namespace.Key))
                                        {
                                            writer.WriteLine("import {0};", @name.GetModuleName(false));
                                        }
                                            var genericName = GetGenericMetadataName(specialization);
                                      
                                       
                                        WriteReflectionInfo(writer, specialization, genericName);
                                    }
                                }
                            }
                            else
                            {

                                var allNamespaces = GetAllNamespaces((INamedTypeSymbol) type);
                                foreach (var @name in allNamespaces.DistinctBy(o => o).Where(p => p != null).Except(@namespace.Key))
                                {
                                    writer.WriteLine("import {0};", @name.GetModuleName(false));
                                }
                               
                                var genericName = GetGenericMetadataName((INamedTypeSymbol) type);
                                WriteReflectionInfo(writer, type, genericName);
                            }
                        }
                        writer.CloseBrace();
                        writer.CloseBrace();

                    }
                }
            }
        }

        private static void WriteReflectionInfo(OutputWriter writer, ITypeSymbol specialization, string genericName)
        {
            if (specialization.TypeKind == TypeKind.Interface)
            {
                //for now no interfaces in reflection

                return;
            }
            writer.WriteLine("__TypeOf!(" + TypeProcessor.ConvertType(specialization, true, true, false) + "" + ")(\"" +
                             genericName
                             + "\")");

            writer.WriteLine(".__Setup(" + (specialization.BaseType!=null?("__TypeOf!(" + TypeProcessor.ConvertType(specialization.BaseType, true, true, false)+")") :"null") + "," + (specialization.Interfaces.Length>0? ("[" + specialization.Interfaces.Select(k=> "__TypeOf!("+TypeProcessor.ConvertType(k, true, true, false)+")").Aggregate((a,b)=>a + "," + b) +"]") : "null") + ")");
            bool writtenGetType = false;
            foreach (var member in specialization.GetMembers().DistinctBy(l=>l))
            {

                if (member is IFieldSymbol)
                {
                    var field = member as IFieldSymbol;
                    if (field.AssociatedSymbol == null)
                    {
                        // Ignore compiler generated backing fields ... for properties etc ...
                        writer.WriteLine(
                            !field.IsConst
                                ? ".__Field(\"{0}\", new FieldInfo__G!({1},{2})(function {2} ({1}* __{4}){{ return __{4}.{3};}},function void ({1}* __{4}, {2} __{0}){{ __{4}.{3} = __{0};}}))"
                                : ".__Field(\"{0}\", new FieldInfo__G!({1},{2})(function {2} ({1}* __{4}){{ return __{4}.{3};}},null))",
                            field.Name,
                            TypeProcessor.ConvertType(field.ContainingType),
                            TypeProcessor.ConvertType(field.Type),
                            WriteIdentifierName.TransformIdentifier(field.Name),
                            field.ContainingType.Name);
                    }
                }
                else if (member is IPropertySymbol)
                {
                    var property = member as IPropertySymbol;
                    bool isiface = false;
                    ITypeSymbol iface;
                    ISymbol[] proxies;
                    var name = MemberUtilities.GetMethodName(property, ref isiface, out iface, out proxies);
                    //TODO... set/specify interfaces
                    if (property.IsAbstract == false)
                    {
                        // Ignore compiler generated backing fields ... for properties etc ...
                        // //.__Property("Counter",new PropertyInfo__G!(Simple,int)(function int (Simple a){ return a.Counter();},function void (Simple a,int _age){ a.Counter(_age);})) // Need to explicitly set the interfaces etc ...

                        writer.WriteLine(
                                 ".__Property(\"{0}\", new PropertyInfo__G!({1},{2})(" + (property.GetMethod!=null? "function {2} ({1}* __{4}){{ return __{4}.{3}();}}" :"null") +","+ (property.SetMethod!=null?"function void ({1}* __{4}, {2} __{0}){{ __{4}.{3}(__{0});}}" :"null") +"))",
                               
                            property.Name,
                            TypeProcessor.ConvertType(property.ContainingType),
                            TypeProcessor.ConvertType(property.Type),
                            WriteIdentifierName.TransformIdentifier(property.Name),
                            property.ContainingType.Name);
                    }
                }
                   
                    //Constructors
                else if (member is IMethodSymbol)
                {
                    if(specialization.TypeKind==TypeKind.Enum)
                        continue;

                    var method = member as IMethodSymbol;
                    if (method.Name == "GetType" && method.Parameters == null)
                        writtenGetType = true;

                    bool isiface = false;
                    ITypeSymbol iface;
                    ISymbol[] proxies;
                    var name = MemberUtilities.GetMethodName(method,ref isiface,out iface, out proxies);
                    if (method.AssociatedSymbol == null && method.IsAbstract==false)
                    {

                        if (method.MethodKind == MethodKind.Constructor || method.MethodKind==MethodKind.StaticConstructor)
                            //TODO, need a fix for static constructors
                        {
                            //.__Method("TellEm", new MethodInfo__G!(Simple,void function(Simple,int))(&Simple.TellEm))
                            // Ignore compiler generated backing fields ... for properties etc ...
                            writer.WriteLine(

                                ".__Constructor(\"{0}\", new ConstructorInfo__G!({1},{2} function({4}))( ({4})=> "+ (specialization.TypeKind==TypeKind.Struct?"" :"new ") +"{1}({5})))"
                                ,
                                method.Name,
                                TypeProcessor.ConvertType(method.ContainingType),
                                TypeProcessor.ConvertType(method.ContainingType),
                                "this",
                                //WriteIdentifierName.TransformIdentifier(method.Name),
                                WriteMethod.GetParameterListAsString(method.Parameters, true, null, false),
                                WriteMethod.GetParameterListAsString(method.Parameters, false, null, false)
                                );
                        }
                        else
                        {
                            //.__Method("TellEm", new MethodInfo__G!(Simple,void function(Simple,int))(&Simple.TellEm))
                            // Ignore compiler generated backing fields ... for properties etc ...
                            writer.WriteLine(

                                ".__Method(\"{0}\", new MethodInfo__G!({1},{2} function({4}))(&{1}.{3}))"
                                ,
                                method.Name,
                                TypeProcessor.ConvertType(method.ContainingType),
                                TypeProcessor.ConvertType(method.ReturnType),
                                WriteIdentifierName.TransformIdentifier(name),
                                WriteMethod.GetParameterListAsString(method.Parameters, true, iface, false));
                        }
                    }

                }
                

            }
            if(!writtenGetType)
                writer.WriteLine(".__Method(\"GetType\", new MethodInfo__G!({0},System.Namespace.Type function())(&{0}.GetType))", TypeProcessor.ConvertType(specialization, true, true, false));
            writer.Write(";");
        }

        private static List<INamespaceSymbol> GetAllNamespaces(INamedTypeSymbol specialization)
        {
            var list = new List<INamespaceSymbol>();
            list.Add(specialization.ContainingNamespace);
            foreach (var arg in specialization.TypeArguments)
            {
             list.Add(arg.ContainingNamespace);   
            }
            return list;
        }

        public static string GetGenericMetadataName(this INamedTypeSymbol specialization)
        {
            //Todo Add support for Arrays and Inner classes
            return specialization.ContainingNamespace.FullNameWithDotCSharp() + (specialization.ContainingType!=null? specialization.ContainingType.MetadataName +"+" : "") + specialization.MetadataName +
            (specialization.TypeArguments.Any() ? ("[" +

                   
            specialization.TypeArguments.Select(u =>
                {
                    var namedTypeSymbol = u as INamedTypeSymbol;
                    return u.ContainingNamespace.FullNameWithDotCSharp() + (u.ContainingType != null ? (u.ContainingType.Name + "+") : "") + (namedTypeSymbol != null && namedTypeSymbol.IsGenericType ? (namedTypeSymbol.GetGenericMetadataName()) : (String.IsNullOrEmpty(u.MetadataName) ? u.Name : u.MetadataName));
                })
                       .Aggregate((a, b) => a + "," + b) + "]") : "");
        }

        //http://stackoverflow.com/questions/27105909/get-fully-qualified-metadata-name-in-roslyn
        public static string GetFullMetadataName(this INamespaceOrTypeSymbol symbol)
        {
            ISymbol s = symbol;
            var sb = new StringBuilder(s.MetadataName);

            var last = s;
            s = s.ContainingSymbol;
            while (!IsRootNamespace(s))
            {
                if (s is ITypeSymbol && last is ITypeSymbol)
                {
                    sb.Insert(0, '+');
                }
                else
                {
                    sb.Insert(0, '.');
                }
                sb.Insert(0, s.MetadataName);
                s = s.ContainingSymbol;
            }

            return sb.ToString();
        }

        private static bool IsRootNamespace(ISymbol s)
        {
            return s is INamespaceSymbol && ((INamespaceSymbol)s).IsGlobalNamespace;
        }

        internal static void Go(Compilation result, string outDir, IEnumerable<string> extraTranslations)
        {
            _compilation = result;
            OutDir = outDir;

            Context.Update(_compilation);
            Task.WaitAll(Task.Run(() => Build(outDir)), Task.Run(() => Generate(extraTranslations)));
        }

        public static void Go(string outDir, IEnumerable<string> extraTranslation, string exePath = "",
            IEnumerable<string> cSharp = null, string testName = "")
        {
            var compilation = CSharpCompilation.Create(testName, cSharp.Select(o => CSharpSyntaxTree.ParseText(o)),
                                  new MetadataReference[]
                {
                    AssemblyMetadata.CreateFromFile(typeof(object).Assembly.Location).GetReference(),
                    AssemblyMetadata.CreateFromFile(typeof(RuntimeBinderException).Assembly.Location).GetReference(),
                    AssemblyMetadata.CreateFromFile(typeof(DynamicAttribute).Assembly.Location).GetReference(),
                    AssemblyMetadata.CreateFromFile(typeof(Queryable).Assembly.Location).GetReference(),
                    AssemblyMetadata.CreateFromFile(typeof(DataTable).Assembly.Location).GetReference(),
                    AssemblyMetadata.CreateFromFile(typeof(XmlAttribute).Assembly.Location).GetReference(),
                    AssemblyMetadata.CreateFromFile(typeof(CSharpCodeProvider).Assembly.Location).GetReference(),
                    AssemblyMetadata.CreateFromFile(typeof(Enumerable).Assembly.Location).GetReference(),

                    AssemblyMetadata.CreateFromFile(typeof(HttpRequest).Assembly.Location).GetReference(),
                    AssemblyMetadata.CreateFromFile(typeof(CSharpCodeProvider).Assembly.Location).GetReference()
                }, new CSharpCompilationOptions(OutputKind.ConsoleApplication, allowUnsafe: true));

            _compilation = compilation;
            OutDir = outDir;

            Context.Update(compilation);
            Task.WaitAll(Task.Run(() => Build(exePath)), Task.Run(() => Generate(extraTranslation)));
        }

        private static void Build(string exePath)
        {
            exePath = exePath.Replace("\\", "/");
            if (!String.IsNullOrEmpty(exePath))
            {
                Context.Namespaces.Clear();
                if (Driver.Verbose)
                    Console.WriteLine("Building...");

                var sw = Stopwatch.StartNew();
                var fStream = File.Open(exePath, FileMode.CreateNew, FileAccess.ReadWrite);
                var buildResult = _compilation.Emit(fStream);

                fStream.Close();

                if (buildResult.Success == false)
                {
                    if (buildResult.Diagnostics.FirstOrDefault().Id != "CS5001") // No main
                    {
                        throw new Exception("Build failed. " + buildResult.Diagnostics.Count() + " errors: " +
                            string.Join("", buildResult.Diagnostics.Select(o => "\n  " + o.ToString())));
                    }
                }
                else
                {
                   //  CurrentAssembly = Assembly.LoadFile(exePath);
                    
                }
                if (Driver.Verbose)
                    Console.WriteLine("Built in " + sw.Elapsed.TotalMilliseconds + " ms");
            }
        }

        public static Assembly CurrentAssembly { get; set; }

        private static bool isCorlib = false;
        private static void Generate(IEnumerable<string> extraTranslation)
        {
            try
            {
                if (Driver.Verbose)
                    Console.WriteLine("Parsing...");
                var sw = Stopwatch.StartNew();

                if (!Directory.Exists(OutDir))
                    Directory.CreateDirectory(OutDir);

                decimal lastTemporaryIndex = 0;

                Context.LastNode = _compilation.SyntaxTrees.First().GetRoot();
                Context.Compilation = _compilation;

                //Yield Support
//               _compilation= _compilation.AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(@"
//namespace System
//{
//    public abstract class __YieldIterator<T> : System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IEnumerator<T>
//    {
//        public T Current { get; protected set; }
//
//        public abstract System.Collections.Generic.IEnumerator<T> GetEnumerator();
//        public abstract bool MoveNext();
//
//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }
//
//        public void Dispose()
//        {
//        }
//
//        object System.Collections.IEnumerator.Current
//        {
//            get { return Current; }
//        }
//
//        public void Reset()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}"));


                //Replace Object Initializers
              //  ProcessObjectInitializers(lastTemporaryIndex);

                //Change order of invocations to take care of default arguments and named parameters
                FixupInvocations();

                FullyQualifyReferences();

                ProcessYield();


                //TODO: not needed for Dlang
                //GetGenericSpecializations();
                if (Driver.Verbose)
                    Console.WriteLine("Parsed in " + sw.Elapsed.TotalMilliseconds + " ms . Writing out d files ...");
                sw.Restart();

                ProcessAnonymousTypes();

                ProcessDelegates();
                
                ProcessTypes();

                WriteNamespaces();
                if (Driver.Verbose)
                    Console.WriteLine("D code written out in " + sw.Elapsed.TotalMilliseconds + " ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with exception: " + ex.Message + ex.StackTrace +
                    ((ex.InnerException != null)
                                      ? ex.InnerException.Message + ex.InnerException.StackTrace
                                      : ""));
            }
        }

        private static void ProcessYield()
        {
            foreach (var syntaxTree in _compilation.SyntaxTrees)
            {
                var compilationUnit = ((CompilationUnitSyntax)syntaxTree.GetRoot());
                var model = _compilation.GetSemanticModel(syntaxTree);
                var rewriter = new YieldGenerator(_compilation,syntaxTree,model);

                compilationUnit = (CompilationUnitSyntax)compilationUnit.Accept(rewriter);
                _compilation = _compilation.ReplaceSyntaxTree(syntaxTree,
                    SyntaxFactory.SyntaxTree(compilationUnit, null, syntaxTree.FilePath));
            }
        }

        private static void FixupInvocations()
        {
            foreach (var syntaxTree in _compilation.SyntaxTrees)
            {
                var compilationUnit = ((CompilationUnitSyntax)syntaxTree.GetRoot());
                var model = _compilation.GetSemanticModel(syntaxTree);
                var rewriter = new InvocationRewriter(model);

                compilationUnit = (CompilationUnitSyntax)compilationUnit.Accept(rewriter);
                _compilation = _compilation.ReplaceSyntaxTree(syntaxTree,
                    SyntaxFactory.SyntaxTree(compilationUnit, null, syntaxTree.FilePath));
            }
        }

        private static void FullyQualifyReferences()
        {
            foreach (var syntaxTree in _compilation.SyntaxTrees)
            {
                var compilationUnit = ((CompilationUnitSyntax)syntaxTree.GetRoot());
                var model = _compilation.GetSemanticModel(syntaxTree);
                var rewriter = new CSharpToDlangRewriter(model);

                compilationUnit = (CompilationUnitSyntax)compilationUnit.Accept(rewriter);
                _compilation = _compilation.ReplaceSyntaxTree(syntaxTree,
                    SyntaxFactory.SyntaxTree(compilationUnit, null, syntaxTree.FilePath));
            }
        }

        private static bool RunInParallel = false;

        private static void ProcessTypes()
        {
            var allTypes = _compilation.SyntaxTrees
                .SelectMany(o => o.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
                .Select(o => new
                {
                    Syntax = o,
                    Symbol = GetModel(o).GetDeclaredSymbol(o),
                    TypeName =  WriteType.TypeName(GetModel(o).GetDeclaredSymbol(o))
                }).Where(k => k.Symbol.ContainingType == null && (k.Symbol.ContainingNamespace.FullNameWithDot() + k.TypeName)!= "System.Namespace.__YieldIterator__G") // Ignore all nested classes
                .GroupBy(o => o.Symbol.ContainingNamespace.FullNameWithDot() + o.TypeName)
                .ToList();

            if (RunInParallel)
                allTypes.Parallel(type => //.ForEach(type => //.Parallel(type =>
                    {
                        Context.Instance = new Context
                        {
                            TypeName = type.First().TypeName,
                            Partials =
                        type.Select(
                                o =>
                                new Context.SyntaxAndSymbol
                                {
                                    Symbol = o.Symbol,
                                    Syntax = o.Syntax
                                })
                            .Where(o => !DoNotWrite.ContainsKey(o.Syntax))
                            .ToList()
                        };

                        if (Context.Instance.Partials.Count > 0)
                        {
                            try
                            {
                                Context.LastNode = type.First().Syntax;
                                WriteType.Go();
                            }
                            catch (Exception ex)
                            {
                                //TODO: remove this when done with CorLib 
                                //  throw ex;

                                Console.WriteLine("An exception occurred: " + ex.Message + "\r\n" + ex.StackTrace + "\r\nwhile processing node " + Context.LastNode.ToFullString() + " in " + Context.Instance.TypeName + " from " + Context.LastNode.GetDetails());
                            }
                        }
                    });
            else
            {
                allTypes.ForEach(type => //.ForEach(type => //.Parallel(type =>
                    {
                        Context.Instance = new Context
                        {
                            TypeName = type.First().TypeName,
                            Partials =
                        type.Select(
                                o =>
                                new Context.SyntaxAndSymbol
                                {
                                    Symbol = o.Symbol,
                                    Syntax = o.Syntax
                                })
                            .Where(o => !DoNotWrite.ContainsKey(o.Syntax))
                            .ToList()
                        };

                        if (Context.Instance.Partials.Count > 0)
                        {
                            try
                            {
                                Context.LastNode = type.First().Syntax;
                                WriteType.Go();
                            }
                            catch (Exception ex)
                            {
                                //TODO: remove this when done with CorLib 
                                //  throw ex;

                                Console.WriteLine("An exception occurred: " + Context.LastNode.GetDetails() + "\r\nDetails:\r\n" +
                                    ex.Message + "\r\n" + ex.StackTrace + "\r\nwhile processing node " +
                                    Context.LastNode.ToFullString());
                                // + " in " + Context.Instance.TypeName + " from " + );
                            }
                        }
                    });
            }

            var symbols =
                allTypes.SelectMany(o => o)
                    .Where(o => !DoNotWrite.ContainsKey(o.Syntax))
                    .Select(o => o.Symbol)
                    .Select(g => g)
                    .Where(g => g != null);

            if (symbols.Any(j => j.ContainingAssembly.Name == "mscorlib"))
            {
                isCorlib = true;
            }

           
        }

        private static void ProcessDelegates()
        {
            var delegates = _compilation.SyntaxTrees
                .SelectMany(o => o.GetRoot().DescendantNodes().OfType<DelegateDeclarationSyntax>())
                .Select(o => new
                {
                    Syntax = o,
                    Symbol = GetModel(o).GetDeclaredSymbol(o),
                    TypeName = WriteType.TypeName(GetModel(o).GetDeclaredSymbol(o))
                }).Where(k => k.Symbol.ContainingType == null) // Ignore all nested delegates
                .GroupBy(o => o.Symbol.ContainingNamespace.FullNameWithDot() + o.TypeName)
                .ToList();

            if (RunInParallel)
                delegates.Parallel(type => //.ForEach(type => //.Parallel(type =>
                    {
                        Context.Instance = new Context
                        {
                            TypeName = type.First().TypeName,
                            DelegatePartials =
                        type.Select(
                                o =>
                                new Context.DelegateSyntaxAndSymbol
                                {
                                    Symbol = o.Symbol,
                                    Syntax = o.Syntax
                                })
                            .Where(o => !DoNotWrite.ContainsKey(o.Syntax))
                            .ToList()
                        };

                        if (Context.Instance.DelegatePartials.Count > 0)
                            WriteDelegate.Go();
                    });
            else
            {
                delegates.ForEach(type => //.ForEach(type => //.Parallel(type =>
                    {
                        Context.Instance = new Context
                        {
                            TypeName = type.First().TypeName,
                            DelegatePartials =
                        type.Select(
                                o =>
                                new Context.DelegateSyntaxAndSymbol
                                {
                                    Symbol = o.Symbol,
                                    Syntax = o.Syntax
                                })
                            .Where(o => !DoNotWrite.ContainsKey(o.Syntax))
                            .ToList()
                        };

                        if (Context.Instance.DelegatePartials.Count > 0)
                            WriteDelegate.Go();
                    });
            }
        }

        private static void ProcessAnonymousTypes()
        {
            _compilation.SyntaxTrees
                .SelectMany(o => o.GetRoot().DescendantNodes().OfType<AnonymousObjectCreationExpressionSyntax>())
                .Select(o => new
                {
                    Syntax = o,
                    Name = WriteAnonymousObjectCreationExpression.TypeName(o)
                })
                .GroupBy(o => o.Name)
                .Parallel(o =>
                {
                    Context.Instance = new Context
                    {
                        TypeName = o.Key,
                    };

                    WriteAnonymousObjectCreationExpression.WriteAnonymousType(o.First().Syntax);
                });
        }

        static bool IsSpecialized(INamedTypeSymbol o)
        {
            if (o == null)
                return false;

            if (o.ContainingType != null && !IsSpecialized(o.ContainingType))
                return false;

            return o.TypeArguments.All(k =>
                {
                    var asNamed = k as INamedTypeSymbol;

                    return k.TypeKind != TypeKind.TypeParameter && (asNamed != null && asNamed.TypeArguments.All(l => IsSpecialized(l as INamedTypeSymbol)));
                });
        }

        //Only needed for CPP target
        //Resurrected, useful for reflection
        private static List<INamedTypeSymbol> GetGenericSpecializations(ITypeSymbol type)
        {
            return _compilation.SyntaxTrees
                .SelectMany(o => o.GetRoot().DescendantNodes().OfType<GenericNameSyntax>().Union(o.GetRoot().DescendantNodes().OfType<TypeSyntax>()))
                .Select(
                o =>
                        (ModelExtensions.GetTypeInfo(GetModel(o), o).Type ??
                ModelExtensions.GetTypeInfo(GetModel(o), o).ConvertedType) as INamedTypeSymbol)
                .Where(o => o != null && (IsSpecialized(o) && o.OriginalDefinition == type))
                .ToList();
        }

        private static void ProcessObjectInitializers(decimal lastTemporaryIndex)
        {
            foreach (var stree in _compilation.SyntaxTrees)
            {
                SyntaxNode newRoot = stree.GetRoot();
                SyntaxNode[] allTargets = newRoot.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().ToArray();

                var targets =
                    allTargets.Where(k => !(k.Parent.Parent.IsKind(SyntaxKind.ObjectInitializerExpression)))
                        .Select(k => k as ObjectCreationExpressionSyntax);

                var objectCreationExpressionSyntaxs = targets as ObjectCreationExpressionSyntax[] ?? targets.ToArray();

                IEnumerable<KeyValuePair<SyntaxNode, SyntaxNode>> replacedNodes =
                    new Dictionary<SyntaxNode, SyntaxNode>();
                newRoot = newRoot.TrackNodes(allTargets);
                foreach (var target in objectCreationExpressionSyntaxs)
                {
                    //is root ?
                    var isroot = !(target.Ancestors().OfType<ObjectCreationExpressionSyntax>().Any());

                    //                  Console.WriteLine ("rc: " + target.Ancestors ().OfType<ObjectCreationExpressionSyntax> ().Count());

                    //                  if (isroot) // Debug
                    //                  {
                    //                      Console.WriteLine ("Found a root..." + initcounter++ + ": " + target.GetLocation().GetLineSpan());
                    //                  }

                    lastTemporaryIndex = ProcessObjectInitializer(target, lastTemporaryIndex, ref newRoot,
                        ref allTargets,
                        ref replacedNodes, 0);

                    replacedNodes = replacedNodes.OrderBy(o => o.Key.DescendantNodes().Count());

                    var newList1 = new List<ObjectCreationExpressionSyntax>();

                    var union1 = (replacedNodes.Select(k => k.Key));
                    var nodes1 = union1 as SyntaxNode[] ?? union1.ToArray();

                    foreach (var statementSyntax in nodes1)
                    {
                        try
                        {
                            newList1.Add((ObjectCreationExpressionSyntax)newRoot.GetCurrentNode(statementSyntax));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    var nodeArray = replacedNodes.ToArray();

//                    if (!isroot)
//                        Console.WriteLine("No root here ... move along ...");
//                    else
                    {
//                        Console.WriteLine("Lets do the magic...");
                        //                      for (int i = 0, nodeArrayLength = nodeArray.Length; i < nodeArrayLength; i++)
                        //                      {
                        //                          var replacedNode = nodeArray [i];
                        //                          var nodeKey = replacedNode.Key;
                        //                          try
                        //                          {
                        //                              newRoot = newRoot.TrackNodes (newList1);
                        //                              var nkey = newRoot.GetCurrentNode (nodeKey);
                        //                              var newList = new List<ObjectCreationExpressionSyntax> ();
                        //
                        //
                        //                              newRoot = newRoot.ReplaceNode (nkey, replacedNode.Value);
                        //                              var union = (nodeArray.Select (k => k.Key));
                        //                              var nodes = union as SyntaxNode[] ?? union.ToArray ();
                        //                              foreach (var statementSyntax in nodes)
                        //                              {
                        //                                  try
                        //                                  {
                        //                                      newList.Add ((ObjectCreationExpressionSyntax)newRoot.GetCurrentNode (statementSyntax));
                        //                                  }
                        //                                  catch (Exception ex)
                        //                                  {
                        //                                  }
                        //                              }
                        //                              newList1 = newList;
                        //                              newRoot = newRoot.TrackNodes (newList1);
                        //                              newRoot = newRoot.NormalizeWhitespace ();
                        //                          }
                        //                          catch (Exception ex)
                        //                          {
                        //
                        //                          }
                        //                      }
                        //Create new Block with all nodes and convert it to a lambda ...
                    }
                }

                _compilation = _compilation.ReplaceSyntaxTree(stree,
                    SyntaxFactory.ParseSyntaxTree(newRoot.ToFullString(), null,
                        String.IsNullOrEmpty(stree.FilePath) ? "/var/temp/tree.cs" : stree.FilePath));

                //                  SyntaxFactory.SyntaxTree(compilationUnit,null, syntaxTree.FilePath)
            }
        }

        private static decimal ProcessObjectInitializer(ObjectCreationExpressionSyntax target,
            decimal lastTemporaryIndex, ref SyntaxNode newRoot, ref SyntaxNode[] originalObjectExpressionSyntax,
            ref IEnumerable<KeyValuePair<SyntaxNode, SyntaxNode>> replacedNodes, int depth = 0)
        {
            depth++;
            var objectCreation = target;

            if (objectCreation.Initializer.IsKind(SyntaxKind.ObjectInitializerExpression))
            {
                var initializerInfo = GetModel(target).GetTypeInfo(target);

                var tempName = initializerInfo.Type.Name + "_helper_" + lastTemporaryIndex++;

                var assignments = new List<SyntaxNode>();

                foreach (var expression in objectCreation.Initializer.Expressions)
                {
                    var localExpression = (expression as AssignmentExpressionSyntax);
                    if (localExpression == null)
                        continue;

                    var right = localExpression.Right;

                    var targets =
                        expression.DescendantNodes()
                            .OfType<ObjectCreationExpressionSyntax>()
                            .Where(k => k.Ancestors().OfType<ObjectCreationExpressionSyntax>().First() == target);

                    var objectCreationExpressionSyntaxs = targets as ObjectCreationExpressionSyntax[] ??
                                                          targets.ToArray();

                    var noriginalObjectExpressionSyntax =
                        objectCreationExpressionSyntaxs.ToArray<SyntaxNode>().ToArray();

                    foreach (var ntarget in noriginalObjectExpressionSyntax)
                    {
                        lastTemporaryIndex = ProcessObjectInitializer((ObjectCreationExpressionSyntax)ntarget,
                            lastTemporaryIndex, ref newRoot,
                            ref originalObjectExpressionSyntax, ref replacedNodes, depth);
                    }

                    if (noriginalObjectExpressionSyntax.Any())
                    {
                        bool contained;
                        try
                        {
                            newRoot.GetCurrentNode(right);
                            contained = true;
                        }
                        catch (Exception)
                        {
                            contained = false;
                        }

                        if (contained)
                        {
                            if (right is MemberAccessExpressionSyntax) // Since Roslyn 1.0.0, old method doesnt work ...
                            {
                                var mar = right as MemberAccessExpressionSyntax;

                                contained = !(mar.Expression.ToString() == replacedNodes.Last().Key.ToString());
                            }
                            else if (right is ObjectCreationExpressionSyntax)
                            {
                                var mar = right as ObjectCreationExpressionSyntax;

                                contained =
                                    !mar.Initializer.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any();
                                //!(mar.ToString () == replacedNodes.Last ().Key.ToString ());
                            }
                        }

                        if (!contained || replacedNodes.Count() == 1) // We been replaced 
                        {
                            try
                            {
                                var str = replacedNodes.Last().Value.ToFullString();
                                if (right is MemberAccessExpressionSyntax)
                                {
                                    var mar = right as MemberAccessExpressionSyntax;
                                    right = SyntaxFactory.ParseExpression(str + "." + mar.Name.ToFullString());
                                }
                                else
                                {
                                    // for

                                    // replacedNodes.Any(k => k.Key == right);
                                    right = SyntaxFactory.ParseExpression(str);
                                    // (ExpressionSyntax)replacedNodes.FirstOrDefault(o => o.Key.ToFullString() == right.ToFullString()).Value;
                                }

                                replacedNodes = replacedNodes.Except(replacedNodes.Last());
                            }
                            catch (Exception)
                            {
                                //      throw;
                            }
                        }
                    }

                    var newAssignment =
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(
                                        tempName), SyntaxFactory.IdentifierName(
                                        localExpression.Left.As<IdentifierNameSyntax>()
                                                .ToFullString())), right)
                        ).NormalizeWhitespace();

                    assignments.Add(newAssignment);
                    //  var varName = declarator.Identifier.Text + "_helper_"+ e + lastTemporaryIndex++;
                }

                var statements = new List<StatementSyntax>();

                ObjectCreationExpressionSyntax newObjectCreation;
                if (objectCreation.ArgumentList == null ||
                    (!objectCreation.ArgumentList.Arguments.Any()))
                {
                    var text = objectCreation.WithInitializer(null).ToFullString().Trim();
                    if (!text.EndsWith("()"))
                        text += "()";

                    var exp = SyntaxFactory.ParseExpression(text);
                    newObjectCreation = (ObjectCreationExpressionSyntax)exp;
                    //SyntaxFactory.ObjectCreationExpression(objectCreation.Type);
                }
                else
                {
                    newObjectCreation =
                        objectCreation.WithInitializer(null).WithArgumentList(objectCreation.ArgumentList);
                }

                var local = SyntaxFactory.LocalDeclarationStatement(
                                SyntaxFactory.VariableDeclaration(
                                    target.Type,
                                    SyntaxFactory.SeparatedList(new[]
                            {
                                SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier(tempName),
                                    null,
                                    SyntaxFactory.EqualsValueClause(newObjectCreation))
                            })
                                )
                            ).NormalizeWhitespace();

                statements.Add(local);
                statements.AddRange(assignments.Cast<StatementSyntax>());

                var listS = SyntaxFactory.List(statements);

                var newList2 = new List<ObjectCreationExpressionSyntax>();
                var union2 = originalObjectExpressionSyntax.Union(new SyntaxNode[] { target });
                foreach (var statementSyntax in union2)
                {
                    try
                    {
                        newList2.Add((ObjectCreationExpressionSyntax)newRoot.GetCurrentNode(statementSyntax));
                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (!newList2.Contains(target))
                {
                    //                 
                    originalObjectExpressionSyntax =
                        originalObjectExpressionSyntax.Union(new SyntaxNode[] { target }).ToArray();
                }

                originalObjectExpressionSyntax = originalObjectExpressionSyntax.Where(l => l != null).ToArray();
                newRoot = newRoot.TrackNodes(originalObjectExpressionSyntax);
                try
                {
                    var currentNode = newRoot.GetCurrentNode(target);

                    var lastStatement = currentNode.Ancestors().OfType<StatementSyntax>().First();

                    newRoot = newRoot.InsertNodesBefore(lastStatement, listS);

                    //  var currentNodes = newRoot.GetCurrentNodes<SyntaxNode>(originalObjectExpressionSyntax);
                    newRoot = newRoot.TrackNodes(originalObjectExpressionSyntax);

                    currentNode = newRoot.GetCurrentNode(target);
                    var newName = SyntaxFactory.ParseExpression(tempName);

                    var nDict = new Dictionary<SyntaxNode, SyntaxNode>();
                    foreach (var replacedNode in replacedNodes)
                        nDict[replacedNode.Key] = replacedNode.Value;

                    nDict[currentNode] = newName;
                    replacedNodes = nDict;
                    // newRoot = newRoot;
                    newRoot = newRoot.ReplaceNode(currentNode, newName);

                    var newList = new List<ObjectCreationExpressionSyntax>();
                    var union = originalObjectExpressionSyntax.Union(nDict.Values); //.Union(new SyntaxNode[]{newName});
                    foreach (var statementSyntax in union)
                    {
                        try
                        {
                            //                          if(statementSyntax==newName)
                            //                          {
                            //                              newList.Add((ObjectCreationExpressionSyntax) (newName));
                            //                              continue;
                            //                          }

                            var syntaxNode = newRoot.GetCurrentNode(statementSyntax);
                            newList.Add((ObjectCreationExpressionSyntax)syntaxNode);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    originalObjectExpressionSyntax = newList.ToArray();
                    newRoot = newRoot.TrackNodes(originalObjectExpressionSyntax);
                }
                catch (Exception ex)
                {
                }
            }

            return lastTemporaryIndex;
        }

        /// <summary>
        ///     Get all the type declarations in a compilation
        /// </summary>
        private static IEnumerable<BaseTypeDeclarationSyntax> GetTypeDeclarations(CompilationUnitSyntax compilationUnit)
        {
            foreach (var member in compilationUnit.Members)
            {
                if (member is BaseTypeDeclarationSyntax)
                    yield return (BaseTypeDeclarationSyntax)member;
                else if (member is NamespaceDeclarationSyntax)
                {
                    foreach (var item in GetTypeDeclarations((NamespaceDeclarationSyntax) member))
                        yield return item;
                }
            }
        }

        /// <summary>
        ///     Get all the type declarations in a given namespace
        /// </summary>
        private static IEnumerable<BaseTypeDeclarationSyntax> GetTypeDeclarations(NamespaceDeclarationSyntax ns)
        {
            foreach (var member in ns.Members)
            {
                if (member is BaseTypeDeclarationSyntax)
                    yield return (BaseTypeDeclarationSyntax)member;
                else if (member is NamespaceDeclarationSyntax)
                {
                    foreach (var item in GetTypeDeclarations((NamespaceDeclarationSyntax) member))
                        yield return item;
                }
            }
        }

        /// <summary>
        ///     Get all the delegates in a given type member.
        /// </summary>
        private static IEnumerable<DelegateDeclarationSyntax> GetDelegates(MemberDeclarationSyntax member)
        {
            if (member is ClassDeclarationSyntax)
            {
                foreach (var item in GetDelegates((ClassDeclarationSyntax) member))
                    yield return item;
            }
            else if (member is NamespaceDeclarationSyntax)
            {
                foreach (var item in GetDelegates((NamespaceDeclarationSyntax) member))
                    yield return item;
            }
            else if (member is DelegateDeclarationSyntax)
                yield return (DelegateDeclarationSyntax)member;
        }

        private static IEnumerable<DelegateDeclarationSyntax> GetDelegates(CompilationUnitSyntax compilationUnit)
        {
            foreach (var member in compilationUnit.Members)
            {
                foreach (var item in GetDelegates(member))
                    yield return item;
            }
        }

        private static IEnumerable<DelegateDeclarationSyntax> GetDelegates(ClassDeclarationSyntax type)
        {
            foreach (var member in type.Members)
            {
                foreach (var item in GetDelegates(member))
                    yield return item;
            }
        }

        private static IEnumerable<DelegateDeclarationSyntax> GetDelegates(NamespaceDeclarationSyntax ns)
        {
            foreach (var member in ns.Members)
            {
                foreach (var item in GetDelegates(member))
                    yield return item;
            }
        }
    }
}