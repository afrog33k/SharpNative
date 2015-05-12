// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CSharp.RuntimeBinder;

#endregion

namespace SharpNative.Compiler
{
  
    internal static class WriteLocalDeclaration
    {

        public static Assembly Compile(params string[] sources)
        {
            var assemblyFileName = "gen" + Guid.NewGuid().ToString().Replace("-", "") + ".dll";
            var compilation = CSharpCompilation.Create(assemblyFileName,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                syntaxTrees: from source in sources
                             select CSharpSyntaxTree.ParseText(source),
                references: new[]
        {
            AssemblyMetadata.CreateFromFile(typeof(object).Assembly.Location).GetReference(),
                    AssemblyMetadata.CreateFromFile(typeof(RuntimeBinderException).Assembly.Location).GetReference(),
                    AssemblyMetadata.CreateFromFile(typeof(DynamicAttribute).Assembly.Location).GetReference(),
        });

            EmitResult emitResult;

            using (var ms = new MemoryStream())
            {
                emitResult = compilation.Emit(ms);

                if (emitResult.Success)
                {
                    var assembly = Assembly.Load(ms.GetBuffer());
                    return assembly;
                }
            }

            var message = string.Join("\r\n", emitResult.Diagnostics);
            throw new ApplicationException(message);
        }

        public static void Go(OutputWriter writer, LocalDeclarationStatementSyntax declaration)
        {
            foreach (var variable in declaration.Declaration.Variables)
            {


                writer.WriteIndent();
               
                {
                    try
                    {
                        var str = TypeProcessor.ConvertType(declaration.Declaration.Type);
                        //This fails sometimes
                        // if (str == "NObject") // Dlangs casting is slow 
                        // Looks harmless but is actually a performance optimization ... makes CastTest improve by a whole lot
                        //   writer.Write("auto ");//Kills BoxingTest unfortunately as boxed types are also used to make unbound generics work :(
                        //else
                        writer.Write(str);
                    }
                    catch (Exception ex)
                    {
                        writer.Write("auto");   
                    }
                   

                    writer.Write(" ");



                    writer.Write(WriteIdentifierName.TransformIdentifier(variable.Identifier.Text));
                    writer.Write(" = ");

                     WriteInitializer(writer, declaration, variable);
                }

                writer.Write(";\r\n");
            }
        }

        private static void ProcessInitializer(OutputWriter writer, LocalDeclarationStatementSyntax declaration,
            VariableDeclaratorSyntax variable)
        {
            var initializer = variable.Initializer;
            if (initializer != null)
            {
                if (initializer.Value.Kind() == SyntaxKind.CollectionInitializerExpression)
                    return;

                var value = initializer.Value;
                var initializerType = TypeProcessor.GetTypeInfo(value);
                var memberaccessexpression = value as MemberAccessExpressionSyntax;
                var nameexpression = value as NameSyntax;
                var nullAssignment = value.ToFullString().Trim() == "null";
                var convertedType = initializerType.ConvertedType;
                var type = initializerType.Type;
                if (type == null && convertedType == null) //TODO: Rare Case (Compiling csnative corlib... need to find a solution, for now just write it out
                {
                    Core.Write(writer, value);
                    return;
                }
               
				var shouldBox = type != null && (type.IsValueType) &&
                                !convertedType.IsValueType;
                var shouldUnBox = type != null && !type.IsValueType &&
                                  convertedType.IsValueType;
                var isname = value is NameSyntax;
                var ismemberexpression = value is MemberAccessExpressionSyntax ||
                                         (isname &&
                                          TypeProcessor.GetSymbolInfo(value as NameSyntax).Symbol.Kind ==
                                          SymbolKind.Method);


                var isdelegateassignment = ismemberexpression &&
                                           convertedType.TypeKind == TypeKind.Delegate;
                var isstaticdelegate = isdelegateassignment &&
                                       ((memberaccessexpression != null &&
                                         TypeProcessor.GetSymbolInfo(memberaccessexpression).Symbol.IsStatic) ||
                                        (isname && TypeProcessor.GetSymbolInfo(nameexpression).Symbol.IsStatic));

                //Do we have an implicit converter, if so, use it
                if (shouldBox || shouldUnBox)
                {
                    
                   if (WriteConverter(writer, type, convertedType, value))
                        return;
                }

                if (nullAssignment)
                {
                    if(convertedType!=null) //Nullable Support
                    if (convertedType.Name == "Nullable")
                    {
                        var atype = TypeProcessor.ConvertType(convertedType);
                        writer.Write(atype+"()");
                    }
                    else
                    writer.Write("null");
                    return;
                }
                if (shouldBox)
                {
                    WriteBox(writer, type, value);
                    return;
                }
                if (shouldUnBox)
                {
                    WriteUnbox(writer, type, value);
                    return;
                }

                if (isdelegateassignment)
                {
                    WriteDelegateAssignment(writer, convertedType, isstaticdelegate, value);
                    return;
                }

//                if (type == null && convertedType == null)
//                {
//                    writer.Write("null");
//                    return;
//                }

                //CTFE
              /* var aVal =  EvaluateValue(value);
                if (String.IsNullOrEmpty(aVal))
                {
                    writer.Write(aVal);
                }
                else*/
                    Core.Write(writer, value);
            }
            else
                writer.Write(TypeProcessor.DefaultValue(declaration.Declaration.Type));
        }

