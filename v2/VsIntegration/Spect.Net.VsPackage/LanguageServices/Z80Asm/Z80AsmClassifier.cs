using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Spect.Net.Assembler;
using Spect.Net.Assembler.Generated;

namespace Spect.Net.VsPackage.LanguageServices.Z80Asm
{
    /// <summary>
    /// This class carries out the classification of the Z80 Assembly language text
    /// </summary>
    internal class Z80AsmClassifier: IClassifier
    {
        // --- Store registered classification types here
        private readonly IClassificationType
            _label,
            _pragma,
            _directive,
            _includeDirective,
            _instruction,
            _comment,
            _number,
            _identifier,
            _string,
            _function,
            _macroParam,
            _statement,
            _macroInvocation,
            _operand,
            _semiVar,
            _module,
            _breakpoint,
            _currentBreakpoint;

        private readonly ITextBuffer _buffer;
        private bool _isProcessing;
        private Z80AsmVisitor _z80SyntaxTreeVisitor;

        /// <summary>
        /// Initializes the Z80 Assembly language classifier with the specified
        /// attributes
        /// </summary>
        /// <param name="buffer">The text buffer of the Z80 Assembly source code</param>
        /// <param name="registry">The service that manages known classification types</param>
        internal Z80AsmClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            _buffer = buffer;

            _label = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_LABEL);
            _pragma = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_PRAGMA);
            _directive = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_DIRECTIVE);
            _includeDirective = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_INCLUDE_DIRECTIVE);
            _instruction = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_INSTRUCTION);
            _comment = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_COMMENT);
            _number = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_NUMBER);
            _identifier = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_IDENTIFIER);
            _string = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_STRING);
            _function = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_FUNCTION);
            _macroParam = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_MACRO_PARAM);
            _statement = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_STATEMENT);
            _macroInvocation = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_MACRO_INVOCATION);
            _operand = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_OPERAND);
            _semiVar = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_SEMI_VAR);
            _module = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_MODULE);
            _breakpoint = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_BREAKPOINT);
            _currentBreakpoint = registry.GetClassificationType(Z80AsmClassificationTypes.Z80_CURRENT_BREAKPOINT);

            ParseDocument();

            _buffer.Changed += OnBufferChanged;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var list = new List<ClassificationSpan>();

            //if (_z80SyntaxTreeVisitor == null || _isProcessing || span.IsEmpty)
            return list;
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            ParseDocument();
        }

        /// <summary>
        /// Parses the entire source code 
        /// </summary>
        private async void ParseDocument()
        {
            // --- Do not start parsing over existing parsing
            if (_isProcessing) return;
            _isProcessing = true;

            await Task.Run(() =>
            {
                try
                {
                    // --- Get the entire text of the source code
                    var span = new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length);
                    var source = _buffer.CurrentSnapshot.GetText(span);

                    // --- Let's use the Z80 assembly parser to obtain tags
                    var inputStream = new AntlrInputStream(source);
                    var lexer = new Z80AsmLexer(inputStream);
                    var tokenStream = new CommonTokenStream(lexer);
                    var parser = new Z80AsmParser(tokenStream);
                    var context = parser.asmline();
                    _z80SyntaxTreeVisitor = new Z80AsmVisitor(inputStream);
                    _z80SyntaxTreeVisitor.Visit(context);

                    // --- Code is parsed, sign the change
                    ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(span));
                }
                finally
                {
                    _isProcessing = false;
                }
            });
        }
    }
}