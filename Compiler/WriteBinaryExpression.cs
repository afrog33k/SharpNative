// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteBinaryExpression
    {
        public static void Go(OutputWriter writer, BinaryExpressionSyntax expression)
        {
            if (expression.OperatorToken.IsKind(SyntaxKind.AsKeyword))
            {
                var typeinfo = TypeProcessor.GetTypeInfo(expression.Right);

                var isPtr = typeinfo.Type != null && typeinfo.Type.IsValueType ? "" : "";
                writer.Write("cast( ");
                writer.Write(TypeProcessor.ConvertType(expression.Right) + isPtr);
                writer.Write(" )(");
                Core.Write(writer, expression.Left);
                writer.Write(")");
            }
            else if (expression.OperatorToken.IsKind(SyntaxKind.IsKeyword)) // isCast
            {
                var leftSymbolType = TypeProcessor.GetTypeInfo(expression.Left);
                var rightSymbolType = TypeProcessor.GetTypeInfo(expression.Right);

                if (leftSymbolType.Type.IsValueType)
                {
                    writer.Write("IsCast!(Boxed!(");
                    writer.Write(TypeProcessor.ConvertType(expression.Right));
                    writer.Write("))");
                    writer.Write("(");
                    Core.Write(writer, expression.Left);
                    writer.Write(")");

//						writer.Write("(cast(BOX!(");
//						writer.Write(TypeProcessor.ConvertType(expression.Right));
//						writer.Write("))(Boxed!(" + TypeProcessor.ConvertType(leftSymbolType.Type));
//						writer.Write(")(");
//						Core.Write(writer, expression.Left);
//						writer.Write(")) is! null)");
                    //Todo improve this ... though its silly to do this in the first place
//					writer.Write("(cast(BOX!(");
//                    writer.Write(TypeProcessor.ConvertType(expression.Right));
//					writer.Write("))(Boxed!( " + TypeProcessor.ConvertType(leftSymbolType.Type));
//					writer.Write(")(");
//                    Core.Write(writer, expression.Left);
//					writer.Write(")) is! null)");
                }
                else if (rightSymbolType.Type.IsValueType)
                {
                    writer.Write("IsCast!(Boxed!(");
                    writer.Write(TypeProcessor.ConvertType(expression.Right));
                    writer.Write("))");
                    writer.Write("(");
                    Core.Write(writer, expression.Left);
                    writer.Write(")");

//					writer.Write("(cast(Boxed!( ");
//                    writer.Write(TypeProcessor.ConvertType(expression.Right));
//					writer.Write(" ) )(");
//                    Core.Write(writer, expression.Left);
//					writer.Write(") is! null)");
                }
                else
                {
                    var typeinfo = TypeProcessor.GetTypeInfo(expression.Right);

                    var isPtr = typeinfo.Type != null && typeinfo.Type.IsValueType ? "" : "";
                    writer.Write("(IsCast!(");
                    writer.Write(TypeProcessor.ConvertType(expression.Right) + isPtr);
                    writer.Write(")(");
                    Core.Write(writer, expression.Left);
                    writer.Write("))");
                }
            }
            else if (expression.OperatorToken.IsKind(SyntaxKind.QuestionQuestionToken))
            {
                writer.Write("((");
                Core.Write(writer, expression.Left);
                writer.Write(")!is null?(");
                Core.Write(writer, expression.Left);
                writer.Write("):(");
                Core.Write(writer, expression.Right);
                writer.Write("))");
            }
            else
            {
                //                if (expression.Left is ElementAccessExpressionSyntax && IsAssignmentToken((SyntaxKind) expression.OperatorToken.RawKind))
                //                {
                //                    var subExpr = expression.Left.As<ElementAccessExpressionSyntax>();
                //                    var typeStr = TypeProcessor.GenericTypeName(TypeProcessor.GetTypeInfo(subExpr.Expression).Type);
                //                 
                //
                //             
                //                }

                Action<ExpressionSyntax> write = e =>
                {
                    var type = TypeProcessor.GetTypeInfo(e);

                    //Check for enums being converted to strings by string concatenation
                    var typeSymbol = type.Type ?? type.ConvertedType;

                    if (expression.OperatorToken.RawKind == (decimal) SyntaxKind.PlusToken &&
                        typeSymbol.TypeKind == TypeKind.Enum)
                    {
                        writer.Write(typeSymbol.ContainingNamespace.FullNameWithDot());
                        writer.Write(WriteType.TypeName(typeSymbol.As<INamedTypeSymbol>()));
                        writer.Write(".ToString(");
                        Core.Write(writer, e);
                        writer.Write(")");
                    }
                    else if (expression.OperatorToken.RawKind == (decimal) SyntaxKind.PlusToken &&
                             (typeSymbol.Name == "Nullable" && typeSymbol.ContainingNamespace.FullName() == "System" &&
                              typeSymbol.As<INamedTypeSymbol>().TypeArguments.Single().TypeKind == TypeKind.Enum))
                    {
                        var enumType = typeSymbol.As<INamedTypeSymbol>().TypeArguments.Single();
                        writer.Write(enumType.ContainingNamespace.FullNameWithDot());
                        writer.Write(WriteType.TypeName(enumType.As<INamedTypeSymbol>()));
                        writer.Write(".ToString(");
                        Core.Write(writer, e);
                        writer.Write(")");
                    }
                    else if (expression.OperatorToken.RawKind == (decimal) SyntaxKind.PlusToken &&
                             IsException(typeSymbol))
                        //Check for exceptions being converted to strings by string concatenation
                    {
                        //   writer.Write("System.SharpNative.ExceptionToString(");
                        Core.Write(writer, e);
                        //   writer.Write(")");
                    }
//                 
                    else
                        Core.Write(writer, e);
                };

                var symbolInfoLeft = TypeProcessor.GetSymbolInfo(expression.Left);
                var symbolInfoRight = TypeProcessor.GetSymbolInfo(expression.Right);

                var leftExpressionType = TypeProcessor.GetTypeInfo(expression.Left);
                var rightExpressionType = TypeProcessor.GetTypeInfo(expression.Right);

                var boxLeft = leftExpressionType.Type != null &&
                              (leftExpressionType.Type.IsValueType && (leftExpressionType.ConvertedType.IsReferenceType));

                var boxRight = rightExpressionType.ConvertedType != null &&
                               (rightExpressionType.Type != null && (rightExpressionType.Type.IsValueType &&
                                                                     (rightExpressionType.ConvertedType.IsReferenceType)));
                var derefRight = rightExpressionType.ConvertedType != null &&
                                 (leftExpressionType.ConvertedType != null &&
                                  !leftExpressionType.ConvertedType.IsReferenceType &&
                                  rightExpressionType.ConvertedType.IsReferenceType);

                //Property calls will be fixed in a preprocessor step ... i.e. just call them
                // var propertyLeft = symbolInfoLeft.Symbol != null && symbolInfoLeft.Symbol.Kind == SymbolKind.Property;
                // var propertyRight = symbolInfoRight.Symbol != null && symbolInfoRight.Symbol.Kind == SymbolKind.Property;

                //Do we have an implicit converter, if so, use it
                if (boxLeft || boxRight)
                {
                    if (boxLeft)
                    {
                        bool useType = true;

                        //We should start with exact converters and then move to more generic convertors i.e. base class or integers which are implicitly convertible
                        var correctConverter = leftExpressionType.Type.GetImplicitCoversionOp(leftExpressionType.Type,
                            rightExpressionType.Type);
                        //                            initializerType.Type.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);

                        if (correctConverter == null)
                        {
                            useType = false;
                            correctConverter =
                                rightExpressionType.Type.GetImplicitCoversionOp(leftExpressionType.Type,
                                    rightExpressionType.Type);
                                //.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);
                        }

                        if (correctConverter != null)
                        {
                            if (useType)
                            {
                                writer.Write(TypeProcessor.ConvertType(leftExpressionType.Type) + "." + "op_Implicit_" +
                                             TypeProcessor.ConvertType(correctConverter.ReturnType));
                            }
                            else
                            {
                                writer.Write(TypeProcessor.ConvertType(rightExpressionType.Type) + "." + "op_Implicit_" +
                                             TypeProcessor.ConvertType(correctConverter.ReturnType));
                            }
                            writer.Write("(");
                            Core.Write(writer, expression.Right);
                            writer.Write(")");
                            return;
                        }
                    }
//                    if (shouldBox)
//                    {
//                        bool useType = true;
//                        var correctConverter =
//                            initializerType.Type.GetCoversionOp(initializerType.ConvertedType, initializerType.Type);//.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.ConvertedType && h.Parameters[0].Type == initializerType.Type);
//
//                        if (correctConverter == null)
//                        {
//                            useType = false;
//                            correctConverter =
//                                initializerType.ConvertedType.GetCoversionOp(initializerType.ConvertedType,
//                                    initializerType.Type);
//                            //.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.ConvertedType && h.Parameters[0].Type == initializerType.Type);
//                        }
//
//                        if (correctConverter != null)
//                        {
//                            if (useType)
//                                writer.Write(TypeProcessor.ConvertType(initializerType.Type) + "." + "op_Implicit(");
//                            else
//                            {
//                                writer.Write(TypeProcessor.ConvertType(initializerType.ConvertedType) + "." + "op_Implicit(");
//
//                            }
//                            Core.Write(writer, value);
//                            writer.Write(")");
//                            return;
//                        }
//                    }
                }

                if (expression.OperatorToken.CSharpKind() == SyntaxKind.PlusEqualsToken ||
                    expression.OperatorToken.CSharpKind() == SyntaxKind.MinusEqualsToken)
                {
                    var isname = expression.Right is NameSyntax;

                    var nameexpression = expression.Right as NameSyntax;

                    var ismemberexpression = expression.Right is MemberAccessExpressionSyntax ||
                                             (isname &&
                                              TypeProcessor.GetSymbolInfo(expression.Right as NameSyntax).Symbol.Kind ==
                                              SymbolKind.Method);

                    var isdelegateassignment = ismemberexpression &&
                                               rightExpressionType.ConvertedType.TypeKind == TypeKind.Delegate;

                    var memberaccessexpression = expression.Right as MemberAccessExpressionSyntax;

                    var isstaticdelegate = isdelegateassignment &&
                                           ((memberaccessexpression != null &&
                                             TypeProcessor.GetSymbolInfo(memberaccessexpression).Symbol.IsStatic) ||
                                            (isname && TypeProcessor.GetSymbolInfo(nameexpression).Symbol.IsStatic));

                    if (isdelegateassignment)
                    {
                        write(expression.Left);

                        writer.Write(expression.OperatorToken.ToString());

                        var createNew = !(expression.Right is ObjectCreationExpressionSyntax);
                        var typeString = TypeProcessor.ConvertType(rightExpressionType.ConvertedType);

                        if (createNew)
                        {
                            if (rightExpressionType.ConvertedType.TypeKind == TypeKind.TypeParameter)
                                writer.Write(" __TypeNew!(" + typeString + ")(");
                            else
                                writer.Write("new " + typeString + "(");
                        }

                        var isStatic = isstaticdelegate;
                        if (isStatic)
                            writer.Write("__ToDelegate(");
                        writer.Write("&");

                        write(expression.Right);
                        //Core.Write (writer, expression.Right);
                        if (isStatic)
                            writer.Write(")");

                        if (createNew)
                            writer.Write(")");
                        return;
                    }
                }
                if (leftExpressionType.Type == null || rightExpressionType.Type == null)
                {
                    // seems we have a null here obj==null or null==obj
                    if ((rightExpressionType.Type != null && rightExpressionType.Type.IsValueType) ||
                        (leftExpressionType.Type != null && leftExpressionType.Type.IsValueType))
                    {
                        writer.Write("/*value type cannot be null*/");
                        write(expression.Left);
                        switch (expression.OperatorToken.CSharpKind())
                        {
                            case SyntaxKind.EqualsEqualsToken:
                                writer.Write("!=");
                                break;
                            case SyntaxKind.NotEqualsExpression:
                                writer.Write("==");
                                break;
                            default:
                                writer.Write(expression.OperatorToken.ToString());
                                break;
                        }

                        write(expression.Right);
                    }
                    else
                    {
                        write(expression.Left);
                        if (expression.OperatorToken.CSharpKind() == SyntaxKind.EqualsEqualsToken)
                            writer.Write(" is ");
                        else if (expression.OperatorToken.CSharpKind() == SyntaxKind.ExclamationEqualsToken)
                            writer.Write(" !is ");
                        else
                            writer.Write(expression.OperatorToken.ToString());

                        write(expression.Right);
                    }
                }
                else
//                if (symbolInfoLeft.Symbol != null && (symbolInfoLeft.Symbol.Kind == SymbolKind.Property && expression.OperatorToken.ValueText == "=")) //Assignment of property
//                {
//
//                    write(expression.Left);
//                    writer.Write("(");
//                    write(expression.Right);
//                    writer.Write(")");
//                }
//                else
                {
//                    writer.Write(derefLeft ? "*(" : "");
                    writer.Write(boxLeft ? "BOX!(" + TypeProcessor.ConvertType(leftExpressionType.Type) + ")(" : "");
                    write(expression.Left);

                    writer.Write(boxLeft ? ")" : "");
//                    writer.Write(derefLeft ? ")" : "");
                    writer.Write(expression.OperatorToken.ToString());
//                    writer.Write(derefRight ? "(" : "");
                    writer.Write(boxRight ? "BOX!(" + TypeProcessor.ConvertType(rightExpressionType.Type) + ")(" : "");
                    write(expression.Right);

                    writer.Write(boxRight ? ")" : "");
//                    writer.Write(derefRight ? ")" : "");
                }
            }
        }


        //TODO: Add full list here ...
        private static readonly string[] OverloadableOperators =
        {
            "+", "-", "!", "~", "++", "--", //true, false
            //These unary operators can be overloaded.

            "+", "-", "*", "/", "%", "&", "|", "^", "<<", ">>",

            //These binary operators can be overloaded.

            "==", "!=", "<", ">", "<=", ">=",

            //The comparison operators can be overloaded

            "&&", "||"
        };

        private static bool CouldBeNullString(SemanticModel model, ExpressionSyntax e)
        {
            while (true)
            {
                if (model.GetConstantValue(e).HasValue)
                    return false; //constants are never null

                //For in-line conditions, just recurse on both results.
                var cond = e as ConditionalExpressionSyntax;
                if (cond != null)
                    return CouldBeNullString(model, cond.WhenTrue) || CouldBeNullString(model, cond.WhenFalse);

                var paren = e as ParenthesizedExpressionSyntax;
                if (paren != null)
                {
                    e = paren.Expression;
                    continue;
                }

                var invoke = e as InvocationExpressionSyntax;

                if (invoke == null)
                    return true;

                var methodSymbol = ModelExtensions.GetSymbolInfo(model, invoke).Symbol;

                //Hard-code some well-known functions as an optimization
                if (methodSymbol.Name == "HtmlEncode" && methodSymbol.ContainingNamespace.FullName() == "System.Web")
                    return false;

                return methodSymbol.Name != "ToString";
            }
        }


        private static bool IsException(ITypeSymbol typeSymbol)
        {
            while (true)
            {
                if (typeSymbol.Name == "Exception" && typeSymbol.ContainingNamespace.FullName() == "System")
                    return true;

                if (typeSymbol.BaseType == null)
                    return false;
                typeSymbol = typeSymbol.BaseType;
            }
        }

        private static bool IsAssignmentToken(SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.EqualsToken:
                case SyntaxKind.PlusEqualsToken:
                case SyntaxKind.MinusEqualsToken:
                case SyntaxKind.SlashEqualsToken:
                case SyntaxKind.AsteriskEqualsToken:
                    return true;
                default:
                    return false;
            }
        }
    }
}