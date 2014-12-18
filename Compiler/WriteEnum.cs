// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
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
            writer.Write("enum ");
            writer.Write(Context.Instance.TypeName);
            writer.Write(":" + TypeProcessor.ConvertType(Context.Instance.Type.EnumUnderlyingType) + "\r\n");
            writer.OpenBrace();

            long lastEnumValue = 0;
            var children = allChildren.ToArray();
            var values =
                children.Select(
                    o => new {Syntax = o, Value = o.EqualsValue != null ? o.EqualsValue.Value.ToString() : null})
                    .ToList();

            foreach (var value in values)
            {
                writer.WriteLine(
                    WriteIdentifierName.TransformIdentifier(value.Syntax.Identifier.ValueText) +
                    (value.Value != null ? " = " + value.Value : "") +
                    ",");
            }

            //                writer.WriteLine("final val " + WriteIdentifierName.TransformIdentifier(value.Syntax.Identifier.ValueText) + ":Int = " + value.Value + ";");

            //            writer.WriteLine();
            //            writer.WriteLine(@"def ToString(n:java.lang.Integer):String = if (n == null) """" else ToString(n.intValue());");
            //
            //            writer.WriteLine("def ToString(e:Int):String =");
            //            writer.WriteOpenBrace();
            //            writer.WriteLine("return e match");
            //            writer.WriteOpenBrace();
            //
            //            foreach (var value in values)
            //                writer.WriteLine("case " + value.Value + " => \"" + value.Syntax.Identifier.ValueText + "\";");
            //
            //            writer.WriteCloseBrace();
            //            writer.WriteCloseBrace();
            //
            //            writer.WriteLine();
            //            writer.WriteLine("def Parse(s:String):Int =");
            //            writer.WriteOpenBrace();
            //            writer.WriteLine("return s match");
            //            writer.WriteOpenBrace();
            //
            //            foreach (var value in values)
            //                writer.WriteLine("case \"" + value.Syntax.Identifier.ValueText + "\" | \"" + value.Value + "\" => " + value.Value + ";");
            //
            //            writer.WriteCloseBrace();
            //            writer.WriteCloseBrace();
            //
            //            writer.WriteLine();
            //            writer.WriteIndent();
            //            writer.Write("final val Values:Array[Int] = Array(");
            //            writer.Write(string.Join(", ", values.Select(o => o.Value.ToString())));
            //            writer.Write(");\r\n");

            writer.CloseBrace();
            writer.Write(";");
        }

//        private static long DetermineEnumValue(EnumMemberDeclarationSyntax syntax, ref long lastEnumValue) //Not necessary D and C# enums are equivalent
//        {
//            if (syntax.EqualsValue == null)
//                return ++lastEnumValue;
//
//			var value = syntax.EqualsValue.Value.ToString ();
//
//			if (value.StartsWith ("0x") || value.StartsWith ("0x")) {
//			//probably hex number
//
//			}
//
//			value = value.Replace ("0x","X").Replace("0X","X");  //Int.parse cannot process 0x :(
//
//			if (!long.TryParse(value,  out lastEnumValue))
//                throw new Exception("Enums must be assigned with an integer " + Utility.Descriptor(syntax));
//
//            return lastEnumValue;
//        }


        public static void Check(SyntaxNode node)
        {
            //Check for enums being converted to objects.  There are common methods in .net that accept objects just to call .ToString() on them, such as Console.WriteLine and HttpResponse.Write.  In these cases, we would miss our enum-to-string conversion that ensures the strings are used instead of the number.  To work around this, you should call .ToString() on the enum before passing it in.  It's a good idea to do this anyway for performance, so we just fail instead of doing the conversion automatically.  This check is a bit overzealous, as there are also legitimate reasons to conver enums to objects, but we'd rather be safe and reject a legitimate use than have code behave incorrectly.

            var expression = node as ExpressionSyntax;
            if (expression == null)
                return;

            var typeInfo = TypeProcessor.GetTypeInfo(expression);

            if (typeInfo.Type == null || typeInfo.ConvertedType == null || typeInfo.Type == typeInfo.ConvertedType ||
                typeInfo.Type.BaseType == null)
                return;
            if (typeInfo.ConvertedType.SpecialType != SpecialType.System_Object)
                return;

            if (typeInfo.Type.BaseType.SpecialType != SpecialType.System_Enum)
            {
                if (typeInfo.Type.Name != "Nullable" || typeInfo.Type.ContainingNamespace.FullName() != "System")
                    return;

                var nullableType = typeInfo.Type.As<INamedTypeSymbol>().TypeArguments.Single();

                if (nullableType.BaseType.SpecialType != SpecialType.System_Enum)
                    return;
            }

            throw new Exception(
                "Enums cannot convert to objects.  Use .ToString() if you're using the enum as a string. " +
                Utility.Descriptor(node) + ", expr=" + expression);
        }
    }
}