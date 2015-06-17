
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Threading;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SharpNative.Compiler;

namespace VisualCompiler
{

	public static class ListExtensions
	{
		public static IEnumerable<string> CustomSort(this IEnumerable<string> list)
		{
			int maxLen = list.Select(s => s.Length).Max();

			return list.Select(s => new
			{
				OrgStr = s,
				SortStr = Regex.Replace(s, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
			})
				.OrderBy(x => x.SortStr)
				.Select(x => x.OrgStr);
		}

	}
	
}
