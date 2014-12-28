// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

#endregion

namespace SharpNative.Compiler
{
    public class SymbolNameMap
    {
        private readonly Dictionary<ISymbol, string> names;

        public SymbolNameMap(Dictionary<ISymbol, string> names)
        {
            this.names = names;
        }

        public string this[ISymbol symbol, string fallbackName]
        {
            get
            {
                string result;
                if (!names.TryGetValue(symbol, out result))
                    return fallbackName;

                if (symbol is INamespaceSymbol && !symbol.ContainingNamespace.IsGlobalNamespace)
                    result = this[symbol.ContainingNamespace, symbol.ContainingNamespace.FullName()] + "." + result;

                return result;
            }
        }
    }
}