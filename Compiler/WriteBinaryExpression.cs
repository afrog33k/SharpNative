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
            var leftExpression = expression.Left;
            var rightExpression = expression.Right;

            var operatorToken = expression.OperatorToken;
             
            WriteIt(writer, operatorToken, rightExpression, leftExpression);
        }

        public static void WriteIt(OutputWriter writer, SyntaxToken operatorToken, CSharpSyntaxNode rightExpression,
            CSharpSyntaxNode leftExpression)
        {
            if (operatorToken.IsKind(SyntaxKind.AsKeyword))
            {
                writer.Write("AsCast!(");
                writer.Write(TypeProcessor.ConvertType(rightExpression));
                writer.Write(")(");
                Core.Write(writer, leftExpression);
                writer.Write(")");
            }
            else if (operatorToken.IsKind(SyntaxKind.IsKeyword)) // isCast
            {
                var leftSymbolType = TypeProcessor.GetTypeInfo(leftExpression);
                var rightSymbolType = TypeProcessor.GetTypeInfo(rightExpression);

                if (leftSymbolType.Type.IsValueType)
                {
                    writer.Write("IsCast!(Boxed!(");
                    writer.Write(TypeProcessor.ConvertType(rightExpression));
                    writer.Write("))");
                    writer.Write("(");
                    Core.Write(writer, leftExpression);
                    writer.Write(")");
                }
                else if (rightSymbolType.Type.IsValueType)
                {
                    writer.Write("IsCast!(Boxed!(");
                    writer.Write(TypeProcessor.ConvertType(rightExpression));
                    writer.Write("))");
                    writer.Write("(");
                    Core.Write(writer, leftExpression);
                    writer.Write(")");
                }
                else
                {
                    writer.Write("(IsCast!(");
                    writer.Write(TypeProcessor.ConvertType(rightExpression));
                    writer.Write(")(");
                    Core.Write(writer, leftExpression);
                    writer.Write("))");
                }
            }
            else if (operatorToken.IsKind(SyntaxKind.QuestionQuestionToken))
            {
                writer.Write("((");
                Core.Write(writer, leftExpression);
                writer.Write(")!is null?(");
                Core.Write(writer, leftExpression);
                writer.Write("):(");
                Core.Write(writer, rightExpression);
                writer.Write("))");
            }
            else
            {
                Action<ExpressionSyntax> write = e => Core.Write(writer, e);

                TypeInfo leftExpressionType;
                if(leftExpression!=null)
                    leftExpressionType = TypeProcessor.GetTypeInfo(leftExpression);
                else
                {
                    leftExpressionType = TypeProcessor.GetTypeInfo(rightExpression);
                }

                var rightExpressionType = TypeProcessor.GetTypeInfo(rightExpression);

                var boxLeft = leftExpressionType.Type != null &&
                              (leftExpressionType.Type.IsValueType && (leftExpressionType.ConvertedType.IsReferenceType));

                var boxRight = rightExpressionType.ConvertedType != null &&
                               (rightExpressionType.Type != null && (rightExpressionType.Type.IsValueType &&
                                                                     (rightExpressionType.ConvertedType.IsReferenceType)));

                var rightnull = rightExpression.ToFullString().Trim() == "null";
                var leftnull = leftExpression != null && leftExpression.ToFullString().Trim() == "null";

                var nullAssignment =  (rightnull || leftnull);

                //Property calls will be fixed in a preprocessor step ... i.e. just call them
                if (nullAssignment)
                {
                    if (rightnull)
                    {
                        Core.Write(writer, leftExpression);

                        switch (operatorToken.CSharpKind())
                        {
                            case SyntaxKind.EqualsEqualsToken:
                                writer.Write(" is ");
                                break;
                            case SyntaxKind.NotEqualsExpression:
                            case SyntaxKind.ExclamationEqualsToken:
                                writer.Write(" !is ");
                                break;
                            default:
                                writer.Write(operatorToken.ToString());
                                break;
                        }

                        writer.Write("null");
                        return;
                    }

                    if (leftnull)
                    {
                        writer.Write("null");


                        switch (operatorToken.CSharpKind())
                        {
                            case SyntaxKind.EqualsEqualsToken:
                                writer.Write(" is ");
                                break;
                            case SyntaxKind.NotEqualsExpression:
                            case SyntaxKind.ExclamationEqualsToken:
                                writer.Write(" !is ");
                                break;
                            default:
                                writer.Write(operatorToken.ToString());
                                break;
                        }

                        Core.Write(writer, leftExpression);

                        return;
                    }
                }


                //Do we have an implicit converter, if so, use it
                if (leftExpressionType.Type != rightExpressionType.Type && rightExpressionType.Type != null)
                {
                    
                        bool useType = true;

                        //We should start with exact converters and then move to more generic convertors i.e. base class or integers which are implicitly convertible
                        var correctConverter = leftExpressionType.Type.GetImplicitCoversionOp(leftExpressionType.Type,
                            rightExpressionType.Type);

                        if (correctConverter == null)
                        {
                            useType = false;
                            correctConverter =
                                rightExpressionType.Type.GetImplicitCoversionOp(leftExpressionType.Type,
                                    rightExpressionType.Type);
                        }

                        if (correctConverter != null)
                        {
                        Core.Write(writer, leftExpression);
                        writer.Write(operatorToken.ToString());
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
                            Core.Write(writer, rightExpression);
                            writer.Write(")");
                            return;
                        }
                    
                }

                if (operatorToken.CSharpKind() == SyntaxKind.PlusEqualsToken ||
                    operatorToken.CSharpKind() == SyntaxKind.MinusEqualsToken)
                {
                    var isname = rightExpression is NameSyntax;

                    var nameexpression = rightExpression as NameSyntax;

                    var ismemberexpression = rightExpression is MemberAccessExpressionSyntax ||
                                             (isname &&
                                              TypeProcessor.GetSymbolInfo(rightExpression as NameSyntax).Symbol.Kind ==
                                              SymbolKind.Method);

                    var isdelegateassignment = rightExpressionType.ConvertedType != null && (ismemberexpression &&
                                                                                             rightExpressionType.ConvertedType
                                                                                                 .TypeKind == TypeKind.Delegate);

                    var memberaccessexpression = rightExpression as MemberAccessExpressionSyntax;

                    var isstaticdelegate = isdelegateassignment &&
                                           ((memberaccessexpression != null &&
                                             TypeProcessor.GetSymbolInfo(memberaccessexpression).Symbol.IsStatic) ||
                                            (isname && TypeProcessor.GetSymbolInfo(nameexpression).Symbol.IsStatic));

                    if (isdelegateassignment)
                    {
                        Core.Write(writer,leftExpression);

                        writer.Write(operatorToken.ToString());

                        
                        var typeString = TypeProcessor.ConvertType(rightExpressionType.ConvertedType);

                        if (rightExpressionType.ConvertedType.TypeKind == TypeKind.TypeParameter)
                            writer.Write(" __TypeNew!(" + typeString + ")(");
                        else
                            writer.Write("new " + typeString + "(");

                        var isStatic = isstaticdelegate;
                        if (isStatic)
                            writer.Write("__ToDelegate(");

                        writer.Write("&");

                        Core.Write(writer,rightExpression);
                        if (isStatic)
                            writer.Write(")");

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
                        Core.Write(writer, leftExpression);
                        switch (operatorToken.CSharpKind())
                        {
                            case SyntaxKind.EqualsEqualsToken:
                                writer.Write("!=");
                                break;
                            case SyntaxKind.NotEqualsExpression:
                                writer.Write("==");
                                break;
                            default:
                                writer.Write(operatorToken.ToString());
                                break;
                        }

                        Core.Write(writer, rightExpression);
                    }
                    else
                    {
                        Core.Write(writer, leftExpression);
                        if (operatorToken.CSharpKind() == SyntaxKind.EqualsEqualsToken)
                            writer.Write(" is ");
                        else if (operatorToken.CSharpKind() == SyntaxKind.ExclamationEqualsToken)
                            writer.Write(" !is ");
                        else
                            writer.Write(operatorToken.ToString());

                        Core.Write(writer, rightExpression);
                    }
                }
                else
                {
                    writer.Write(boxLeft ? "BOX!(" + TypeProcessor.ConvertType(leftExpressionType.Type) + ")(" : "");
                    Core.Write(writer, leftExpression);
                    writer.Write(boxLeft ? ")" : "");
                    writer.Write(operatorToken.ToString());
                    writer.Write(boxRight ? "BOX!(" + TypeProcessor.ConvertType(rightExpressionType.Type) + ")(" : "");
                    Core.Write(writer, rightExpression);
                    writer.Write(boxRight ? ")" : "");
                }
            }
        }
    }
}