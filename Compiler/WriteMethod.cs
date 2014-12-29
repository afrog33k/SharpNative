// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteMethod
    {
        public static string GetParameterListAsString(ParameterListSyntax list, bool includeTypes = true)
        {
            var writer = new TempWriter(); // Temp Writer
            writer.Write("(");
            var firstParam = true;
            foreach (var parameter in list.Parameters)
            {
                var refKeyword = "";

                if (parameter.Modifiers.Any(SyntaxKind.RefKeyword))
                    refKeyword = " ref ";

                if (parameter.Modifiers.Any(SyntaxKind.OutKeyword))
                    refKeyword = " out ";

                if (parameter.Modifiers.Any(SyntaxKind.InKeyword))
                    refKeyword = " in ";

                bool isRef = parameter.Modifiers.Any(SyntaxKind.OutKeyword) ||
                             parameter.Modifiers.Any(SyntaxKind.RefKeyword);
                    // || parameter.Modifiers.Any (SyntaxKind.InKeyword);
                if (firstParam)
                    firstParam = false;
                else
                    writer.Write(", ");

                var s = refKeyword + TypeProcessor.ConvertType(parameter.Type) + " ";
                if (includeTypes)
                    writer.Write(s);

                writer.Write(WriteIdentifierName.TransformIdentifier(parameter.Identifier.ValueText));
                if (parameter.Default == null)
                    continue;
                writer.Write(" = ");
                Core.Write(writer, parameter.Default.Value);
            }
            writer.Write(")");
            return writer.ToString();
        }

      

        public static void WriteIt(OutputWriter writer, MethodDeclarationSyntax method, bool isProxy = true)
        {
            writer.WriteLine();
            var methodSymbol = (IMethodSymbol) TypeProcessor.GetDeclaredSymbol(method);
        
          

            var pinvokeAttributes = method.GetAttribute(Context.DllImport);
            var isInternalPInvoke = pinvokeAttributes == null && method.Modifiers.Any(SyntaxKind.ExternKeyword);

            //TODO: Improve partial class / method support -- partials classes work, methods need minor work ...
            if (method.Modifiers.Any(SyntaxKind.PartialKeyword) && method.Body == null)
            {
                //We only want to render out one of the two partial methods.  If there's another, skip this one.
                if (Context.Instance.Partials.SelectMany(o => o.Syntax.As<ClassDeclarationSyntax>().Members)
                    .OfType<MethodDeclarationSyntax>()
                    .Except(method).Any(o => o.Identifier.ValueText == method.Identifier.ValueText))
                    return;
            }

          

            var accessString = "";

          

            var isInterface = method.Parent is InterfaceDeclarationSyntax;

            var methodName = MemberUtilities.GetMethodName(method, ref isInterface); //
            if (methodName == "Main" /*&& method.Modifiers.Any(SyntaxKind.PublicKeyword)*/&&
                method.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                accessString = ("public ");

                accessString += ("static ");

                var methodCall = methodSymbol.ContainingNamespace.FullName() + "." +
                                 methodSymbol.ContainingType.FullName() +
                                 "." + methodName + (method.ParameterList.Parameters.Count < 1 ? "();" : "(null);");
                    //: "(new Array_T!(String)(args));"); // for now args not supported
                Context.Instance.EntryMethod = methodCall;
            }

            else
            {
                accessString = MemberUtilities.GetAccessModifiers(method,isInterface);
            }

            //	        if (isInterface)
            //	        {
            //                writer.IsInterface = true;
            //	        }

            var returnTypeString = TypeProcessor.ConvertType(method.ReturnType,true) + " ";
            var methodSignatureString = "";
            if (method.ReturnType.ToString() == "void")
                returnTypeString = ("void ");
           
            methodSignatureString += methodName;

            if (method.TypeParameterList != null)
            {
                var genericArgs = method.TypeParameterList.Parameters.ToList();

                //				if (genericArgs.Count > 0)
                //				{
                //					writer.Write("( ");
                //					writer.Write(string.Join(" , ", genericArgs.Select(o => " " + o)));
                //					writer.Write(" )\r\n");
                //				}

                if (genericArgs.Count > 0) // && !methodSymbol.ContainingType.IsGenericType) // doesnt matter
                {
                    methodSignatureString += ("(");
                    methodSignatureString += (string.Join(",", genericArgs.Select(o => " " + o)));
                    methodSignatureString += (")");
                }
            }

            var @params = GetParameterListAsString(method.ParameterList);

            string constraints = GetMethodConstraints(method);

            if (isInterface || method.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                writer.WriteLine(accessString + returnTypeString + methodSignatureString + @params + constraints + ";");

                return;
            }

            writer.WriteLine(accessString + returnTypeString + methodSignatureString + @params + constraints);

            writer.OpenBrace();

            if (isProxy)
            {
                @params = GetParameterListAsString(method.ParameterList, false);

                if (method.ReturnType.ToString() == "void")
                    writer.WriteLine("Value." + methodName + @params + ";");
                else
                    writer.WriteLine("return Value." + methodName + @params + ";");
            }
            else
            {
                if (method.Body != null)
                {
                    foreach (var statement in method.Body.Statements)
                        Core.Write(writer, statement);

                    TriviaProcessor.ProcessTrivias(writer, method.Body.DescendantTrivia());
                }

                if (pinvokeAttributes != null)
                    WritePInvokeMethodBody.Go(writer, methodName, methodSymbol, pinvokeAttributes);

                if(isInternalPInvoke)
                    WritePInvokeMethodBody.Go(writer, methodName, methodSymbol, null);

            }

            writer.CloseBrace();
        }

        private static string GetMethodConstraints(MethodDeclarationSyntax method)
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
                        string dlangCondition = condition.ToString();

                        if (dlangCondition == "new()")
                            continue;
                        if (dlangCondition == "class") // TODO: is there a better way to do this ?
                            dlangCondition = "NObject";

                        if (dlangCondition == "struct")
                            constraints += ((isFirst ? "" : "&&") + " !is(" + constraint.Name + " : NObject)");
                        else
                        {
                            constraints += ((isFirst ? "" : "&&") + " is(" + constraint.Name + " : " + dlangCondition +
                                            ")");
                        }

                        isFirst = false;

                        //								Console.WriteLine (condition);
                    }
                }

                constraints += (")");
            }
            return constraints;
        }


        public static void Go(OutputWriter writer, MethodDeclarationSyntax method)
        {
            WriteIt(writer, method, false);
        }
    }
}