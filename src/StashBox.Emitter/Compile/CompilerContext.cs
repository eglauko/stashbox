#if NET45 || NET40 || IL_EMIT
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading;

namespace Stashbox.BuildUp.Expressions.Compile
{
    public class CompilerContext
    {
        private readonly AtomicBool hasCapturedVariablesArgumentConstructed;

        public Expression[] DefinedVariables { get; }

        public Expression[] CapturedArguments { get; }

        public Expression[] StoredExpressions { get; }

        public DelegateTarget Target { get; }

        public CapturedArgumentsHolder CapturedArgumentsHolder { get; }

        public bool IsNestedLambda { get; }

        public LocalBuilder[] LocalBuilders { get; set; }

        public LocalBuilder CapturedArgumentsHolderVariable { get; set; }

        public KeyValuePair<LambdaExpression, Expression[]>[] NestedLambdas { get; }

        public bool HasClosure => this.Target != null;

        public bool HasCapturedVariablesArgument => this.CapturedArguments.Length > 0;

        public bool HasCapturedVariablesArgumentConstructed => !this.hasCapturedVariablesArgumentConstructed.CompareExchange(false, true);

        public CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments,
            KeyValuePair<LambdaExpression, Expression[]>[] nestedLambdas, CapturedArgumentsHolder capturedArgumentsHolder)
            : this(target, definedVariables, storedExpressions, capturedArguments, nestedLambdas, capturedArgumentsHolder, false, false)
        { }

        private CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments,
            KeyValuePair<LambdaExpression, Expression[]>[] nestedLambdas, CapturedArgumentsHolder capturedArgumentsHolder, bool isNestedLambda, bool hasCapturedVariablesArgumentConstructed)
        {
            this.hasCapturedVariablesArgumentConstructed = new AtomicBool(hasCapturedVariablesArgumentConstructed);
            this.Target = target;
            this.DefinedVariables = definedVariables;
            this.StoredExpressions = storedExpressions;
            this.CapturedArgumentsHolder = capturedArgumentsHolder;
            this.IsNestedLambda = isNestedLambda;
            this.CapturedArguments = capturedArguments;
            this.NestedLambdas = nestedLambdas;
        }

        public CompilerContext CreateNew(Expression[] definedVariables, bool isNestedLambda) =>
            new CompilerContext(this.Target, definedVariables, this.StoredExpressions, this.CapturedArguments, this.NestedLambdas,
                this.CapturedArgumentsHolder, isNestedLambda, this.hasCapturedVariablesArgumentConstructed.Value);
    }


    /// <summary>
    /// Represents an atomic boolean implementation.
    /// </summary>
    public class AtomicBool
    {

        private const int ValueTrue = 1;
        private const int ValueFalse = 0;

        private int currentValue;

        /// <summary>
        /// Constructs an <see cref="AtomicBool"/>
        /// </summary>
        /// <param name="initialValue">The initial internal value.</param>
        public AtomicBool(bool initialValue = false)
        {
            this.currentValue = this.BoolToInt(initialValue);
        }

        private int BoolToInt(bool value)
        {
            return value ? ValueTrue : ValueFalse;
        }

        private bool IntToBool(int value)
        {
            return value == ValueTrue;
        }

        /// <summary>
        /// Returns the value of the AtomicBool.
        /// </summary>
        public bool Value
        {
            get { return this.IntToBool(this.currentValue); }
            set { this.currentValue = this.BoolToInt(value); }
        }

        /// <summary>
        /// Compares the internal value with the expected value and if they matches the internal value will be replaced with the new value in one atomic operation.
        /// </summary>
        /// <param name="expectedValue">The expected value of the comparison.</param>
        /// <param name="newValue">The new value, the internal value will be replaced with this.</param>
        /// <returns></returns>
        public bool CompareExchange(bool expectedValue, bool newValue)
        {
            var expectedVal = this.BoolToInt(expectedValue);
            var newVal = this.BoolToInt(newValue);
            return Interlocked.CompareExchange(ref this.currentValue, newVal, expectedVal) == expectedVal;
        }
    }
}
#endif