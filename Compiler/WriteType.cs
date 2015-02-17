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
            if (Driver.Verbose)
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

            OutputWriter writer = null;


            using (writer = outputWriter == null ? new OutputWriter(Context.Instance.Namespace, Context.Instance.TypeName) : new TempWriter())
            {
                if (outputWriter != null)
                {
                    writer.WriteLine();
                    writer.Indent = outputWriter.Indent + 2;
                    writer.WriteIndent();
                }

                var bases = partials
                    .Select(o => o.Syntax.BaseList)
                    .Where(o => o != null)
                    .SelectMany(o => o.Types)
                    .Select(o => TypeProcessor.GetTypeInfo(o.Type).ConvertedType)
                    .Distinct()
                    .ToList();

                //                var interfaces = bases.Where(o => o.TypeKind == TypeKind.Interface).ToList();

                if (Context.Instance.Type != Context.Object)
                    if (!bases.Any((j => j.TypeKind != TypeKind.Interface)) &&
                        !(first.Symbol.TypeKind == TypeKind.Interface || first.Symbol.TypeKind == TypeKind.Struct))
                        //TODO: fix structs using mixins / alias this
                        bases.Add(Context.Object);

                //                    WriteStandardIncludes.Go(writer);

                //                    writer.WriteLine(String.Format("#include \"{0}\"", TypeState.Instance.TypeName + ".h"));

                WriteBcl.Go(writer);

                //TypeState.Instance.DerivesFromObject = bases.Count == interfaces.Count;

                var @namespace = first.Symbol.ContainingNamespace.FullName();
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

                //TODO: Fix enum support
                if (first.Syntax is EnumDeclarationSyntax)
                {
                    WriteEnum.Go(writer,
                        Context.Instance.Partials.Select(o => o.Syntax)
                            .Cast<EnumDeclarationSyntax>()
                            .SelectMany(o => o.Members)
                            .Where(o => !Program.DoNotWrite.ContainsKey(o)));

                    if (outputWriter != null)
                    {
                        outputWriter.Write(writer.ToString());
                    }

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


                    //Internal classes/structs are declared static in D to behave correctly
                    if (outputWriter != null)
                    {
                        writer.Write("static ");
                    }
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
                    {
                        parentTypeParameters = Context.Instance.Type.ContainingType.TypeArguments.ToList();
                    }
                    else
                    {
                        parentTypeParameters = new List<ITypeSymbol>();
                    }
                    writer.Write(TypeName(Context.Instance.Type, false));

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
                                            {
                                                genericArgs.Add(arg);
                                            }
                                        }
                                    }
                                }
                            }
                            if (genericArgs.Any())
                            {
                                writer.Write("(" + string.Join(" , ", genericArgs.Select(o => o)) + ")");
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

                var nonFields = WriteFields(membersToWrite, first, writer);

                foreach (var member in nonFields)
                {
                    //                    writer.WriteLine();
                    Core.Write(writer, member);
                }

                WriteConstructors(instanceCtors, writer);

                WriteStaticConstructors(staticCtors, writer);

                //PInvoke is now centralized, so we can call it on all libraries etc without issue

                writer.Indent--;
                WriteOutNestedTypes(first, writer);

                WriteOutBoxed(writer, genericArgs, bases);


                if (Context.Instance.Type.TypeKind == TypeKind.Struct)
                {
                    writer.WriteLine("public __Boxed_ __Get_Boxed()");
                    writer.OpenBrace();
                    writer.WriteLine("return new __Boxed_(this);");
                    writer.CloseBrace();
                    writer.WriteLine("alias __Get_Boxed this;");
                }

                if (Context.Instance.Type.TypeKind != TypeKind.Interface)
                {
					if (Context.Instance.Type.TypeKind == TypeKind.Class)
						writer.WriteLine ("public override Type GetType()");
//					else if (Context.Instance.Type.TypeKind == TypeKind.Interface) // Messes with GetType overrides of objects
//					{
//						writer.WriteLine ("public final Type GetType()");
//					}
					else if (Context.Instance.Type.TypeKind == TypeKind.Struct)
					{
						writer.WriteLine ("public Type GetType()");

					}
                    writer.OpenBrace();
					if (Context.Instance.Type.TypeKind == TypeKind.Class)
                    	writer.WriteLine("return __TypeOf!(typeof(this));");
					else
						writer.WriteLine("return __TypeOf!(__Boxed_);");
						
                    writer.CloseBrace();
                }



                writer.CloseBrace();

                WriteEntryMethod(writer);

                if (outputWriter != null)
                {
                    outputWriter.Write(writer.ToString());
                }
            }
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

			if (constructorCount > 0 && (Context.Instance.Type.TypeKind == TypeKind.Struct)) // struct with constructors
			{
				writer.WriteLine ("static {0} opCall(U...)(U args_)",Context.Instance.TypeName);
				writer.OpenBrace ();
				writer.WriteLine (" {0} s;",Context.Instance.TypeName);
				writer.WriteLine ("s.__init(args_);");
				writer.WriteLine ("return s;");
				writer.CloseBrace();
			}
          
        }

        private static IEnumerable<MemberDeclarationSyntax> WriteFields(List<MemberDeclarationSyntax> membersToWrite, Context.SyntaxAndSymbol first, OutputWriter writer)
        {
            var fields = membersToWrite.OfType<FieldDeclarationSyntax>().ToList();
            var nonFields = membersToWrite.Except(fields); // also static fields should be separately dealt with

            if (fields.Count > 0 && fields.Any(j => j.GetModifiers().Any(SyntaxKind.ConstKeyword) ||
                                                    j.GetModifiers().Any(SyntaxKind.StaticKeyword)))
            {
                writer.WriteLine("enum __staticFieldTuple = __Tuple!(" +
                                 fields.Where(
                                     j =>
                                         j.GetModifiers().Any(SyntaxKind.ConstKeyword) ||
                                         j.GetModifiers().Any(SyntaxKind.StaticKeyword))
                                     .Select(
                                         k =>
                                             "\"" +
                                             WriteIdentifierName.TransformIdentifier(
                                                 k.Declaration.Variables[0].Identifier.ValueText) + "\"")
                                     .Aggregate((i, j) => i + "," + j) + ");//Reflection Support");
            }

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
                        foreach (var member in @group)
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
                var typeName = TypeName(Context.Instance.Type, false) +
                               (Context.Instance.Type.IsGenericType && genericArgs.Any()
                                   ? ("!(" + string.Join(" , ", genericArgs.Select(o => o)) + ")")
                                   : "");

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

                var ifacemembers = members.DistinctBy(k=>k);//Utility.GetAllMembers(Context.Instance.Type);

                    foreach (var member in ifacemembers)
                    {
                        var ifacemethod =
                            Context.Instance.Type.FindImplementationForInterfaceMember(member)
                                .DeclaringSyntaxReferences.First()
                                .GetSyntax();

                        var syntax = ifacemethod as MethodDeclarationSyntax;
                        if (syntax != null)
                            WriteMethod.WriteIt(writer, syntax);

                        var property = ifacemethod as PropertyDeclarationSyntax;
                        if (property != null)
                            WriteProperty.Go(writer, property, true);

                    }
                //}

                //This is required to be able to create an instance at runtime / reflection
                //					this()
                //					{
                //						super(SimpleStruct.init);
                //					}

                writer.WriteLine();
                writer.WriteLine("this()");
                writer.OpenBrace();
				writer.WriteLine("super(__TypeNew!({0})());", typeName);
                writer.CloseBrace();

                if (Context.Instance.Type.GetMembers("ToString").Any()) // Use better matching ?
                {
                    //					writer.WriteLine ();
                    writer.WriteLine("override String ToString()");
                    writer.OpenBrace();
                    writer.WriteLine("return __Value.ToString();");
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


        public static string TypeName(INamedTypeSymbol type, bool appendContainingTypeName = true)
        {
            var sb =
                new StringBuilder(string.Join("_",
                    (new[] { type.Name }).Union(type.TypeArguments.Select(o => o.ToString()))));

            if (appendContainingTypeName)
                while (type.ContainingType != null)
                {
                    type = type.ContainingType;
                    sb.Insert(0, type.Name + (type.TypeArguments.Any() ? (("_" + string.Join("_", type.TypeArguments.Select(o => o)) + "!(" +
                        string.Join(",", (type.TypeArguments.Select(o => o.ToString())))) + ")") : "") + ".");

                    //                sb.Append(string.Join("_", type.TypeArguments.Select(o => o)));
                }

            return sb.ToString();
        }
    }
}