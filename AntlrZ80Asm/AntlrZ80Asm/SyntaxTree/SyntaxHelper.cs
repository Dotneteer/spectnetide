using Antlr4.Runtime.Tree;

namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class contains helper methods for creating the syntax tree
    /// </summary>
    public static class SyntaxHelper
    {
        /// <summary>
        /// Normalizes a token by omitting whitespaces and converting to uppercase
        /// </summary>
        /// <param name="element">Parse tree to get the element from</param>
        /// <returns>Normalized token</returns>
        public static string NormalizeToken(this IParseTree element) 
            => element?.GetText().NormalizeToken();

        /// <summary>
        /// Normalizes a token by omitting whitespaces and converting to uppercase
        /// </summary>
        /// <param name="token">Token to normalize</param>
        /// <returns>Normalized token</returns>
        public static string NormalizeToken(this string token) 
            => token?.ToUpperInvariant();
    }
}