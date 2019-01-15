using System.Collections.Generic;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// Represents an expression node that can be evaluated
    /// </summary>
    public abstract class ExpressionNode
    {
        /// <summary>
        /// Gets the list of symbol errors
        /// </summary>
        public static List<string> SymbolErrorList { get; } = new List<string>();

        /// <summary>
        /// Symbol names concatenated.
        /// </summary>
        public static string SymbolErrors => $"{string.Join(", ", SymbolErrorList)}";

        /// <summary>
        /// Clears the error list
        /// </summary>
        public static void ClearErrors()
        {
            SymbolErrorList.Clear();
        }

        /// <summary>
        /// Adds a new symbol to the error list
        /// </summary>
        /// <param name="id"></param>
        public static void AddError(string id)
        {
            SymbolErrorList.Add(id);
        }

        /// <summary>
        /// The source text of the expression
        /// </summary>
        public string SourceText { get; set; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public virtual bool ReadyToEvaluate(IEvaluationContext evalContext) => true;

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public abstract ExpressionValue Evaluate(IEvaluationContext evalContext);

        /// <summary>
        /// Retrieves the evaluation error text, provided there is any issue
        /// </summary>
        public virtual string EvaluationError { get; set; } = null;

        /// <summary>
        /// Indicates if this expression has a macro parameter
        /// </summary>
        public virtual bool HasMacroParameter => false;
    }
}