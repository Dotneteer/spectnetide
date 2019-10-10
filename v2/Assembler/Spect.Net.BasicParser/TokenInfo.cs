using System;
using System.Collections.Generic;
using System.Text;

namespace Spect.Net.BasicParser
{
    /// <summary>
    /// This class describes token information
    /// </summary>
    public struct TokenInfo
    {
        /// <summary>
        /// Type of the token (for classification)
        /// </summary>
        public TokenType TokenType { get; }

        /// <summary>
        /// Start column within the row
        /// </summary>
        public int StartColumn { get; }

        /// <summary>
        /// Length of the token
        /// </summary>
        public int Length { get; }

        public TokenInfo(TokenType tokenType, int startColumn, int length)
        {
            TokenType = tokenType;
            StartColumn = startColumn;
            Length = length;
        }
    }
}
