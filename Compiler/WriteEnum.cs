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
    internal static class WriteEnum
    {
        //TODO should enum be light class of static members ? or just a plain enum ? (using plain enum for now)
        public static void Go(OutputWriter writer, IEnumerable<EnumMemberDeclarationSyntax> allChildren)
        {
//            writer.IsInterface = true;
            writer.Write("struct ");
            writer.Write(WriteType.TypeName(Context.Instance.Type, false) + "// Enum");

            //            writer.Write(Context.Instance.TypeName);

            //TODO: Find a better fix for this, casting integers to e.g. enum of ubyte gives lots of issues
            // writer.Write(":" + TypeProcessor.ConvertType(Context.Instance.Type.EnumUnderlyingType));
            writer.Write("\r\n");

           



            writer.OpenBrace();

            writer.WriteLine("public " + TypeProcessor.ConvertType(Context.Instance.Type.EnumUnderlyingType) + " __Value;");
            writer.WriteLine("alias __Value this;");

            writer.WriteLine("public enum __IsEnum = true; // Identifies struct as enum");

            

            writer.WriteLine(string.Format("public this({0} value)", TypeProcessor.ConvertType(Context.Instance.Type.EnumUnderlyingType)));
            writer.OpenBrace();
            writer.WriteLine("__Value = value;");
             writer.CloseBrace();


            writer.WriteLine();
            writer.WriteLine("public Type GetType()");
            writer.OpenBrace();
            writer.WriteLine("return __TypeOf!(typeof(this));");
            writer.CloseBrace();

            long lastEnumValue = 0;
            var children = allChildren.ToArray();
            var values =
                children.Select(
                    o => new {Syntax = o, Value = o.EqualsValue != null ? o.EqualsValue.Value : null})
                    .ToList();

            for (int index = 0; index < values.Count; index++)
            {
                var value = values[index];
                var text = "";

                text = "public enum " + WriteType.TypeName(Context.Instance.Type, false) + " "
                       + WriteIdentifierName.TransformIdentifier(value.Syntax.Identifier.ValueText);
                var expressionSyntax = value.Value;
                var expression = expressionSyntax;

                //lets try parsing the value so we can evaluate it
                if (expression != null)
                {
                    var type = TypeProcessor.GetTypeInfo(expression);

                    var tempw = new TempWriter();

                    Core.Write(tempw, expression);

                    var temp = tempw.ToString();

                    if (type.Type != Context.Instance.Type.EnumUnderlyingType)
                    {
                        //TODO: only int enums are supported properly ... should we change them to static structs with constants ?
                        //						temp = "cast(" + TypeProcessor.ConvertType (Context.Instance.Type.EnumUnderlyingType) + ")" + temp;
                        temp = "cast(" + TypeProcessor.ConvertType(Context.Instance.Type.EnumUnderlyingType) + ")" +
                               temp;
                    }

                    text += " = " + temp;
                    tempw.Dispose();
                    ;
                }
                else
                {
                    if (expressionSyntax!=null && expressionSyntax.ToFullString().Trim() != "")
                        text += expressionSyntax;
                    else
                    {
                        if (index > 0)
                        {
                            text += " = " +
                                WriteIdentifierName.TransformIdentifier(values[index - 1].Syntax.Identifier.ValueText) +
                                " + 1";
                        }
                        else
                        {
                            text += " = 0";
                        }

                    }
                }

                writer.WriteLine(text + ";");
            }

            writer.WriteLine();
            var typeName = WriteType.TypeName(Context.Instance.Type, false);

            var baseString = "";


            writer.WriteLine();

            writer.WriteLine("{0} opBinary(string op)({0} rhs)", typeName);
            writer.OpenBrace();
            writer.WriteLine("return mixin(\"{0}(__Value \"~op~\" rhs.__Value)\");",typeName);
            writer.CloseBrace();

            writer.WriteLine("bool opEquals(const {0} a)", typeName);
            writer.OpenBrace();
            writer.WriteLine("return a.__Value == this.__Value;", typeName);
            writer.CloseBrace();

            writer.WriteLine("bool opEquals(const {0} a)", TypeProcessor.ConvertType(Context.Instance.Type.EnumUnderlyingType));
            writer.OpenBrace();
            writer.WriteLine("return a == this.__Value;", typeName);
            writer.CloseBrace();


            writer.WriteLine("public string toString()");
            writer.OpenBrace();
            foreach (var membername in values)
            {
                var name = WriteIdentifierName.TransformIdentifier(membername.Syntax.Identifier.ValueText);
                writer.WriteLine("if (this == {0}.__Value)", name);
                writer.OpenBrace();
                writer.WriteLine("return \"{0}\";", name);
                writer.CloseBrace();
            }
            writer.WriteLine("return std.conv.to!string(GetType().FullName.Text);");
            writer.CloseBrace();

            //Not needed for enum ... all enum should have a ToString function ...
            //            writer.WriteLine("public static class __Boxed_" + " " +
            //                                 //(genericArgs.Any() ? ("( " + (string.Join(" , ", genericArgs.Select(o => o)) + " )")) : "") +//Internal boxed should not be generic
            //                                 ": Boxed!(" + typeName + ")" + baseString);
            //
            //            writer.OpenBrace();
            //
            //
            //
            //            writer.WriteLine("import std.traits;");
            //
            //            var members = new List<ISymbol>();
            //
            //           
            //
            //            //  foreach (var baseType in bases.Where(o => o.TypeKind == TypeKind.Interface))
            //            //{
            //
            //            var ifacemembers = members.DistinctBy(k => k);//Utility.GetAllMembers(Context.Instance.Type);
            //
            //            foreach (var member in ifacemembers)
            //            {
            //                var ifacemethod =
            //                    Context.Instance.Type.FindImplementationForInterfaceMember(member)
            //                        .DeclaringSyntaxReferences.First()
            //                        .GetSyntax();
            //
            //                var syntax = ifacemethod as MethodDeclarationSyntax;
            //                if (syntax != null)
            //                    WriteMethod.WriteIt(writer, syntax);
            //
            //                var property = ifacemethod as PropertyDeclarationSyntax;
            //                if (property != null)
            //                    WriteProperty.Go(writer, property, true);
            //
            //            }
            //            //}
            //
            //            //This is required to be able to create an instance at runtime / reflection
            //            //					this()
            //            //					{
            //            //						super(SimpleStruct.init);
            //            //					}
            //
            //            writer.WriteLine();
            //            writer.WriteLine("this()");
            //            writer.OpenBrace();
            //            writer.WriteLine("super(__TypeNew!({0})());", typeName);
            //            writer.CloseBrace();
            //
            //            writer.WriteLine();
            //            writer.WriteLine("public override Type GetType()");
            //            writer.OpenBrace();
            //            writer.WriteLine("return __TypeOf!(typeof(__Value));");
            //            writer.CloseBrace();
            //
            //
            //            if (Context.Instance.Type.GetMembers("ToString").Any()) // Use better matching ?
            //            {
            //                //					writer.WriteLine ();
            //                writer.WriteLine("override String ToString()");
            //                writer.OpenBrace();
            //                writer.WriteLine("return Value.ToString();");
            //                writer.CloseBrace();
            //            }
            //
            //            writer.WriteLine();
            //            writer.WriteLine("this(ref " + typeName + " value)");
            //            writer.OpenBrace();
            //            writer.WriteLine("super(value);");
            //            writer.CloseBrace();
            //
            //            writer.WriteLine();
            //            writer.WriteLine("U opCast(U)()");
            //            writer.WriteLine("if(is(U:{0}))", typeName);
            //            writer.OpenBrace();
            //            writer.WriteLine("return Value;");
            //            writer.CloseBrace();
            //
            //            writer.WriteLine();
            //            writer.WriteLine("U opCast(U)()");
            //            writer.WriteLine("if(!is(U:{0}))", typeName);
            //            writer.OpenBrace();
            //            writer.WriteLine("return this;");
            //            writer.CloseBrace();
            //
            //            writer.WriteLine();
            //            writer.WriteLine("auto opDispatch(string op, Args...)(Args args)");
            //            writer.OpenBrace();
            //            writer.WriteLine("enum name = op;");
            //            writer.WriteLine("return __traits(getMember, Value, name)(args);");
            //            writer.CloseBrace();

            //            writer.WriteLine();
            //            writer.WriteLine("public override Type GetType()");
            //            writer.OpenBrace();
            //            writer.WriteLine("return __Value.GetType();");
            //            writer.CloseBrace();


            //            writer.CloseBrace();

            writer.CloseBrace();
//            writer.Write(";");
        }




     
    }
}