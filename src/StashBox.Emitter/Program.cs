using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Stashbox.BuildUp.Expressions.Compile;
using System.Diagnostics;

namespace StashBox.Emitter.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MakeExpressions();
            Console.ReadKey();
        }

        private static void MakeExpressions()
        {
            Console.WriteLine("Run Expression created via code");

            var tuple = RunExpression<Basic>(c => new Basic(), true, true);
            var basicCompiled = tuple.Item1;
            var basicEmitted = tuple.Item2;
            tuple = RunExpression<Simple>(c => new Simple((Basic)basicCompiled(c)), true, false);
            var simpleCompiled = tuple.Item1;
            tuple = RunExpression<Simple>(c => new Simple((Basic)basicEmitted(c)), false, true);
            var simpleEmitted = tuple.Item2;
            tuple = RunExpression<Complex>(c => new Complex((Simple)simpleCompiled(c)), true, false);
            var complexCompiled = tuple.Item1;
            tuple = RunExpression<Complex>(c => new Complex((Simple)simpleEmitted(c)), true, false);
            var complexEmitted = tuple.Item2;

            Console.WriteLine();
            Console.WriteLine("Run Expression created on hand");

            var funcType = typeof(Func<Context, object>);
            var ctx = new Context();
            var ctxParameter = Expression.Parameter(typeof(Context));
            var ctxConstant = Expression.Constant(ctx);

            var basicType = typeof(Basic);
            var basicCtor = basicType.GetConstructor(Type.EmptyTypes);
            var basicNew = Expression.New(basicCtor);
            var basicLambda = Expression.Lambda<Func<Context, object>>(basicNew, ctxParameter);

            tuple = RunExpression<Basic>(basicLambda, true, true);
            basicCompiled = tuple.Item1;
            basicEmitted = tuple.Item2;

            var simpleType = typeof(Simple);
            var simpleCtor = simpleType.GetConstructor(new Type[] { basicType });
            
            var basicInvoke = Expression.Invoke(basicLambda, ctxConstant);
            var basicInvokeCast = Expression.Convert(basicInvoke, basicType);

            var simpleNew = Expression.New(simpleCtor, basicInvokeCast);
            var simpleLambda = Expression.Lambda<Func<Context, object>>(simpleNew, ctxParameter);

            tuple = RunExpression<Simple>(simpleLambda, true, true);
            simpleCompiled = tuple.Item1;
            simpleEmitted = tuple.Item2;
        }

        private static Tuple<Func<Context, object>, Func<Context, object>> RunExpression<T>(
            Expression<Func<Context, object>> expression, bool printCompiled, bool printEmitted)
        {
            Func<Context, object> expCompiled = null;
            Func<Context, object> ExpEmitted = null;
            try
            {
                expCompiled = expression.Compile();
            }
            catch(Exception e)
            {
                Console.WriteLine("Expression Compile falhou :(" + e.Message);
            }


            Delegate delegateEmitted;
            if (!expression.TryEmit(out delegateEmitted))
                Console.WriteLine("Expression Emit falhou :(");
            else
                ExpEmitted = (Func<Context, object>)delegateEmitted;


            var ctx = new Context();
            var watch = new Stopwatch();


            if (expCompiled != null)
            {
                watch.Start();
                for (int i = 0; i < 1000000; i++)
                {
                    var basic = (T)expCompiled(ctx);
                }
                watch.Stop();
                if (printCompiled)
                    Console.WriteLine(typeof(T).Name + " Expression Compiled: " + watch.ElapsedMilliseconds);
                watch.Reset();
            }

            if (ExpEmitted != null)
            {
                watch.Start();
                for (int i = 0; i < 1000000; i++)
                {
                    var basic = (T)ExpEmitted(ctx);
                }
                watch.Stop();
                if (printEmitted)
                    Console.WriteLine(typeof(T).Name + "Expression Emitted: " + watch.ElapsedMilliseconds);
            }

            return new Tuple<Func<Context, object>, Func<Context, object>>(expCompiled, ExpEmitted);
        }
    }

    public class Basic
    {

    }

    public class Simple
    {
        private Basic basic;

        public Simple(Basic basic)
        {
            this.basic = basic;
        }
    }

    public class Complex
    {
        public Complex(Simple basic)
        {

        }
    }

    public class Context { }

    public class Resolver
    {
        private Func<Context, object> func;

        public Resolver(Func<Context, object> func)
        {
            this.func = func;
        }

        object Resolce(Context ctx) => func(ctx);
    }
}
