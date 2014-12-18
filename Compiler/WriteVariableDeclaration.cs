// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteVariableDeclaration
    {
        public static void Go(OutputWriter writer, VariableDeclarationSyntax declaration)
        {
            foreach (var variable in declaration.Variables)
            {
                ISymbol symbol = TypeProcessor.GetDeclaredSymbol(variable);

                var isRef = false; //UsedAsRef(variable, symbol);

                writer.WriteIndent();
                // writer.Write("var ");

                //                if (isRef) //Not needed c++ can passby ref
                //                {
                //
                //                    var typeStr = TypeProcessor.ConvertType(declaration.Declaration.Type);
                //
                //                    var localSymbol = symbol as ILocalSymbol;
                //                    var ptr = localSymbol != null && !localSymbol.Type.IsValueType?"*" : "";
                //                                        writer.Write("gc::gc_ptr < " + typeStr+ ptr + " >");
                //                    writer.Write("" + typeStr + ptr + "");
                //
                //                    writer.Write(" ");
                //                    writer.Write(WriteIdentifierName.TransformIdentifier(variable.Identifier.ValueText));
                //                    
                //                    Program.RefOutSymbols.TryAdd(symbol, null);
                //
                //                    writer.Write(" = std::make_shared < ");
                //                    writer.Write(typeStr + ptr);
                //                    writer.Write(" >(");
                //
                //                    WriteInitializer(writer, declaration, variable);
                //
                //                    writer.Write(")");
                //                }
                //                else
                {
                    var lsymbol = symbol as ILocalSymbol;

                    if (lsymbol != null && lsymbol.Type.IsValueType == false)
                    {
                        writer.Write(" ");
                            // Ideally Escape analysis should take care of this, but for now all value types are on heap and ref types on stack
                    }

                    writer.Write(TypeProcessor.ConvertType(declaration.Type));

                    if (lsymbol != null && lsymbol.Type.IsValueType == false)
                        writer.Write(" ");

                    writer.Write(" ");
                    writer.Write(WriteIdentifierName.TransformIdentifier(variable.Identifier.ValueText));
                    writer.Write(" = ");

                    WriteInitializer(writer, declaration, variable);
                }

                writer.Write(";\r\n");
            }
        }

        private static void ProcessInitializer(OutputWriter writer, VariableDeclarationSyntax declaration,
            VariableDeclaratorSyntax variable)
        {
            var initializer = variable.Initializer;
            if (initializer != null)
            {
                if (CSharpExtensions.CSharpKind(initializer.Value) == SyntaxKind.CollectionInitializerExpression)
                    return;
                var value = initializer.Value;
                var initializerType = TypeProcessor.GetTypeInfo(value);
                var memberaccessexpression = value as MemberAccessExpressionSyntax;
                var nameexpression = value as NameSyntax;
                var nullAssignment = value.ToFullString().Trim() == "null";
                var shouldBox = initializerType.Type != null && initializerType.Type.IsValueType &&
                                !initializerType.ConvertedType.IsValueType;
                var shouldUnBox = initializerType.Type != null && !initializerType.Type.IsValueType &&
                                  initializerType.ConvertedType.IsValueType;
                var isname = value is NameSyntax;
                var ismemberexpression = value is MemberAccessExpressionSyntax ||
                                         (isname &&
                                          TypeProcessor.GetSymbolInfo(value as NameSyntax).Symbol.Kind ==
                                          SymbolKind.Method);
                var isdelegateassignment = ismemberexpression &&
                                           initializerType.ConvertedType.TypeKind == TypeKind.Delegate;
                var isstaticdelegate = isdelegateassignment &&
                                       ((memberaccessexpression != null &&
                                         TypeProcessor.GetSymbolInfo(memberaccessexpression).Symbol.IsStatic) ||
                                        (isname && TypeProcessor.GetSymbolInfo(nameexpression).Symbol.IsStatic));
                var shouldCast = initializerType.Type != initializerType.ConvertedType &&
                                 initializerType.ConvertedType != null;

                if (nullAssignment)
                {
                    writer.Write("null");
                    return;
                }
                if (shouldBox)
                {
                    //Box
                    writer.Write("BOX!( " + TypeProcessor.ConvertType(initializerType.Type) + " )(");
                    //When passing an argument by ref or out, leave off the .Value suffix
                    Core.Write(writer, value);
                    writer.Write(")");
                    return;
                }
                if (shouldUnBox)
                {
                    //UnBox
                    writer.Write("cast( " + TypeProcessor.ConvertType(initializerType.Type) + " )(");
                    Core.Write(writer, value);
                    writer.Write(")");
                    return;
                }
                if (initializer.Parent.Parent.Parent is FixedStatementSyntax)
                {
                    //TODO write a better fix
                    var type = TypeProcessor.GetTypeInfo(declaration.Type);

                    writer.Write("cast( " + TypeProcessor.ConvertType(type.Type) + " )(");
                    Core.Write(writer, value);
                    writer.Write(")");
                    return;
                }
                if (isdelegateassignment)
                {
                    var typeString = TypeProcessor.ConvertType(initializerType.ConvertedType);

                    var createNew = !(value is ObjectCreationExpressionSyntax);

                    if (createNew)
                    {
                        if (initializerType.ConvertedType.TypeKind == TypeKind.TypeParameter)
                            writer.Write(" __TypeNew!(" + typeString + ")(");
                        else
                            writer.Write("new " + typeString + "(");
                    }

                    var isStatic = isstaticdelegate;
                    if (isStatic)
                        writer.Write("__ToDelegate(");
                    writer.Write("&");

                    Core.Write(writer, value);
                    if (isStatic)
                        writer.Write(")");

                    if (createNew)
                        writer.Write(")");
                    return;
                }
                if (initializerType.Type == null && initializerType.ConvertedType == null)
                {
                    writer.Write("null");
                    return;
                }
                Core.Write(writer, value);
            }
            else
                writer.Write(TypeProcessor.DefaultValue(declaration.Type));
        }

        private static void WriteInitializer(OutputWriter writer, VariableDeclarationSyntax declaration,
            VariableDeclaratorSyntax variable)
        {
            ProcessInitializer(writer, declaration, variable);
        }

        /// <summary>
        ///     Determines if the passed symbol is used in any ref or out clauses
        /// </summary>
        private static bool UsedAsRef(VariableDeclaratorSyntax variable, ISymbol symbol)
        {
            SyntaxNode node = variable;
            BlockSyntax scope;
            do
                scope = (node = node.Parent) as BlockSyntax;
            while (scope == null);

            return scope.DescendantNodes().OfType<InvocationExpressionSyntax>()
                .SelectMany(o => o.ArgumentList.Arguments)
                .Where(o => o.RefOrOutKeyword.RawKind != (decimal) SyntaxKind.None)
                .Any(o => TypeProcessor.GetSymbolInfo(o.Expression).Symbol == symbol);
        }
    }
}