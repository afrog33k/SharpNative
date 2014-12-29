// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    public class Context
    {
        public INamedTypeSymbol Enumerator_T { get; set; }


        public List<string> StaticInits = new List<string>();
        public List<string> InstanceInits = new List<string>();


        public UsingDirectiveSyntax[] UsingDeclarations;

        public static Dictionary<INamespaceSymbol, ITypeSymbol[]> Namespaces = new Dictionary<INamespaceSymbol, ITypeSymbol[]>();

        [ThreadStatic] public static Context Instance;

        public List<SyntaxAndSymbol> Partials;
        public List<DelegateSyntaxAndSymbol> DelegatePartials;
        public string TypeName;

        public List<MemberDeclarationSyntax> AllMembers;
        public int InLambdaBreakable = 0;


        public class SyntaxAndSymbol
        {
            public BaseTypeDeclarationSyntax Syntax;
            public INamedTypeSymbol Symbol;
        }

        public class DelegateSyntaxAndSymbol
        {
            public DelegateDeclarationSyntax Syntax;
            public INamedTypeSymbol Symbol;
        }

        public string Namespace;

        public List<ITypeSymbol> UsedTypes = new List<ITypeSymbol>();

        public string EntryMethod;
        public List<AttributeSyntax> DllImports = new List<AttributeSyntax>();
        public INamedTypeSymbol Type { get; set; }


        public Solution Solution { get; private set; }
        public Project Project { get; private set; }
        public static Compilation Compilation { get; private set; }
        public SymbolNameMap SymbolNames { get; private set; }


        public static INamedTypeSymbol StructLayout { get; set; }

        public static INamedTypeSymbol FieldOffset { get; set; }


        public static INamedTypeSymbol DllImport { get; set; }


        public static INamedTypeSymbol IEnumeratorT { get; set; }

        public static INamedTypeSymbol IEnumerator { get; set; }

        public static INamedTypeSymbol Object { get; set; }
    public static void Update( Compilation compilation)
        {
            Instance = new Context();
            if (compilation != null)
                Instance.UpdateContext(compilation);
        }


        private void UpdateContext( Compilation compilation)
        {
           Object = compilation.FindType("System.Object");
            IEnumeratorT = compilation.FindType("System.Collections.Generic.IEnumerator`1");
            IEnumerator = compilation.FindType("System.Collections.IEnumerator");
              try
            {
                StructLayout = compilation.FindType("System.Runtime.InteropServices.StructLayoutAttribute");
            DllImport = compilation.FindType("System.Runtime.InteropServices.DllImportAttribute");
            FieldOffset = compilation.FindType("System.Runtime.InteropServices.FieldOffsetAttribute");
            }
            catch (Exception ex)
            {
                
                //These are not always included
            }
        }

        private static readonly Stack<Context> Instances = new Stack<Context>();
        internal static void Push()
        {
            Instances.Push(Instance);
        }

        internal static void Pop()
        {
            Instance = Instances.Pop();
        }
    }
}