using System;
using System.Collections.Generic;
using System.Text;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents a function invocation
    /// </summary>
    public class FunctionInvocationNode : ExpressionNode
    {
        /// <summary>
        /// The name of the function
        /// </summary>
        public string FunctionName { get; }

        /// <summary>
        /// The list of argument expressions
        /// </summary>
        public List<ExpressionNode> ArgumentExpressions { get; }

        /// <summary>
        /// Initializes the function invocation
        /// </summary>
        /// <param name="functionName">Name of the function</param>
        /// <param name="argumentExpressions">Argument expressions</param>
        public FunctionInvocationNode(string functionName, List<ExpressionNode> argumentExpressions)
        {
            FunctionName = functionName;
            ArgumentExpressions = argumentExpressions;
        }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IEvaluationContext evalContext) => 
            ArgumentExpressions.TrueForAll(expr => expr.ReadyToEvaluate(evalContext));

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            // --- Evaluate all arguments from left to right
            var argValues = new List<ExpressionValue>();
            var errorMessage = new StringBuilder(1024);
            var index = 0;
            var errCount = 0;
            foreach (var expr in ArgumentExpressions)
            {
                index++;
                var argValue = expr.Evaluate(evalContext);
                if (argValue == null)
                {
                    errCount++;
                    errorMessage.AppendLine($"Arg #{index}: {expr.EvaluationError}");
                }
                else
                {
                    argValues.Add(argValue);
                }
            }

            // --- Check for evaluation errors
            if (errCount > 0)
            {
                EvaluationError = $"Function argument evaluation failed:\n {errorMessage}";
                return ExpressionValue.Error;
            }

            // --- Function must be defined
            if (!s_Evaluators.TryGetValue(FunctionName, out var evaluator))
            {
                EvaluationError = $"Unknown function '{FunctionName}'";
                return ExpressionValue.Error;
            }

            // --- Find the apropriate signature
            FunctionEvaluator evaluatorFound = null;
            foreach (var evalOption in evaluator)
            {
                if (evalOption.ArgTypes.Length != ArgumentExpressions.Count) continue;

                // --- A viable option found
                var match = true;
                for (var i = 0; i < evalOption.ArgTypes.Length; i++)
                {
                    var type = argValues[i].Type;
                    switch (evalOption.ArgTypes[i])
                    {
                        case ExpressionValueType.Bool:
                            match = type == ExpressionValueType.Bool;
                            break;
                        case ExpressionValueType.Integer:
                            match = type == ExpressionValueType.Bool
                                    || type == ExpressionValueType.Integer;
                            break;
                        case ExpressionValueType.Real:
                            match = type == ExpressionValueType.Bool
                                    || type == ExpressionValueType.Integer
                                    || type == ExpressionValueType.Real;
                            break;
                        case ExpressionValueType.String:
                            match = type == ExpressionValueType.String;
                            break;
                        default:
                            return ExpressionValue.Error;
                    }

                    // --- Abort search if the current argumernt type does not match
                    if (!match) break;
                }

                if (match)
                {
                    // --- We have found a matching signature
                    evaluatorFound = evalOption;
                    break;
                }
            }

            // --- Check whether we found an option
            if (evaluatorFound == null)
            {
                EvaluationError = $"The arguments of '{FunctionName}' do not match any acceptable signatures";
                return ExpressionValue.Error;

            }

            // --- Now, it is time to evaluate the function
            try
            {
                var functionValue = evaluatorFound.EvaluateFunc(argValues);
                return functionValue;
            }
            catch (Exception e)
            {
                EvaluationError = $"Exception while evaluating '{FunctionName}': {e.Message}";
                return ExpressionValue.Error;
            }
        }

        /// <summary>
        /// This class describes a function evaluator
        /// </summary>
        internal class FunctionEvaluator
        {
            /// <summary>
            /// Argument Types
            /// </summary>
            public ExpressionValueType[] ArgTypes { get; }

            /// <summary>
            /// Function evaluation
            /// </summary>
            public Func<IList<ExpressionValue>, ExpressionValue> EvaluateFunc { get; }

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public FunctionEvaluator(Func<IList<ExpressionValue>, ExpressionValue> evaluateFunc, params ExpressionValueType[] argTypes)
            {
                ArgTypes = argTypes;
                EvaluateFunc = evaluateFunc;
            }
        }

        /// <summary>
        /// Declares the function evaluator methods
        /// </summary>
        private static readonly Dictionary<string, IList<FunctionEvaluator>> s_Evaluators = 
            new Dictionary<string, IList<FunctionEvaluator>>
        {
            { "abs", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Abs(args[0].AsLong())), ExpressionValueType.Integer),
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Abs(args[0].AsReal())), ExpressionValueType.Real),
                }
            },
            { "acos", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Acos(args[0].AsReal())), ExpressionValueType.Real),
                }
            }
        };
    }
}