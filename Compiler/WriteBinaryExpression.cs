// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
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

                ProcessExpression(writer, operatorToken, rightExpression, leftExpression);
            }
        }

        private static void ProcessExpression(OutputWriter writer, SyntaxToken operatorToken, CSharpSyntaxNode rightExpression,
            CSharpSyntaxNode leftExpression)
        {
            TypeInfo leftExpressionType = TypeProcessor.GetTypeInfo(leftExpression ?? rightExpression);

            var rightExpressionType = TypeProcessor.GetTypeInfo(rightExpression);

            var boxLeft = leftExpressionType.Type != null &&  (leftExpressionType.Type != leftExpressionType.ConvertedType) &&
                          ((leftExpressionType.Type.IsValueType || leftExpressionType.Type.TypeKind == TypeKind.TypeParameter) &&
                           (leftExpressionType.ConvertedType.IsReferenceType));

            var boxRight = (rightExpressionType.ConvertedType != null &&
                            (rightExpressionType.Type != null &&
                             ((rightExpressionType.Type.IsValueType ||
                               rightExpressionType.Type.TypeKind == TypeKind.TypeParameter) &&
                              (rightExpressionType.ConvertedType.IsReferenceType))))
                           ||
                           (rightExpressionType.Type != null && rightExpressionType.Type.IsValueType &&
                            leftExpressionType.Type != null && leftExpressionType.Type.TypeKind == TypeKind.Error);
                //Fix for yield ... why does it give errortypes ?

            var unboxRight = rightExpressionType.ConvertedType != null &&
                             (rightExpressionType.Type != null && (rightExpressionType.Type.IsReferenceType &&
                                                                   (rightExpressionType.ConvertedType.IsValueType)));

            var rightnull = rightExpression!=null && rightExpression.ToFullString().Trim() == "null";
            var leftnull = leftExpression != null && leftExpression.ToFullString().Trim() == "null";

            var nullAssignment = (rightnull || leftnull);

            var val = WriteOperatorDeclaration.AllBinaryOperators.FirstOrDefault(k => k.Value == operatorToken.Text);
            //Matching Binary Operator Overload
            if (!String.IsNullOrEmpty(val.Value))
            {
                //Try Left
                IEnumerable<ISymbol> members = new List<ISymbol>();
                if (leftExpressionType.Type != null)
                    members = leftExpressionType.Type.GetMembers(val.Key);
                if (rightExpressionType.Type != null)
                {
                    members = members.
                        Union(rightExpressionType.Type.GetMembers(val.Key));
                }

                var leftExpressionString = Core.WriteString(leftExpression);
                if (members != null && members.Any())
                {
                    if (!(leftExpressionType.Type.IsPrimitive() && rightExpressionType.Type.IsPrimitive()))
                    {
                        var correctOverload =
                            members.OfType<IMethodSymbol>()
                                .FirstOrDefault(
                                    u =>
                                        u.Parameters[0].Type == leftExpressionType.Type &&
                                        u.Parameters[1].Type == rightExpressionType.Type);

                        if (correctOverload != null)
                        {
                            var name =
                                WriteIdentifierName.TransformIdentifier(OverloadResolver.MethodName(correctOverload));
                            writer.Write(TypeProcessor.ConvertType(correctOverload.ContainingType) + "." + name +
                                         "(" +
                                         leftExpressionString + "," + Core.WriteString(rightExpression)
                                         + ")");
                            return;
                        }
                    }
                }
                else
                {
                    if (WriteOperatorDeclaration.AssignOpOperators.ContainsKey(val.Key))
                    {
                        var methodName =
                            WriteOperatorDeclaration.AllBinaryOperators.FirstOrDefault(
                                k => k.Value == val.Value.Substring(0, 1));
                        // emulate c# facility to use the lower op ...
                        //Try Left
                        members = null;
                        if (leftExpressionType.Type != null)
                            members = leftExpressionType.Type.GetMembers(methodName.Key);
                        if (rightExpressionType.Type != null)
                        {
                            members = members.
                                Union(rightExpressionType.Type.GetMembers(methodName.Key));
                        }

                        if (members != null && members.Any())
                        {
                            if (!(leftExpressionType.Type.IsPrimitive() && rightExpressionType.Type.IsPrimitive()))
                            {
                                var correctOverload =
                                    members.OfType<IMethodSymbol>()
                                        .FirstOrDefault(
                                            u =>
                                                u.Parameters[0].Type == leftExpressionType.Type &&
                                                u.Parameters[1].Type == rightExpressionType.Type);

                                if (correctOverload != null)
                                {
                                    var name =
                                        WriteIdentifierName.TransformIdentifier(
                                            OverloadResolver.MethodName(correctOverload));
                                    writer.Write(leftExpressionString + " = " +
                                                 TypeProcessor.ConvertType(correctOverload.ContainingType) + "." +
                                                 name +
                                                 "(" +
                                                 leftExpressionString + "," + Core.WriteString(rightExpression)
                                                 + ")");
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            //Property calls will be fixed in a preprocessor step ... i.e. just call them
            if (nullAssignment)
            {
                if (rightnull)
                {

                    switch (operatorToken.Kind())
                    {
                        case SyntaxKind.EqualsEqualsToken:
                            writer.Write("");
                            break;
                        case SyntaxKind.NotEqualsExpression:
                        case SyntaxKind.ExclamationEqualsToken:
                            writer.Write("!");
                            break;
                        default:
                            Core.Write(writer, leftExpression);
                            writer.Write(operatorToken.ToString());
                            writer.Write("null");
                            return;
                            break;
                    }

                   
                        writer.Write("__IsNull(");
                        Core.Write(writer, leftExpression ?? SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
                        writer.Write(")");
                    
                    return;
                }

                if (leftnull)
                {
                    //                    writer.Write("null");
                    //
                    //                    switch (operatorToken.CSharpKind())
                    //                    {
                    //                        case SyntaxKind.EqualsEqualsToken:
                    //                            writer.Write(" is ");
                    //                            break;
                    //                        case SyntaxKind.NotEqualsExpression:
                    //                        case SyntaxKind.ExclamationEqualsToken:
                    //                            writer.Write(" !is ");
                    //                            break;
                    //                        default:
                    //                            writer.Write(operatorToken.ToString());
                    //                            break;
                    //                    }
                    //
                    //                    Core.Write(writer, rightExpression);
                    //
                    //                    return;
                    switch (operatorToken.Kind())
                    {
                        case SyntaxKind.EqualsEqualsToken:
                            writer.Write("");
                            break;
                        case SyntaxKind.NotEqualsExpression:
                        case SyntaxKind.ExclamationEqualsToken:
                            writer.Write("!");
                            break;
                        default:
                            writer.Write("null");
                            if (!operatorToken.IsKind(SyntaxKind.None))
                            {
                                writer.Write(operatorToken.ToString());
                                Core.Write(writer, rightExpression);
                            }
                            return;
                            break;
                    }

                   
                        writer.Write("__IsNull(");
                        Core.Write(writer, rightExpression ?? SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
                        writer.Write(")");
                    
                    return;
                }
            }

            //Do we have an implicit converter, if so, use it
            if (leftExpressionType.Type != rightExpressionType.Type && rightExpressionType.Type != null)
            {
                bool useType = true;

                //We should start with exact converters and then move to more generic convertors i.e. base class or integers which are implicitly convertible
                var correctConverter = leftExpressionType.Type.GetImplicitCoversionOp(leftExpressionType.Type,
                    rightExpressionType.Type, true);

                if (correctConverter == null)
                {
                    useType = false;
                    correctConverter =
                        rightExpressionType.Type.GetImplicitCoversionOp(leftExpressionType.Type,
                            rightExpressionType.Type, true);
                }

                if (correctConverter != null)
                {
                    Core.Write(writer, leftExpression);
                    writer.Write(operatorToken.ToString());
                    if (useType)
                    {
                        writer.Write(TypeProcessor.ConvertType(leftExpressionType.Type) + "." + "op_Implicit_" +
                                     TypeProcessor.ConvertType(correctConverter.ReturnType, false, true, false).Replace(".", "_"));
                    }
                    else
                    {
                        writer.Write(TypeProcessor.ConvertType(rightExpressionType.Type) + "." + "op_Implicit_" +
                                     TypeProcessor.ConvertType(correctConverter.ReturnType, false, true, false).Replace(".", "_"));
                    }
                    writer.Write("(");
                    Core.Write(writer, rightExpression);
                    writer.Write(")");
                    return;
                }
            }

            if (operatorToken.Kind() == SyntaxKind.PlusEqualsToken ||
                operatorToken.Kind() == SyntaxKind.MinusEqualsToken)
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
                    Core.Write(writer, leftExpression);

                    writer.Write(operatorToken.ToString());

                    var typeString = TypeProcessor.ConvertType(rightExpressionType.ConvertedType);

                    if (rightExpressionType.ConvertedType.TypeKind == TypeKind.TypeParameter)
                        writer.Write(" __TypeNew!(" + typeString + ")(");
                    else
                        writer.Write("new " + typeString + "(");

                    var isStatic = isstaticdelegate;
                    //                        if (isStatic)
                    //                            writer.Write("__ToDelegate(");

                    MemberUtilities.WriteMethodPointer(writer, rightExpression);
                    //                        if (isStatic)
                    //                            writer.Write(")");

                    writer.Write(")");
                    return;
                }
            }
            if (leftExpressionType.Type == null || (rightExpressionType.Type == null && rightExpression!=null))
            {
                // seems we have a null here obj==null or null==obj
                if ((rightExpressionType.Type != null && rightExpressionType.Type.IsValueType) ||
                    (leftExpressionType.Type != null && leftExpressionType.Type.IsValueType))
                {
                    writer.Write("/*value type cannot be null*/");
                    Core.Write(writer, leftExpression);
                    switch (operatorToken.Kind())
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
                    if (operatorToken.IsKind(SyntaxKind.EqualsEqualsToken))
                        writer.Write(" is ");
                    else if (operatorToken.IsKind(SyntaxKind.ExclamationEqualsToken))
                        writer.Write(" !is ");
                    else
                        writer.Write(operatorToken.ToString());
                    if (rightExpression != null)
                        Core.Write(writer, rightExpression);
                }
            }
            else
            {
                writer.Write(boxLeft ? "BOX!(" + TypeProcessor.ConvertType(leftExpressionType.Type) + ")(" : "");
                Core.Write(writer, leftExpression);
                writer.Write(boxLeft ? ")" : "");
                writer.Write(operatorToken.ToString());
                if (rightExpression != null)
                {
                    writer.Write(unboxRight
                        ? "UNBOX!(" + TypeProcessor.ConvertType(rightExpressionType.ConvertedType) + ")("
                        : "");
                    writer.Write(boxRight ? "BOX!(" + TypeProcessor.ConvertType(rightExpressionType.Type) + ")(" : "");
                    Core.Write(writer, rightExpression);
                    writer.Write(boxRight ? ")" : "");
                    writer.Write(unboxRight ? ")" : "");
                }
            }
        }
    }
}