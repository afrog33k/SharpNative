// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        static Dictionary<string, string> TypeRenames = new Dictionary<string, string>()
        {
            { "System.Namespace.Object","NObject"},
            { "System.Namespace.Exception","NException"}
        };

        public static void Go(OutputWriter outputWriter = null)
        {
            var partials = Context.Instance.Partials;
            var first = partials.First();
            bool fileExists = false;
            foreach (var @partial in partials.GroupBy(o => o.Symbol))
            {
                Context.Instance.Namespace = @partial.First().Symbol.ContainingNamespace.FullName();
                Context.Instance.Type = @partial.First().Symbol;
                WriteOutOneType(outputWriter, @partial.ToArray(), fileExists);
                fileExists = true;
            }
        }

        private static void WriteOutOneType(OutputWriter parentModuleWriter, Context.SyntaxAndSymbol[] typeSymbols, bool fileExists)
        {
            TypeProcessor.ClearUsedTypes();
            var mynamespace = Context.Instance.Type.ContainingNamespace.FullName().RemoveFromEndOfString(".Namespace");

            var fullname = Context.Instance.Namespace + "." + Context.Instance.TypeName;
            if (TypeRenames.ContainsKey(fullname))
                Context.Instance.TypeName = TypeRenames[fullname];

            // + "." + TypeState.Instance.TypeName;
          // if (Driver.Verbose)
            //    Console.WriteLine("Writing out type: " + fullname);
            if (fullname.StartsWith(
                "System.Collections.Generic.Dictionary"))
            {
            }

            var myUsingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(mynamespace));
            var SystemUsingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
            // Required as certain functions like boxing are in this namespace
            var namespaces = typeSymbols.First().Syntax.Parent.DescendantNodes().OfType<UsingDirectiveSyntax>().ToArray();
            var usingStatements = typeSymbols.First().Syntax.Parent.DescendantNodes().OfType<UsingStatementSyntax>().ToArray();
            var allTypeAliases = typeSymbols.First().Syntax.DescendantNodes().OfType<QualifiedNameSyntax>().ToArray();
            Context.Instance.UsingDeclarations = namespaces
                .Union(new[]
                {
                    myUsingDirective, SystemUsingDirective
                }).ToArray();


         

            TypeProcessor.AddAlias(Context.Instance.Type.ContainingNamespace as INamespaceSymbol, Context.Instance.Type.ContainingNamespace.GetModuleName());


            foreach (var ns in namespaces)
            {
                //TODO: Add support for type aliases ...
                var symbol = TypeProcessor.GetSymbolInfo(ns.Name).Symbol;
                if (allTypeAliases.All(o => TypeProcessor.GetSymbolInfo(o.Left).Symbol != symbol))
                    TypeProcessor.AddAlias(symbol as INamespaceSymbol, (symbol as INamespaceSymbol).GetModuleName());
            }

            var aliases = allTypeAliases.DistinctBy(j => TypeProcessor.GetSymbolInfo(j.Left));
            foreach (var alias in aliases)
            {
                var left = alias.Left;
                var type = TypeProcessor.GetSymbolInfo(left).Symbol as INamespaceSymbol;
                var name = left.ToString();
                if (type != null && type.ToString() != name)
                {
                    TypeProcessor.AddAlias(type, name);
                }

            }
            OutputWriter writer = null;

            using (
                writer =
                    parentModuleWriter == null
                        ? new OutputWriter(Context.Instance.Namespace, Context.Instance.TypeName)
                        : new TempWriter())
            {


                writer.FileExists = fileExists;

                if (parentModuleWriter != null)
                {
                    writer.WriteLine();
                    writer.Indent = parentModuleWriter.Indent + 2;
                    writer.WriteIndent();
                }

                var bases = new List<ITypeSymbol>();
                var baselist = typeSymbols.Select(k => k.Syntax.As<BaseTypeDeclarationSyntax>()).Select(o => o.BaseList).Where(k => k != null).ToArray();

                if (baselist.Any())
                {
                    bases = baselist.SelectMany(k => k.Types)
                        .Select(o => TypeProcessor.GetTypeInfo(o.Type).ConvertedType ?? TypeProcessor.GetTypeInfo(o.Type).Type)
                        .Where(k=>k!=null)
                        .Distinct()
                        .ToList();
                }

                //                var interfaces = bases.Where(o => o.TypeKind == TypeKind.Interface).ToList();

                if (Context.Instance.Type != Context.Object)
                {
                    if (bases != null && (!bases.Any((j => j.TypeKind != TypeKind.Interface)) &&
                                          !(typeSymbols.First().Symbol.TypeKind == TypeKind.Interface || typeSymbols.First().Symbol.TypeKind == TypeKind.Struct)))
                        //TODO: fix structs using mixins / alias this
                        bases.Add(Context.Object);
                }
                if(bases==null)
                    bases =  new List<ITypeSymbol>();

                foreach (var type in bases)
                {
                    TypeProcessor.AddUsedType(type);
                }

                //TODO: Fix enum support
                if (typeSymbols.First().Syntax is EnumDeclarationSyntax)
                {
                    WriteEnum.Go(writer,
                        Context.Instance.Partials.Select(o => o.Syntax)
                            .Cast<EnumDeclarationSyntax>()
                            .SelectMany(o => o.Members)
                            .Where(o => !Program.DoNotWrite.ContainsKey(o)));

                    if (parentModuleWriter != null)
                        parentModuleWriter.Write(writer.ToString());

                    return;
                }

                Context.Instance.AllMembers =
                    typeSymbols.Select(k => k.Syntax.As<TypeDeclarationSyntax>()).SelectMany(o => o.Members)
                        .Where(o => !Program.DoNotWrite.ContainsKey(o))
                        .ToList();

                var allMembersToWrite = Context.Instance.AllMembers
                    .Where(member => !(member is TypeDeclarationSyntax)
                                     && !(member is EnumDeclarationSyntax)
                                     && !(member is DelegateDeclarationSyntax) &&
                                     !(member is ConstructorDeclarationSyntax))
                    .ToList();

                Context.Instance.MemberNames = allMembersToWrite.Select(k => k.GetCSharpName()).ToList();
                if (Context.Instance.Type.ContainingType != null)
                {
                    Context.Instance.MemberNames.AddRange(Context.Instance.Type.ContainingType.MemberNames);
                }
               

                {


                    //                    WriteStandardIncludes.Go(writer);

                    //                    writer.WriteLine(String.Format("#include \"{0}\"", TypeState.Instance.TypeName + ".h"));



                    WriteBcl.Go(writer);

                    //TypeState.Instance.DerivesFromObject = bases.Count == interfaces.Count;

                    var @namespace = typeSymbols.First().Symbol.ContainingNamespace.FullName();
                    var genericArgs = Context.Instance.Type.TypeParameters.Select(l => l as ITypeSymbol).ToList();

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

                    if (typeSymbols.First().Syntax is TypeDeclarationSyntax)
                    {
                        //Internal classes/structs are declared static in D to behave correctly
                        if (parentModuleWriter != null)
                            writer.Write("static ");
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
                        List<ITypeSymbol> parentTypeParameters;
                        if (Context.Instance.Type.ContainingType != null)
                            parentTypeParameters = GetParentTypeParameters(Context.Instance.Type);
                        else
                            parentTypeParameters = new List<ITypeSymbol>();

                        writer.Write(TypeName(Context.Instance.Type, false));
                        //TypeProcessor.ConvertType(Context.Instance.Type, true, false,true));

                        if (Context.Instance.Type.IsGenericType)
                        {

                            {
                                foreach (var @base in bases)
                                {
                                    var namedTypeSymbol = @base as INamedTypeSymbol;
                                    if (namedTypeSymbol != null)
                                    {
                                        foreach (var arg in namedTypeSymbol.TypeArguments)
                                        {
                                            if (arg.TypeKind == TypeKind.TypeParameter && !parentTypeParameters.Contains(arg))
                                            {
                                                if (!genericArgs.Any(k => k.Name == arg.Name))
                                                    genericArgs.Add(arg);
                                            }
                                        }
                                    }
                                }
                                if (genericArgs.Any())
                                {
                                    writer.Write("(" +
                                                 string.Join(" , ",
                                                     genericArgs.Select(o => TypeProcessor.ConvertType(o, true, true, false))) + ")");
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

                        string constraints = GetTypeConstraints((TypeDeclarationSyntax) typeSymbols.First().Syntax);
                        writer.Write(constraints);
                    }

                    writer.WriteLine();

                    writer.OpenBrace();

                    var nonFields = WriteFields(membersToWrite, typeSymbols.First(), writer);

                    foreach (var member in nonFields)
                    {
                        //                    writer.WriteLine();
                        Core.Write(writer, member);
                    }

                    WriteConstructors(instanceCtors, writer);

                    WriteStaticConstructors(staticCtors, writer);

                    //PInvoke is now centralized, so we can call it on all libraries etc without issue

                    writer.Indent--;
                    WriteOutNestedTypes(typeSymbols.First(), writer);

                    var methodSymbols = membersToWrite.OfType<MethodDeclarationSyntax>().Select(TypeProcessor.GetDeclaredSymbol);
                    //TypeProcessor.GetDeclaredSymbol(method);

                    if (!methodSymbols.OfType<IMethodSymbol>()
                       .Any(k => k.Name == "ToString" && k.Parameters.Length == 0 && k.ReturnType == Context.String))
                    {
                        if (Context.Instance.Type.TypeKind == TypeKind.Struct ||
                            (Context.Instance.Type.TypeKind == TypeKind.Class))
                        {
                            var overrideS = Context.Instance.Type.TypeKind == TypeKind.Struct ? "" : "override ";
                            writer.WriteLine();
                            writer.WriteLine("public " + overrideS + "String ToString()");
                            writer.OpenBrace();
                            writer.WriteLine("return GetType().FullName;");//Better names based on specialization
                            writer.CloseBrace();
                        }

                    }

                    WriteOutBoxed(writer, genericArgs, bases);
                }

                if (Context.Instance.Type.TypeKind == TypeKind.Struct)
                {
                    var typename = TypeProcessor.ConvertType(Context.Instance.Type, true, true, true);
                    writer.WriteLine();
                    writer.WriteLine("public __Boxed_ __Get_Boxed()");
                    writer.OpenBrace();
                    writer.WriteLine("return new __Boxed_(this);");
                    writer.CloseBrace();
                    writer.WriteLine("alias __Get_Boxed this;");
                    writer.WriteLine();


                    writer.WriteLine("public bool opEquals({0} other)",typename);
                    writer.OpenBrace();
                    var fieldSymbols = Context.Instance.Type.GetMembers().OfType<IFieldSymbol>();
                    if(fieldSymbols.Any())
                    writer.WriteLine("return " + fieldSymbols.Select(k=> "this." +  WriteIdentifierName.TransformIdentifier(k.Name) + "== other." + WriteIdentifierName.TransformIdentifier(k.Name)).Aggregate((a,b)=> a + "&&" + b) + ";");
                    else
                    {
                        writer.WriteLine("return true;");
                    }
                    writer.CloseBrace();

        writer.WriteLine();
                    writer.WriteLine("alias __Get_Boxed this;");
                }

                if (Context.Instance.Type.TypeKind != TypeKind.Interface)
                {
                    writer.WriteLine();
                    if (Context.Instance.Type.TypeKind == TypeKind.Class)
                        writer.WriteLine("public override Type GetType()");
                    //					else if (Context.Instance.Type.TypeKind == TypeKind.Interface) // Messes with GetType overrides of objects
                    //					{
                    //						writer.WriteLine ("public final Type GetType()");
                    //					}
                    else if (Context.Instance.Type.TypeKind == TypeKind.Struct)
                        writer.WriteLine("public Type GetType()");
                    writer.OpenBrace();
                    //if (Context.Instance.Type.TypeKind == TypeKind.Class)
                    writer.WriteLine("return __TypeOf!(typeof(this));");
                    // else
                    //     writer.WriteLine("return __TypeOf!(__Boxed_);");

                    writer.CloseBrace();
                }

                writer.CloseBrace();

                WriteEntryMethod(writer);

                if (parentModuleWriter != null)
                {
                    writer.Finish();
                    parentModuleWriter.Write(writer.ToString());
                }
            }
        }

        private static List<ITypeSymbol> GetParentTypeParameters(INamedTypeSymbol type)
        {
            var parames = new List<ITypeSymbol>();
            type = type.ContainingType;

            while (type != null)
            {
                parames.AddRange(type.TypeArguments);
                type = type.ContainingType;
            }
            return parames;
        }

        private static string GetTypeConstraints(TypeDeclarationSyntax method)
        {
            string constraints = "";
            if (method.ConstraintClauses.Count > 0)
            {
                constraints += (" if (");
                bool isFirst = true;
                foreach (var constraint in method.ConstraintClauses)
                {
                    foreach (var condition in constraint.Constraints)
                    {
                        if (condition is TypeConstraintSyntax)
                        {
                            var type = (condition as TypeConstraintSyntax).Type;
                            constraints += (isFirst ? "" : "&&") + "is(__BoxesTo!(" + WriteIdentifierName.TransformIdentifier(constraint.Name.ToFullString()) + "):" + TypeProcessor.ConvertType(type) +")";
                            //Added support for __BoxableObjects
                        }

                        if (condition is ConstructorConstraintSyntax)
                        {
                            constraints += (isFirst ? "" : "&&") + "__isNewwable!(" + WriteIdentifierName.TransformIdentifier(constraint.Name.ToFullString()) + ")";
                        }

                        if (condition is ClassOrStructConstraintSyntax)
                        {
                            var properConstraint = condition as ClassOrStructConstraintSyntax;
                            if (properConstraint.ClassOrStructKeyword.RawKind == (decimal) SyntaxKind.StructKeyword)
                            {
                                constraints += (isFirst ? "" : "&&") + "__isCSStruct!(" + WriteIdentifierName.TransformIdentifier(constraint.Name.ToFullString()) + ")";
                            }
                            else
                            {
                                constraints += (isFirst ? "" : "&&") + "__isClass!(" + WriteIdentifierName.TransformIdentifier(constraint.Name.ToFullString()) + ")";
                            }
                        }

                      /*  string dlangCondition = condition.ToString();

                        if (dlangCondition == "new()") // haven't got around to this yet
                            // constraints += " __traits(compiles, {0}())";//Fix this
                            dlangCondition = "";
                        if (dlangCondition == "class") // TODO: is there a better way to do this ?
                            dlangCondition = "NObject";

                        if (dlangCondition == "struct")
                            constraints += ((isFirst ? "" : "&&") + " !is(" + constraint.Name + " : NObject)");
                        else
                        {
                            //TODO: fix this up better
                            constraints += ((isFirst ? "" : "&&") + " is(" + constraint.Name + " : " + dlangCondition.Replace("<", "!(").Replace(">", ")") +
                                            ")");
                        }*/

                        isFirst = false;

                        //								Console.WriteLine (condition);
                    }
                }

                constraints += (")");
            }
            return constraints;
        }

        private static void WriteStaticConstructors(List<ConstructorDeclarationSyntax> staticCtors, OutputWriter writer)
        {
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
        }

        private static void WriteConstructors(List<ConstructorDeclarationSyntax> instanceCtors, OutputWriter writer)
        {

            int constructorCount = 0;
            if (instanceCtors.Count == 0)
            {
                if (Context.Instance.InstanceInits.Count > 0)
                {
                    var constructor = SyntaxFactory.ConstructorDeclaration(Context.Instance.TypeName);

                    constructor =
                        constructor.WithModifiers(
                            SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));


                    WriteConstructorBody.WriteInstanceConstructor(writer, constructor, Context.Instance.InstanceInits);

                    instanceCtors.Add(constructor);
                    constructorCount++;
                }
            }
            else
            {
                var isFirst = true;
                foreach (ConstructorDeclarationSyntax constructor in instanceCtors)
                {
                    //TODO: centralize this to a function
                    //	if (isFirst)
                    //	{
                    WriteConstructorBody.WriteInstanceConstructor(writer, constructor,
                        Context.Instance.InstanceInits);
                    //	}
                    //	else
                    //		WriteConstructorBody.Go(writer, constructor);

                    isFirst = false;
                    constructorCount++;
                }
            }

            if (Context.Instance.Type.TypeKind == TypeKind.Struct) // struct with constructors
            {
                if (instanceCtors.All(k => k.ParameterList.Parameters.Count != 0))
                {
                    writer.WriteLine("void __init(){}//default xtor");
                }
                writer.WriteLine("static {0} opCall(__U...)(__U args_)", Context.Instance.TypeName);
                writer.OpenBrace();
                writer.WriteLine(" {0} s;", Context.Instance.TypeName);
                writer.WriteLine("s.__init(args_);");
                writer.WriteLine("return s;");
                writer.CloseBrace();
            }

        }

        private static IEnumerable<MemberDeclarationSyntax> WriteFields(List<MemberDeclarationSyntax> membersToWrite, Context.SyntaxAndSymbol first, OutputWriter writer)
        {
            var fields = membersToWrite.OfType<FieldDeclarationSyntax>().ToList();
            var nonFields = membersToWrite.Except(fields); // also static fields should be separately dealt with

            //Not needed anymore ... reflection takes care of this
            //if (fields.Count > 0 && fields.Any(j => j.GetModifiers().Any(SyntaxKind.ConstKeyword) ||
            //                                        j.GetModifiers().Any(SyntaxKind.StaticKeyword)))
            //{
            //    writer.WriteLine("enum __staticFieldTuple = __Tuple!(" +
            //                     fields.Where(
            //                         j =>
            //                             j.GetModifiers().Any(SyntaxKind.ConstKeyword) ||
            //                             j.GetModifiers().Any(SyntaxKind.StaticKeyword))
            //                         .Select(
            //                             k =>
            //                                 "\"" +
            //                                 WriteIdentifierName.TransformIdentifier(
            //                                     k.Declaration.Variables[0].Identifier.ValueText) + "\"")
            //                         .Aggregate((i, j) => i + "," + j) + ");//Reflection Support");
            //}

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
                            Enum.Parse(typeof(LayoutKind),
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
                            Enum.Parse(typeof(LayoutKind),
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
                                    Enum.Parse(typeof(CharSet),
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
                                    Enum.Parse(typeof(CharSet),
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
                        Context.Instance.ShouldInitializeVariables = false;
                        foreach (var member in @group)
                            Core.Write(writer, member);
                        Context.Instance.ShouldInitializeVariables = true;
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
                        //TODO: on mac osx and mono this is required, on windows, it causes and issue
                        //                            writer.WriteLine("align (" + pack + "): //Pack = " + pack + " C# default");
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
                    //TODO: on mac osx and mono this is required, on windows, it causes an issue (sizes are different)
                    //                        writer.WriteLine("align (" + pack + "): //Pack = " + pack + "C# default");
                    //                    writer.WriteLine();
                    Core.Write(writer, member);
                }
            }

            return nonFields;
        }

        private static void WriteEntryMethod(OutputWriter writer)
        {
            if (Context.Instance.EntryMethod != null)
            {
                //TODO: DllImports should be centralized

                //
                writer.WriteLine();
                writer.WriteLine("void main(string[] args)");
                writer.OpenBrace();

                writer.WriteLine("__SetupSystem();");
                writer.WriteLine(Context.Instance.EntryMethod);
                writer.WriteLine("__EndSystem();");

                writer.CloseBrace();
            }
        }

        private static void WriteOutBoxed(OutputWriter writer, List<ITypeSymbol> genericArgs, List<ITypeSymbol> bases)
        {
            //Implement Boxed!T
            if (Context.Instance.Type.TypeKind == TypeKind.Struct)
            {
                writer.WriteLine();
                var typeName = TypeProcessor.ConvertType(Context.Instance.Type); /*TypeName(Context.Instance.Type, false) +
                               (Context.Instance.Type.IsGenericType && genericArgs.Any()
                                   ? ("!(" + string.Join(" , ", genericArgs.Select(o => o)) + ")")
                                   : "");*/

                var baseString = "";

                foreach (var baseType in bases.OrderBy(o => o.TypeKind == TypeKind.Interface ? 1 : 0))
                {
                    baseString += (" ,");

                    baseString += (TypeProcessor.ConvertType(baseType, false));
                }

                writer.WriteLine("public static class __Boxed_" + " " +
                                 //(genericArgs.Any() ? ("( " + (string.Join(" , ", genericArgs.Select(o => o)) + " )")) : "") +//Internal boxed should not be generic
                                 ": Boxed!(" + typeName + ")" + baseString);

                writer.OpenBrace();



                writer.WriteLine("import std.traits;");

                var members = new List<ISymbol>();

                foreach (var baseType in bases.Where(o => o.TypeKind == TypeKind.Interface))
                {
                    members.AddRange(Utility.GetAllMembers(baseType));
                }

                //  foreach (var baseType in bases.Where(o => o.TypeKind == TypeKind.Interface))
                //{

                var ifacemembers = members.DistinctBy(k => k);//Utility.GetAllMembers(Context.Instance.Type);

                foreach (var member in ifacemembers)
                {
                    var findImplementationForInterfaceMember = Context.Instance.Type.FindImplementationForInterfaceMember(member);
                    if (findImplementationForInterfaceMember == null)
                    {
                        continue;
                    }
                    var ifacemethod =
                        findImplementationForInterfaceMember
                            .DeclaringSyntaxReferences.First()
                            .GetSyntax();

                    var syntax = ifacemethod as MethodDeclarationSyntax;
                    if (syntax != null)
                        WriteMethod.WriteIt(writer, syntax);

                    var property = ifacemethod as PropertyDeclarationSyntax;
                    if (property != null)
                        WriteProperty.Go(writer, property, true);

                }


                //                {
                //                    writer.WriteLine("public override String ToString()");
                //                    writer.OpenBrace();
                //                    writer.WriteLine("return __Value.ToString();", Program.GetGenericMetadataName(Context.Instance.Type));
                //                    writer.CloseBrace();
                //                }

                //}

                //This is required to be able to create an instance at runtime / reflection
                //					this()
                //					{
                //						super(SimpleStruct.init);
                //					}

                writer.WriteLine();
                writer.WriteLine("this()");
                writer.OpenBrace();
                writer.WriteLine("super({0}.init);", typeName); //TODO fix this for another TypeNew that calls the valuetype constructor only
                writer.CloseBrace();

                //                if (Context.Instance.Type.GetMembers("ToString").Any()) // Use better matching ?
                {
                    //					writer.WriteLine ();
                    writer.WriteLine("public override String ToString()");
                    writer.OpenBrace();
                    writer.WriteLine("return __Value.ToString();");
                    writer.CloseBrace();
                }

                writer.WriteLine();
                writer.WriteLine("public override bool Equals(NObject other)");
                writer.OpenBrace();
                writer.WriteLine("if (cast(Boxed!({0})) other)", typeName);
                writer.OpenBrace();
                writer.WriteLine("auto otherValue = (cast(Boxed!({0})) other).__Value;", typeName);
                writer.WriteLine("return otherValue == __Value;");
                writer.CloseBrace();
                writer.WriteLine("return false;");
                writer.CloseBrace();

                writer.WriteLine();
                writer.WriteLine("this(ref " + typeName + " value)");
                writer.OpenBrace();
                writer.WriteLine("super(value);");
                writer.CloseBrace();

                writer.WriteLine();
                writer.WriteLine("U opCast(U)()");
                writer.WriteLine("if(is(U:{0}))", typeName);
                writer.OpenBrace();
                writer.WriteLine("return __Value;");
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
                writer.WriteLine("return __traits(getMember, __Value, name)(args);");
                writer.CloseBrace();



                writer.WriteLine();
                writer.WriteLine("public override Type GetType()");
                writer.OpenBrace();
                writer.WriteLine("return __Value.GetType();");
                writer.CloseBrace();





                writer.CloseBrace();
            }
        }

        private static void WriteOutNestedTypes(Context.SyntaxAndSymbol first, OutputWriter writer)
        {
            //WriteOut All My nested classes
            Context.Push();

            var delegates = first.Syntax.DescendantNodes().OfType<DelegateDeclarationSyntax>()
                .Select(o => new
            {
                Syntax = o,
                Symbol = TypeProcessor.GetDeclaredSymbol(o),
                TypeName = WriteType.TypeName((INamedTypeSymbol)TypeProcessor.GetDeclaredSymbol(o))
            }).Where(k => k.Symbol.ContainingType == Context.Instance.Type) // Ignore all nested delegates
                .GroupBy(o => o.Symbol.ContainingNamespace.FullNameWithDot() + o.TypeName)
                .ToList();


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
                        Symbol = (INamedTypeSymbol)o.Symbol,
                        Syntax = o.Syntax
                    })
                            .Where(o => !Program.DoNotWrite.ContainsKey(o.Syntax))
                            .ToList()
                };

                if (Context.Instance.DelegatePartials.Count > 0)
                    WriteDelegate.Go(writer);
            });
            Context.Pop();

            Context.Push();
            var subclasses = first.Syntax.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
                .Select(o => new
            {
                Syntax = o,
                Symbol = TypeProcessor.GetDeclaredSymbol(o),
                TypeName = WriteType.TypeName((INamedTypeSymbol)TypeProcessor.GetDeclaredSymbol(o))
            }).Where(k => k.Symbol.ContainingType == Context.Instance.Type) // Ignore all nested classes
                .GroupBy(o => o.Symbol.ContainingNamespace.FullNameWithDot() + o.TypeName)
                .ToList();

            subclasses.ForEach(type => //.ForEach(type => //.Parallel(type =>
            {
                Context.Instance = new Context
                {
                    TypeName = type.First().TypeName,
                    Partials =
                        type.Select(
                            o =>
                                new Context.SyntaxAndSymbol
                    {
                        Symbol = (INamedTypeSymbol)o.Symbol,
                        Syntax = o.Syntax
                    })
                            .Where(o => !Program.DoNotWrite.ContainsKey(o.Syntax))
                            .ToList()
                };

                if (Context.Instance.Partials.Count > 0)
                {
                    try
                    {
                        WriteType.Go(writer);
                    }
                    catch (Exception ex)
                    {
                        //TODO: remove this when done with CorLib 
                        throw ex;
                    }
                }
            });

            Context.Pop();
        }

        public static OutputWriter EntryPoint = null;

        private static List<FieldDeclarationSyntax> SortFields(List<FieldDeclarationSyntax> fields)
        {
            if (fields.Count == 0)
                return fields;

            var dependencies =
                fields.ToDictionary(
                    o => TypeProcessor.GetDeclaredSymbol(o.Declaration.Variables.First()).As<IFieldSymbol>(),
                    o => new { Syntax = o, Dependicies = new List<IFieldSymbol>() });

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


        public static string TypeName(INamedTypeSymbol type, bool appendContainingTypeName = true, bool localize = false)
        {
            var sb = new StringBuilder();


            sb.Append(WriteIdentifierName.TransformIdentifier(type.Name, type));

            if (appendContainingTypeName)
            {
                while (type.ContainingType != null)
                {
                    type = type.ContainingType;
                    sb.Insert(0, WriteIdentifierName.TransformIdentifier(type.Name, type) +
                        (type.TypeArguments.Any()
                            ? (("!(" +
                                string.Join(",", (type.TypeArguments.Select(o => o.ToString())))) + ")")
                            : "") + ".");

                    //                sb.Append(string.Join("_", type.TypeArguments.Select(o => o)));
                }
            }

            return sb.ToString();
        }
    }
}