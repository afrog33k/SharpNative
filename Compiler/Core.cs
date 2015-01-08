// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class Core
    {
        public static void Write(OutputWriter writer, SyntaxNode node, bool isConst = false)
        {
            Context.LastNode = node; //Helps with debugging
            
            //Write Leading Trivia
            TriviaProcessor.WriteLeadingTrivia(writer, node);

            TriviaProcessor.ProcessNode(writer, node);

            if (Program.DoNotWrite.ContainsKey(node))
                return;

            Factory(writer, node, isConst);

            TriviaProcessor.WriteTrailingTrivia(writer, node);
        }

        public static string WriteString(SyntaxNode node, bool isConst = false, int indent = 0)
        {
            var writer = new TempWriter();
            writer.Indent = indent;

            Write(writer,node,isConst);

            return writer.ToString();
        }


        private static void Factory(OutputWriter writer, SyntaxNode node, bool isConst)
        {
            if (node is ConstructorInitializerSyntax)
                WriteConstructorInitializer.Go(writer, node.As<ConstructorInitializerSyntax>());
            else if (node is CheckedExpressionSyntax)
                WriteChecked.Go(writer, node.As<CheckedExpressionSyntax>());
            else if (node is CheckedStatementSyntax)
                WriteChecked.Go(writer, node.As<CheckedStatementSyntax>());
            else if (node is UnsafeStatementSyntax)
                WriteUnsafeStatement.Go(writer, node.As<UnsafeStatementSyntax>());
            else if (node is InitializerExpressionSyntax)
                WriteInitializer.Go(writer, node.As<InitializerExpressionSyntax>());
            else if (node is GotoStatementSyntax)
                WriteGoto.Go(writer, node.As<GotoStatementSyntax>());
            else if (node is CaseSwitchLabelSyntax)
                WriteLabel.Go(writer, node.As<CaseSwitchLabelSyntax>());
            else if (node is LabeledStatementSyntax)
                WriteLabel.Go(writer, node.As<LabeledStatementSyntax>());
            else if (node is OperatorDeclarationSyntax)
                WriteOperatorDeclaration.Go(writer, node.As<OperatorDeclarationSyntax>());
            else if (node is MethodDeclarationSyntax)
                WriteMethod.Go(writer, node.As<MethodDeclarationSyntax>());
            else if (node is PropertyDeclarationSyntax)
                WriteProperty.Go(writer, node.As<PropertyDeclarationSyntax>());
            else if (node is EventDeclarationSyntax)
                WriteEvent.Go(writer, node.As<EventDeclarationSyntax>());
            else if (node is FieldDeclarationSyntax)
                WriteField.Go(writer, node.As<FieldDeclarationSyntax>());
            else if (node is EventFieldDeclarationSyntax)
                WriteField.Go(writer, node.As<EventFieldDeclarationSyntax>());
            else if (node is ConstructorDeclarationSyntax)
                WriteConstructorBody.Go(writer, node.As<ConstructorDeclarationSyntax>());
            else if (node is ExpressionStatementSyntax)
                WriteStatement(writer, node.As<ExpressionStatementSyntax>());
            else if (node is FixedStatementSyntax)
                WriteFixedStatement(writer, node.As<FixedStatementSyntax>());
            else if (node is LocalDeclarationStatementSyntax)
                WriteLocalDeclaration.Go(writer, node.As<LocalDeclarationStatementSyntax>());
            else if (node is VariableDeclarationSyntax)
                WriteVariableDeclaration.Go(writer, node.As<VariableDeclarationSyntax>());
            else if (node is BlockSyntax)
                WriteBlock(writer, node.As<BlockSyntax>());
            else if (node is InvocationExpressionSyntax)
                WriteInvocationExpression.Go(writer, node.As<InvocationExpressionSyntax>());
            else if (node is LiteralExpressionSyntax)
                WriteLiteralExpression.Go(writer, node.As<LiteralExpressionSyntax>(), isConst);
            else if (node is IdentifierNameSyntax)
                WriteIdentifierName.Go(writer, node.As<IdentifierNameSyntax>());
            else if (node is ImplicitArrayCreationExpressionSyntax)
                WriteArrayCreationExpression.Go(writer, node.As<ImplicitArrayCreationExpressionSyntax>());
            else if (node is ArrayCreationExpressionSyntax)
                WriteArrayCreationExpression.Go(writer, node.As<ArrayCreationExpressionSyntax>());
            else if (node is MemberAccessExpressionSyntax)
                WriteMemberAccessExpression.Go(writer, node.As<MemberAccessExpressionSyntax>());
            else if (node is ParenthesizedLambdaExpressionSyntax)
                WriteLambdaExpression.Go(writer, node.As<ParenthesizedLambdaExpressionSyntax>());
            else if (node is SimpleLambdaExpressionSyntax)
                WriteLambdaExpression.Go(writer, node.As<SimpleLambdaExpressionSyntax>());
            else if (node is AnonymousMethodExpressionSyntax)
                WriteLambdaExpression.Go(writer, node.As<AnonymousMethodExpressionSyntax>());
            else if (node is ReturnStatementSyntax)
                WriteReturnStatement.Go(writer, node.As<ReturnStatementSyntax>());
            else if (node is ObjectCreationExpressionSyntax)
                WriteObjectCreationExpression.Go(writer, node.As<ObjectCreationExpressionSyntax>());
            else if (node is ElementAccessExpressionSyntax)
                WriteElementAccessExpression.Go(writer, node.As<ElementAccessExpressionSyntax>());
            else if (node is ForEachStatementSyntax)
                WriteForEachStatement.Go(writer, node.As<ForEachStatementSyntax>());
            else if (node is IfStatementSyntax)
                WriteIfStatement.Go(writer, node.As<IfStatementSyntax>());
            else if (node is BinaryExpressionSyntax)
                WriteBinaryExpression.Go(writer, node.As<BinaryExpressionSyntax>());
            else if (node is AssignmentExpressionSyntax)
                WriteAssignmentExpression.Go(writer, node.As<AssignmentExpressionSyntax>());
            else if (node is ConditionalExpressionSyntax)
                WriteConditionalExpression.Go(writer, node.As<ConditionalExpressionSyntax>());
            else if (node is BaseExpressionSyntax)
                WriteBaseExpression.Go(writer, node.As<BaseExpressionSyntax>());
            else if (node is ThisExpressionSyntax)
                WriteThisExpression.Go(writer, node.As<ThisExpressionSyntax>());
            else if (node is CastExpressionSyntax)
                WriteCastExpression.Go(writer, node.As<CastExpressionSyntax>());
            else if (node is ThrowStatementSyntax)
                WriteThrowStatement.Go(writer, node.As<ThrowStatementSyntax>());
            else if (node is EqualsValueClauseSyntax)
                WriteEqualsValueClause.Go(writer, node.As<EqualsValueClauseSyntax>());
            else if (node is ForStatementSyntax)
                WriteForStatement.Go(writer, node.As<ForStatementSyntax>());
            else if (node is WhileStatementSyntax)
                WriteWhileStatement.Go(writer, node.As<WhileStatementSyntax>());
            else if (node is BreakStatementSyntax)
                WriteBreakStatement.Go(writer, node.As<BreakStatementSyntax>());
            else if (node is ContinueStatementSyntax)
                WriteContinueStatement.Go(writer, node.As<ContinueStatementSyntax>());
            else if (node is DoStatementSyntax)
                WriteDoStatement.Go(writer, node.As<DoStatementSyntax>());
            else if (node is SwitchStatementSyntax)
                WriteSwitchStatement.Go(writer, node.As<SwitchStatementSyntax>());
            else if (node is TryStatementSyntax)
                WriteTryStatement.Go(writer, node.As<TryStatementSyntax>());
            else if (node is UsingStatementSyntax)
                WriteUsingStatement.Go(writer, node.As<UsingStatementSyntax>());
            else if (node is ParenthesizedExpressionSyntax)
                WriteParenthesizedExpression.Go(writer, node.As<ParenthesizedExpressionSyntax>());
            else if (node is LockStatementSyntax)
                WriteLockStatement.Go(writer, node.As<LockStatementSyntax>());
            else if (node is TypeOfExpressionSyntax)
                WriteTypeOfExpression.Go(writer, node.As<TypeOfExpressionSyntax>());
            else if (node is AnonymousObjectCreationExpressionSyntax)
                WriteAnonymousObjectCreationExpression.Go(writer, node.As<AnonymousObjectCreationExpressionSyntax>());
            else if (node is EmptyStatementSyntax)
                return; //ignore empty statements
            else if (node is DelegateDeclarationSyntax)
                return; //don't write delegates - TypeProcessor converts them to function types directly
            else if (node is DefaultExpressionSyntax)
                WriteDefaultExpression.Go(writer, node.As<DefaultExpressionSyntax>());
            else if (node is GenericNameSyntax)
                WriteGenericName.Go(writer, node.As<GenericNameSyntax>());
            else if (node is ConversionOperatorDeclarationSyntax)
                WriteConversionOperatorDeclaration.Go(writer, node.As<ConversionOperatorDeclarationSyntax>());
            else if (node is PrefixUnaryExpressionSyntax)
                WriteUnaryExpression.WritePrefix(writer, node.As<PrefixUnaryExpressionSyntax>());
            else if (node is PostfixUnaryExpressionSyntax)
                WriteUnaryExpression.WritePostfix(writer, node.As<PostfixUnaryExpressionSyntax>());
            else if (node is SizeOfExpressionSyntax)
                WriteSizeOfExpression.Go(writer, node.As<SizeOfExpressionSyntax>());
            else if (node is DestructorDeclarationSyntax)
                WriteDestructorBody.WriteDestructor(writer, node.As<DestructorDeclarationSyntax>());
            else if (node is IndexerDeclarationSyntax)
                WriteIndexer.Go(writer, node.As<IndexerDeclarationSyntax>());
            else if (node is StackAllocArrayCreationExpressionSyntax)
                writer.Write(node.ToFullString() + "//TODO: StackAlloc not supported yet");
            else
                throw new NotImplementedException(node.GetType().Name + " is not supported. " + Utility.Descriptor(node));
        }

        public static void WriteStatement(OutputWriter writer, ExpressionStatementSyntax statement)
        {
            writer.WriteIndent();
            Write(writer, statement.Expression);
            writer.Write(";\r\n");
        }

        public static void WriteFixedStatement(OutputWriter writer, FixedStatementSyntax statement)
        {
//			writer.WriteIndent();
            writer.WriteLine("//fixed() Scope");
            writer.OpenBrace();
            Write(writer, statement.Declaration);
            Write(writer, statement.Statement);
            writer.CloseBrace();
//			writer.Write(";\r\n");
        }


        public static void WriteBlock(OutputWriter writer, BlockSyntax block, bool writeBraces = true)
        {
            if (writeBraces)
                writer.OpenBrace();

            //writer.Indent++;
            foreach (var statement in block.Statements)
            {
                // writer.WriteIndent();
                Write(writer, statement);
            }

            TriviaProcessor.ProcessTrivias(writer, block.DescendantTrivia());
            // writer.Indent--;

            if (writeBraces)
                writer.CloseBrace();
        }

        public static void WriteStatementAsBlock(OutputWriter writer, StatementSyntax statement, bool writeBraces = true)
        {
            if (statement is BlockSyntax)
                WriteBlock(writer, statement.As<BlockSyntax>(), writeBraces);
            else
            {
                if (writeBraces)
                    writer.OpenBrace();

                Write(writer, statement);
                TriviaProcessor.ProcessTrivias(writer, statement.DescendantTrivia());

                if (writeBraces)
                    writer.CloseBrace();
            }
        }
    }
}