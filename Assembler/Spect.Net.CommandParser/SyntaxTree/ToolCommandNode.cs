// ReSharper disable IdentifierTypo

using System.Globalization;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class is intended to be the base of all tool commands used in SpectNetIde
    /// </summary>
    public abstract class ToolCommandNode
    {
        /// <summary>
        /// Signs if this node has a semantic error
        /// </summary>
        public bool HasSemanticError { get; set; }

        /// <summary>
        /// Processes the specified identifier
        /// </summary>
        /// <param name="idText">ID text to process</param>
        /// <param name="number">Number output</param>
        /// <param name="symbol">Symbol output</param>
        /// <returns>True, if ID is a symbol; otherwise, false</returns>
        protected bool ProcessId(string idText, out ushort number, out string symbol)
        {
            number = 0;
            symbol = null;
            if (idText.Length == 0)
            {
                HasSemanticError = true;
                return false;
            }

            var firstChar = idText[0];
            if (char.IsDigit(firstChar))
            {
                // --- Try to parse it as a hexadecimal number
                if (ushort.TryParse(idText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out number))
                {
                    return false;
                }

                // --- It must be an error
                HasSemanticError = true;
                return false;
            }

            if (firstChar == ':')
            {
                // --- Try to parse it as a hexadecimal number
                if (ushort.TryParse(idText.Substring(1), out number))
                {
                    return false;
                }

                // --- It must be an error
                HasSemanticError = true;
                return false;
            }

            symbol = idText;
            return true;
        }

        /// <summary>
        /// Handles the specified ID text as a hexadecimal number
        /// </summary>
        /// <param name="idText">ID to parse as hexadecimal</param>
        /// <returns>Hexadecimal value</returns>
        /// <remarks>Sets a semantic error, if cannot be parsed</remarks>
        protected ushort ProcessNumber(string idText)
        {
            var firstChar = idText[0];
            if (char.IsDigit(firstChar))
            {
                if (ushort.TryParse(idText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var number))
                {
                    return number;
                }
            }
            else if (firstChar == ':')
            {
                // --- Try to parse it as a hexadecimal number
                if (ushort.TryParse(idText.Substring(1), out var number))
                {
                    return number;
                }
            }

            HasSemanticError = true;
            return 0;
        }
    }
}