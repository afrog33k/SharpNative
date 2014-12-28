// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteType
    {
        static Dictionary<string,string> TypeRenames = new Dictionary<string, string>()
        {
            { "System.Namespace.Object","NObject"},
            { "System.Namespace.Exception","NException"}
        }; 

        public static void Go()
        {
            var partials = Context.Instance.Partials;
            var first = partials.First();
            Context.Instance.Namespace = first.Symbol.ContainingNamespace.FullName();
            Context.Instance.Type = first.Symbol;
            TypeProcessor.ClearUsedTypes();
            var mynamespace = Context.Instance.Type.ContainingNamespace.FullName().RemoveFromEndOfString(".Namespace");

            var fullname = Context.Instance.Namespace + "." + Context.Instance.TypeName;
            if (TypeRenames.ContainsKey(fullname))
            {
                Context.Instance.TypeName = TypeRenames[fullname];
            }

            // + "." + TypeState.Instance.TypeName;
            Console.WriteLine("Writing out type: " + fullname);
            if (fullname ==
                "System.Namespace.Nullable")
            {
            }

            var myUsingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(mynamespace));
            var SystemUsingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
                // Required as certain functions like boxing are in this namespace
            var namespaces = first.Syntax.Parent.DescendantNodes().OfType<UsingDirectiveSyntax>().ToArray();
            var usingStatements = first.Syntax.Parent.DescendantNodes().OfType<UsingStatementSyntax>().ToArray();
            Context.Instance.UsingDeclarations = namespaces
                .Union(new[]
                {
                    myUsingDirective, SystemUsingDirective
                }).ToArray();

           

            using (var writer = new OutputWriter(Context.Instance.Namespace, Context.Instance.TypeName))
            {
                var bases = partials
                    .Select(o => o.Syntax.BaseList)
                    .Where(o => o != null)
                    .SelectMany(o => o.Types)
                    .Select(o => TypeProcessor.GetTypeInfo(o.Type).ConvertedType)
                    .Distinct()
                    .ToList();

                //                var interfaces = bases.Where(o => o.TypeKind == TypeKind.Interface).ToList();

                    if (  Context.Instance.Type !=Context.Object)
                if (!bases.Any((j => j.TypeKind != TypeKind.Interface)) &&
                    !(first.Symbol.TypeKind == TypeKind.Interface || first.Symbol.TypeKind == TypeKind.Struct))
                    //TODO: fix structs using mixins / alias this
                    bases.Add(Context.Object);

                //                    WriteStandardIncludes.Go(writer);

                //                    writer.WriteLine(String.Format("#include \"{0}\"", TypeState.Instance.TypeName + ".h"));

                WriteBcl.Go(writer);

                //TypeState.Instance.DerivesFromObject = bases.Count == interfaces.Count;

                var @namespace = first.Symbol.ContainingNamespace.FullName();
                var genericArgs = Context.Instance.Type.TypeParameters.ToList();


                //Module name = namespace + "." + typename;

                WriteStandardIncludes.Go(writer);

                //                    var namespaces = @namespace.Split(new string[] { "." }, StringSplitOptions.None);
                //
                //                    if (@namespace.Length > 0)
                //                    {
                //                        foreach (var ns in namespaces)
                //                        {
                //                            writer.WriteLine("namespace " + ns + "\r\n{");
                //                            writer.WriteLine("namespace " + ns + "\r\n{");
                //                        }
                //
                //                    }

                //TODO: Fix enum support
                if (first.Syntax is EnumDeclarationSyntax)
                {
                    WriteEnum.Go(writer,
                        Context.Instance.Partials.Select(o => o.Syntax)
                            .Cast<EnumDeclarationSyntax>()
                            .SelectMany(o => o.Members)
                            .Where(o => !Program.DoNotWrite.ContainsKey(o)));

                    return;
                }

                Context.Instance.AllMembers =
                    partials.Select(o => o.Syntax)
                        .Cast<TypeDeclarationSyntax>()
                        .SelectMany(o => o.Members)
                        .Where(o => !Program.DoNotWrite.ContainsKey(o))
                        .ToList();

                var allMembersToWrite = Context.Instance.AllMembers
                    .Where(member => !(member is TypeDeclarationSyntax)
                                     && !(member is EnumDeclarationSyntax)
                                     && !(member is DelegateDeclarationSyntax) &&
                                     !(member is ConstructorDeclarationSyntax))
                    .ToList();

                var instanceCtors = Context.Instance.AllMembers.OfType<ConstructorDeclarationSyntax>()
                    .Where(o => !o.Modifiers.Any(SyntaxKind.StaticKeyword))
                    .ToList();

                var staticCtors = Context.Instance.AllMembers.OfType<ConstructorDeclarationSyntax>()
                    .Where(o => (o.Modifiers.Any(SyntaxKind.StaticKeyword)))
                    .ToList();

                //TODO: Add support for overloading constructing
                //                    if (instanceCtors.Count > 1)
                //                        throw new Exception(
                //                            "Overloaded constructors are not supported.  Consider changing all but one to static Create methods " +
                //                            Utility.Descriptor(first.Syntax));

                //                    var ctorOpt = instanceCtors.SingleOrDefault();
                //TODO: Handle interfaces by
                /*
                class Interface
                {
                    public:
                        virtual ~Interface() { }
                        virtual void test() = 0; // Functions, must all be virtual
                }
                */

                var membersToWrite = allMembersToWrite.ToList();
                //.Where(o => IsStatic(o) == staticMembers).ToList();

                //                    if (membersToWrite.Count == 0 && (partials.Any(o => o.Syntax.Modifiers.Any(SyntaxKind.StaticKeyword))))
                //                        continue;

                //                    if (staticMembers)
                //                        writer.Write("object ");
                //                    else if (first.Syntax.Kind == SyntaxKind.InterfaceDeclaration)
                //                        writer.Write("trait ");
                //                    else
                //                    {
                //                        if (partials.Any(o => o.Syntax.Modifiers.Any(SyntaxKind.AbstractKeyword)))
                //                            writer.Write("abstract ");

                //                    }

                // writer.Write(TypeState.Instance.TypeName);

                if (first.Syntax is TypeDeclarationSyntax)
                {
                    //Look for generic arguments 
//                    var genericArgs = partials
//                        .Select(o => o.Syntax)
//                        .Cast<TypeDeclarationSyntax>()
//                        .Where(o => o.TypeParameterList != null)
//                        .SelectMany(o => o.TypeParameterList.Parameters)
//                        .ToList();

                    if (Context.Instance.Type.TypeKind == TypeKind.Class)
                        writer.Write("class ");
                    else if (Context.Instance.Type.TypeKind == TypeKind.Interface)
                        writer.Write("interface ");
                    else if (Context.Instance.Type.TypeKind == TypeKind.Struct)
                    {
                        writer.Write("struct ");
                        //						writer.Write (" class "); // Allows inheritance ... but too many issues, will look at this when it gets relevant
                    }
                    else
                        throw new Exception("don't know how to write type: " + Context.Instance.Type.TypeKind);
                    //writer.Write (((TypeState.Instance.Type.TypeKind== TypeKind.Interface)?" interface ": ) );

                    writer.Write(Context.Instance.TypeName);

                    if (Context.Instance.Type.IsGenericType)
                    {

                        {
                         //   List<string> genArgs = new List<string>();
                            foreach (var @base in bases)
                            {
                                foreach (var arg in (@base as INamedTypeSymbol).TypeParameters)
                                {
                                    
                                    if (!genericArgs.Any(k=>k.Name==arg.Name))
                                    {
                                        genericArgs.Add(arg);
                                    }
                                }
                            }
                            if (genericArgs.Any())
                            {
                                writer.Write("("+string.Join(" , ", genericArgs.Select(o => o))+")");
                            }
                        }
                    }

                    bool firstBase = true;

                    if (Context.Instance.Type.TypeKind != TypeKind.Struct)
                    {
                        foreach (var baseType in bases.OrderBy(o => o.TypeKind == TypeKind.Interface ? 1 : 0))
                        {
                            var convertType = TypeProcessor.ConvertType(baseType);

                         

                            writer.Write(firstBase ? " : " : " ,");

                            
                            writer.Write(convertType);


                            firstBase = false;
                        }
                    }
                }

                writer.WriteLine();

                writer.OpenBrace();

                var fields = membersToWrite.OfType<FieldDeclarationSyntax>().ToList();
                var nonFields = membersToWrite.Except(fields); // also static fields should be separately dealt with

                var structLayout = first.Syntax.GetAttribute(Context.StructLayout);
                if (structLayout != null)
                {
                    LayoutKind value = LayoutKind.Auto;
                    if (
                        structLayout.ArgumentList.Arguments.Any(
                            k => k.NameEquals != null && k.NameEquals.Name.ToFullString().Trim() == "Value"))
                    {
                        value =
                            (LayoutKind)
                                Enum.Parse(typeof (LayoutKind),
                                    structLayout.ArgumentList.Arguments.FirstOrDefault(
                                        k => k.NameEquals != null && k.NameEquals.Name.ToFullString().Trim() == "Value")
                                        .Expression.ToFullString()
                                        .SubstringAfterLast('.'));
                       
                    }
                       
                    else if (structLayout.ArgumentList.Arguments.Count > 0 &&
                             structLayout.ArgumentList.Arguments[0].NameEquals == null)
                    {
                        value =
                            (LayoutKind)
                                Enum.Parse(typeof (LayoutKind),
                                    structLayout.ArgumentList.Arguments[0].Expression.ToFullString()
                                        .SubstringAfterLast('.'));
                    }
                    int pack = -1;
                    int size = -1;
                    CharSet charset = CharSet.Auto;
//					if (structLayout.ArgumentList.Arguments.Count > 1)
                    {
                        try
                        {
                            if (
                                structLayout.ArgumentList.Arguments.Any(
                                    k => k.NameEquals != null && k.NameEquals.Name.ToFullString().Trim() == "CharSet"))
                            {
                                charset =
                                    (CharSet)
                                        Enum.Parse(typeof (CharSet),
                                            structLayout.ArgumentList.Arguments.FirstOrDefault(
                                                k =>
                                                    k.NameEquals != null &&
                                                    k.NameEquals.Name.ToFullString().Trim() == "CharSet")
                                                .Expression.ToFullString()
                                                .SubstringAfterLast('.'));
                                //structLayout.ArgumentList.Arguments.Where (k => k.Expression is MemberAccessExpressionSyntax).FirstOrDefault(k=>(k.Expression as MemberAccessExpressionSyntax).Name.ToFullString() == "Value");
                            }
                                //structLayout.ArgumentList.Arguments.Where (k => k.Expression is MemberAccessExpressionSyntax).FirstOrDefault(k=>(k.Expression as MemberAccessExpressionSyntax).Name.ToFullString() == "Value");
                            else if (structLayout.ArgumentList.Arguments.Count > 1 &&
                                     structLayout.ArgumentList.Arguments[1].NameEquals == null)
                            {
                                charset =
                                    (CharSet)
                                        Enum.Parse(typeof (CharSet),
                                            structLayout.ArgumentList.Arguments[1].Expression.ToFullString()
                                                .SubstringAfterLast('.'));
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                        try
                        {
                            if (
                                structLayout.ArgumentList.Arguments.Any(
                                    k => k.NameEquals != null && k.NameEquals.Name.ToFullString().Trim() == "Pack"))
                            {
                                pack =
                                    int.Parse(
                                        structLayout.ArgumentList.Arguments.FirstOrDefault(
                                            k =>
                                                k.NameEquals != null &&
                                                k.NameEquals.Name.ToFullString().Trim() == "Pack")
                                            .Expression.ToFullString());
                            }
                                //structLayout.ArgumentList.Arguments.Where (k => k.Expression is MemberAccessExpressionSyntax).FirstOrDefault(k=>(k.Expression as MemberAccessExpressionSyntax).Name.ToFullString() == "Value");
                            else if (structLayout.ArgumentList.Arguments.Count > 2 &&
                                     structLayout.ArgumentList.Arguments[2].NameEquals == null)
                                pack = int.Parse(structLayout.ArgumentList.Arguments[2].Expression.ToFullString());
                        }
                        catch (Exception ex)
                        {
                        }

                        try
                        {
                            if (
                                structLayout.ArgumentList.Arguments.Any(
                                    k => k.NameEquals != null && k.NameEquals.Name.ToFullString().Trim() == "Size"))
                            {
                                size =
                                    int.Parse(
                                        structLayout.ArgumentList.Arguments.FirstOrDefault(
                                            k => k.NameColon != null && k.NameColon.ToFullString().Trim() == "Size")
                                            .Expression.ToFullString());
                            }
                                //structLayout.ArgumentList.Arguments.Where (k => k.Expression is MemberAccessExpressionSyntax).FirstOrDefault(k=>(k.Expression as MemberAccessExpressionSyntax).Name.ToFullString() == "Value");
                            else if (structLayout.ArgumentList.Arguments.Count > 3 &&
                                     structLayout.ArgumentList.Arguments[3].NameEquals == null)
                                size = int.Parse(structLayout.ArgumentList.Arguments[3].Expression.ToFullString());
                        }
                        catch (Exception ex)
                        {
                        }

//						size = int.Parse (structLayout.ArgumentList.Arguments [3].Expression.ToFullString ());
                    }
                    //					var pack = structLayout.ArgumentList.Arguments.FirstOrDefault (k => k.NameColon.Name.ToFullString() == "Pack");
//					var charset = structLayout.ArgumentList.Arguments.FirstOrDefault (k => k.NameColon.Name.ToFullString() == "CharSet");
//					var size = structLayout.ArgumentList.Arguments.FirstOrDefault (k => k.NameColon.Name.ToFullString() == "Size");

                    if (value == LayoutKind.Explicit)
                    {
                        var fieldGroups =
                            fields.GroupBy(f => f.GetAttribute(Context.FieldOffset).ArgumentList.Arguments[0].ToString())
                                .OrderBy(k => k.Key);
                        writer.Indent++;

                        foreach (var group in fieldGroups)
                        {
                            writer.WriteLine("//FieldOffset(" + @group.Key + ")");
                            writer.WriteLine("union {");
                            foreach (var member in group)
                                Core.Write(writer, member);
                            writer.WriteLine("}");
                        }

//						foreach (var member in fields)
//						{
//							//                    writer.WriteLine();
//							Core.Write (writer, member);
//						}
                    }
                    else if (value == LayoutKind.Sequential)
                    {
                        fields = SortFields(fields);

                        writer.Indent++;

                        foreach (var member in fields)
                        {
                            if (pack != -1)
                                writer.WriteLine("align (" + pack + "): //Pack = " + pack);
                            //                    writer.WriteLine();
                            Core.Write(writer, member);
                        }
                    }

                    else
                    {
                        //Looks like C# aligns to 1 by default ... don't know about D...
                        fields = SortFields(fields);

                        writer.Indent++;
                        foreach (var member in fields)
                        {
                            pack = 1;
                            writer.WriteLine("align (" + pack + "): //Pack = " + pack + " C# default");
                            //                    writer.WriteLine();
                            Core.Write(writer, member);
                        }
                    }
                }
                else
                {
                    //Looks like C# aligns to 1 by default ... don't know about D...
                    fields = SortFields(fields);

                    writer.Indent++;
                    foreach (var member in fields)
                    {
                        var pack = 1;
                        writer.WriteLine("align (" + pack + "): //Pack = " + pack + "C# default");
                        //                    writer.WriteLine();
                        Core.Write(writer, member);
                    }
                }

                foreach (var member in nonFields)
                {
//                    writer.WriteLine();
                    Core.Write(writer, member);
                }

                foreach (var constructor in instanceCtors)
                {
//                    writer.WriteLine();
                    Core.Write(writer, constructor);
                }

                if (staticCtors.Count == 0)
                {
                    if (Context.Instance.StaticInits.Count > 0)
                    {
                        var constructor = SyntaxFactory.ConstructorDeclaration(Context.Instance.TypeName);
                        constructor =
                            constructor.WithModifiers(
                                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

                        WriteConstructorBody.WriteStaticConstructor(writer, constructor, Context.Instance.StaticInits);

                        staticCtors.Add(constructor);
                    }
                }
                else
                {
                    var isFirst = true;
                    foreach (ConstructorDeclarationSyntax constructor in staticCtors)
                    {
                        if (isFirst)
                        {
                            WriteConstructorBody.WriteStaticConstructor(writer, constructor,
                                Context.Instance.StaticInits);
                        }
                        else
                            WriteConstructorBody.Go(writer, constructor);

                        isFirst = false;
                    }
                }

                var dllImports = Context.Instance.DllImports;
                    // This should only be written once ... I guess we have to put the detection logic in Program.cs

                if (Context.Instance.EntryMethod != null)
                {
                    if (dllImports.Count > 0)
                    {
                        writer.WriteLine();

                        writer.WriteLine(String.Format("static void * __DllImportMap[{0}];", dllImports.Count));
                        writer.WriteLine("static void __SetupDllImports()");

                        writer.OpenBrace();

                        for (int index = 0; index < dllImports.Count; index++)
                        {
                            var dllImport = dllImports[index];
                            writer.WriteLine(String.Format("__DllImportMap[{0}] = LoadNativeLibrary(cast(string){1});",
                                index, dllImport.ArgumentList.Arguments.FirstOrDefault(k => k.Expression != null)));
                        }
                        writer.CloseBrace();
                        writer.WriteLine();
                        writer.WriteLine("static void __FreeDllImports()");
                        writer.OpenBrace();
                        writer.WriteLine(String.Format(@"for(int i=0;i<{0};i++)", dllImports.Count));
                        writer.OpenBrace();
                        writer.WriteLine("if(__DllImportMap[i]!=null)");
                        writer.WriteLine("\tFreeNativeLibrary(__DllImportMap[i]);");
                        writer.CloseBrace();
                        writer.CloseBrace();

                        //                            if (hModule != null)
                        //                            {
                        //    ::FreeLibrary(hModule);
                        //                            }
                    }
                }
                writer.Indent--;
                writer.CloseBrace();

                //Implement Boxed!Interface
                if (Context.Instance.Type.TypeKind == TypeKind.Struct)
                {
                    writer.WriteLine();
                    var typeName = Context.Instance.TypeName + (Context.Instance.Type.IsGenericType? ("!(" + string.Join(" , ", genericArgs.Select(o => o)) + ")") :"");
                    writer.Write("public class __Boxed_" + typeName + " ");

                    if (genericArgs.Any())
                    {
                        writer.Write("( ");
                        writer.Write(string.Join(" , ", genericArgs.Select(o => o)));
                        writer.Write(" )");
                    }

                    writer.Write(": Boxed!(" + typeName + ")");

                    foreach (var baseType in bases.OrderBy(o => o.TypeKind == TypeKind.Interface ? 1 : 0))
                    {
                        writer.Write(" ,");

                        writer.Write(TypeProcessor.ConvertType(baseType, false));
                    }

                    writer.OpenBrace();

                    //FIXME:This is giving issues, we will just generate them here

//					writer.WriteLine ("import Irio.Utilities;");

                    writer.WriteLine("import std.traits;");

                    foreach (var baseType in bases.Where(o => o.TypeKind == TypeKind.Interface))
                    {
                        //FIXME:This is giving issues, we will just generate them here
//						writer.WriteLine ("mixin(__ImplementInterface!({0}, Value));",TypeProcessor.ConvertType(baseType,false)); 
                        var ifacemembers = baseType.GetMembers();

                        foreach (var member in ifacemembers)
                        {
                            var ifacemethod =
                                Context.Instance.Type.FindImplementationForInterfaceMember(member)
                                    .DeclaringSyntaxReferences.First()
                                    .GetSyntax();
//								.Where(member => !(member is TypeDeclarationSyntax)
//									&& !(member is EnumDeclarationSyntax)
//									&& !(member is DelegateDeclarationSyntax) && !(member is ConstructorDeclarationSyntax))
//								.ToList();

                            //                    writer.WriteLine();
//							Core.Write(writer, member);

                            if (ifacemethod is MethodDeclarationSyntax)
                                WriteMethod.WriteIt(writer, (MethodDeclarationSyntax) ifacemethod);
                            else if (ifacemethod is PropertyDeclarationSyntax)
                                WriteProperty.Go(writer, (PropertyDeclarationSyntax) ifacemethod, true);
                        }
                    }

                    //This is required to be able to create an instance at runtime / reflection
//					this()
                    //					{
                    //						super(SimpleStruct.init);
                    //					}

                    writer.WriteLine();
                    writer.WriteLine("this()");
                    writer.OpenBrace();
                    writer.WriteLine("super({0}.init);", typeName);
                    writer.CloseBrace();

                    if (Context.Instance.Type.GetMembers("ToString").Count() >= 1) // Use better matching ?
                    {
                        //					writer.WriteLine ();
                        writer.WriteLine("override String ToString()");
                        writer.OpenBrace();
                        writer.WriteLine("return Value.ToString();");
                        writer.CloseBrace();
                    }

                    writer.WriteLine();
                    writer.WriteLine("this(ref " + typeName + " value)");
                    writer.OpenBrace();
                    writer.WriteLine("super(value);");
                    writer.CloseBrace();

                    writer.WriteLine();
                    writer.WriteLine("U opCast(U)()");
                    writer.WriteLine("if(is(U:{0}))", typeName);
                    writer.OpenBrace();
                    writer.WriteLine("return Value;");
                    writer.CloseBrace();

                    writer.WriteLine();
                    writer.WriteLine("U opCast(U)()");
                    writer.WriteLine("if(!is(U:{0}))", typeName);
                    writer.OpenBrace();
                    writer.WriteLine("return this;");
                    writer.CloseBrace();

                    writer.WriteLine();
                    writer.WriteLine("auto opDispatch(string op, Args...)(Args args)");
                    writer.OpenBrace();
                    writer.WriteLine("enum name = op;");
                    writer.WriteLine("return __traits(getMember, Value, name)(args);");
                    writer.CloseBrace();

                    writer.CloseBrace();
                }

                if (Context.Instance.EntryMethod != null)
                {
                    //TODO: DllImports should be centralized

                    //
                    writer.WriteLine();
                    writer.WriteLine("void main(string[] args)");
                    writer.OpenBrace();

                    if (dllImports.Count > 0)
                        writer.WriteLine(Context.Instance.TypeName + ".__SetupDllImports();");
                    writer.WriteLine(Context.Instance.EntryMethod);
                    if (dllImports.Count > 0)
                        writer.WriteLine(Context.Instance.TypeName + ".__FreeDllImports();");

                    writer.CloseBrace();
                }

                //				var mySpecializations = Program.AllGenericSpecializations.Where (t => t.OriginalDefinition == TypeState.Instance.Type);
                //
                //
                //				foreach (var specialization in mySpecializations) {
                //
                //					var specializationText = ("template class " + specialization.GetFullNameD (false) + " (" +
                //					                                            (string.Join (" , ",
                //						                                            specialization.TypeArguments.Select (
                //							                                            o =>
                //                                                                       TypeProcessor.ConvertType (o) +
                //							                                            ((o.IsValueType ||
                //							                                            o.TypeKind == TypeKind.TypeParameter)
                //                                                                           ? ""
                //                                                                           : "*"))) +
                //							");"));
                //					writer.Write (specializationText);
                //				}

                //                    if (@namespace.Length > 0)
                //                    {
                //                        foreach (var ns in namespaces)
                //                        {
                //                            writer.WriteLine("\n\n}");
                //                            writer.WriteLine("\n\n}");
                //                        }
                //                    }
            }
        }

        public static OutputWriter EntryPoint = null;

        private static List<FieldDeclarationSyntax> SortFields(List<FieldDeclarationSyntax> fields)
        {
            if (fields.Count == 0)
                return fields;

            var dependencies =
                fields.ToDictionary(
                    o => TypeProcessor.GetDeclaredSymbol(o.Declaration.Variables.First()).As<IFieldSymbol>(),
                    o => new {Syntax = o, Dependicies = new List<IFieldSymbol>()});

            foreach (var dep in dependencies)
            {
                foreach (
                    var fieldDepend in
                        dep.Value.Syntax.DescendantNodes()
                            .OfType<ExpressionSyntax>()
                            .Select(o => TypeProcessor.GetSymbolInfo(o).Symbol)
                            .OfType<IFieldSymbol>())
                {
                    if (dependencies.ContainsKey(fieldDepend))
                        dep.Value.Dependicies.Add(fieldDepend);
                }
            }

            var ret = new List<FieldDeclarationSyntax>();
            var symbolsAdded = new HashSet<IFieldSymbol>();

            while (dependencies.Count > 0)
            {
                foreach (var dep in dependencies.ToList())
                {
                    for (int i = 0; i < dep.Value.Dependicies.Count; i++)
                    {
                        if (symbolsAdded.Contains(dep.Value.Dependicies[i]))
                            dep.Value.Dependicies.RemoveAt(i--);
                    }

                    if (dep.Value.Dependicies.Count == 0)
                    {
                        ret.Add(dep.Value.Syntax);
                        symbolsAdded.Add(dep.Key);
                        dependencies.Remove(dep.Key);
                    }
                }
            }

            return ret;
        }

      

        private static bool IsStatic(MemberDeclarationSyntax member)
        {
            var modifiers = member.GetModifiers();
            return modifiers.Any(SyntaxKind.StaticKeyword) || modifiers.Any(SyntaxKind.ConstKeyword);
        }


        public static string TypeName(INamedTypeSymbol type)
        {
            var sb =
                new StringBuilder(string.Join("_",
                    (new[] {type.Name}).Union(type.TypeArguments.Select(o => o.ToString()))));

            while (type.ContainingType != null)
            {
                type = type.ContainingType;
                sb.Insert(0,
                    string.Join("_", (new[] {type.Name}).Union(type.TypeArguments.Select(o => o.ToString()))) + "_");
                sb.Append(string.Join("_", type.TypeArguments.Select(o => o)));
            }

            return sb.ToString();
        }
    }
}