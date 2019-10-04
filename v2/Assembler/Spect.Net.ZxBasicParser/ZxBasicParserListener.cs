using Antlr4.Runtime;
using Spect.Net.ZxBasicParser.Generated;

namespace Spect.Net.ZxBasicParser
{
    class ZxBasicParserListener: ZxBasicBaseListener
    {
        private readonly BufferedTokenStream _tokens;

        public ZxBasicParserListener(BufferedTokenStream tokens)
        {
            _tokens = tokens;
        }
    }
}
