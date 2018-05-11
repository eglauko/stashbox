using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Expressions.Compile
{
    public class LambdaContext
    {
        public Expression[] DefinedVariables { get; }

        public Type StoredLambdaType { get; set; }

        public LambdaContext(Expression[] definedVariables)
        {
            this.DefinedVariables = definedVariables;
        }
    }
}