        private static string EvaluateValue(ExpressionSyntax val)
        {
            try
            {
//Should be able to extend this to do some basic CTFE and speed up pure method calls
                // var assembly = Compile(code);
                var type2 = Program.CurrentAssembly.GetType("Primes");
                var method = type2.GetMethod("AddPrimes", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var result = method.Invoke(null, new object[] {10000});


                // var constantval = Program.GetModel(value).GetConstantValue(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                //throw;
            }
            return null;
        }

        private static void WriteBox(OutputWriter writer, ITypeSymbol type, ExpressionSyntax value)
        {
            writer.Write("BOX!(" + TypeProcessor.ConvertType(type) + ")(");
            if (type.TypeKind == TypeKind.Enum)
            {
                writer.Write("cast({0})", TypeProcessor.ConvertType(type));
            }
            Core.Write(writer, value);
            writer.Write(")");
        }

        private static void WriteUnbox(OutputWriter writer, ITypeSymbol type, ExpressionSyntax value)
        {
            writer.Write("Cast!(" + TypeProcessor.ConvertType(type) + ")(");
            Core.Write(writer, value);
            writer.Write(")");
        }

        private static void WriteDelegateAssignment(OutputWriter writer, ITypeSymbol convertedType, bool isstaticdelegate,
            ExpressionSyntax value)
        {
            var typeString = TypeProcessor.ConvertType(convertedType);

            if (convertedType.TypeKind == TypeKind.TypeParameter)
                writer.Write(" __TypeNew!(" + typeString + ")(");
            else
                writer.Write("new " + typeString + "(");

            var isStatic = isstaticdelegate;
            //            if (isStatic)
            //                writer.Write("__ToDelegate(");
            MemberUtilities.WriteMethodPointer(writer, value);
            //            if (isStatic)
            //                writer.Write(")");

            writer.Write(")");
            return;
        }

        private static bool WriteConverter(OutputWriter writer, ITypeSymbol type, ITypeSymbol convertedType,
            ExpressionSyntax value)
        {
            bool useType = true;
            var correctConverter =
                type.GetImplicitCoversionOp(convertedType,
                    type,true);

            if (correctConverter == null)
            {
                useType = false;
                correctConverter =
                    convertedType.GetImplicitCoversionOp(convertedType,
                        type,true);
            }

            if (correctConverter != null)
            {
                if (useType)
                {
                    writer.Write(TypeProcessor.ConvertType(type) + "." + "op_Implicit_" +
                                 TypeProcessor.ConvertType(correctConverter.ReturnType, false, true, false).Replace(".", "_"));
                }
                else
                {
                    writer.Write(TypeProcessor.ConvertType(convertedType) + "." +
                                 "op_Implicit_" + TypeProcessor.ConvertType(correctConverter.ReturnType, false, true, false).Replace(".", "_"));
                }
                writer.Write("(");
                Core.Write(writer, value);
                writer.Write(")");
                return true;
            }
            return false;
        }


        private static void WriteInitializer(OutputWriter writer, LocalDeclarationStatementSyntax declaration,
            VariableDeclaratorSyntax variable)
        {
            ProcessInitializer(writer, declaration, variable);
        }

       
    }
}