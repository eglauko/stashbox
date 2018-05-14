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
    /// <summary>
    /// A delegate that represents the dynamic method compiled to resolved service instances.
    /// </summary>
    /// <param name="args">The arguments used by the dynamic method that this delegate represents.</param>
    /// <returns>A service instance.</returns>
    internal delegate object InstanceDelegate(Context ctx);

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
            tuple = RunExpression<Complex>(c => new Complex((Simple)simpleEmitted(c)), false, true);
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

            Console.WriteLine();
            Console.WriteLine("Run Resolver with Expression created on hand");

            tuple = RunExpression<Basic>(basicLambda, false, false);
            basicCompiled = tuple.Item1;
            basicEmitted = tuple.Item2;

            var basicResolverCompiled = UseResolver<Basic>(basicCompiled, "Compiled");
            var basicResolverEmitted = UseResolver<Basic>(basicEmitted, "Emitted");

            var resolverType = typeof(Resolver);
            var resolveMethod = resolverType.GetMethod("Resolve");

            ctxParameter = Expression.Parameter(typeof(Context));
            var basicCallResolveCompiled = Expression.Call(
                Expression.Constant(basicResolverCompiled),
                resolveMethod,
                ctxParameter);

            var simpleNewCompiled = Expression.New(
                simpleCtor,
                Expression.Convert(basicCallResolveCompiled, basicType));

            var simpleLambdaCompiled = Expression.Lambda<Func<Context, object>>(
                simpleNewCompiled,
                ctxParameter);

            tuple = RunExpression<Simple>(simpleLambdaCompiled, true, false);
            simpleCompiled = tuple.Item1;

            ctxParameter = Expression.Parameter(typeof(Context));
            var basicCallResolveEmitted = Expression.Call(
                Expression.Constant(basicResolverEmitted),
                resolveMethod,
                ctxParameter);

            var simpleNewEmitted = Expression.New(
                simpleCtor,
                Expression.Convert(basicCallResolveEmitted, basicType));

            var simpleLambdaEmitted = Expression.Lambda<Func<Context, object>>(
                simpleNewEmitted,
                ctxParameter);

            tuple = RunExpression<Simple>(simpleLambdaEmitted, false, true);
            simpleEmitted = tuple.Item2;



            var simpleResolverCompiled = UseResolver<Simple>(simpleCompiled, "Compiled");
            var simpleResolverEmitted = UseResolver<Simple>(simpleEmitted, "Emitted");


            var complexType = typeof(Complex);
            var complexCtor = complexType.GetConstructor(new Type[] { simpleType });


            ctxParameter = Expression.Parameter(typeof(Context));
            var simpleCallResolveCompiled = Expression.Call(
                Expression.Constant(simpleResolverCompiled),
                resolveMethod,
                ctxParameter);

            var complexNewCompiled = Expression.New(
                complexCtor,
                Expression.Convert(simpleCallResolveCompiled, simpleType));

            var complexLambdaCompiled = Expression.Lambda<Func<Context, object>>(
                complexNewCompiled,
                ctxParameter);

            tuple = RunExpression<Complex>(complexLambdaCompiled, true, false);
            complexCompiled = tuple.Item1;


            ctxParameter = Expression.Parameter(typeof(Context));
            var simpleCallResolveEmitted = Expression.Call(
                Expression.Constant(simpleResolverEmitted),
                resolveMethod,
                ctxParameter);

            var complexNewEmitted = Expression.New(
                complexCtor,
                Expression.Convert(simpleCallResolveEmitted, simpleType));

            var complexLambdaEmitted = Expression.Lambda<Func<Context, object>>(
                complexNewEmitted,
                ctxParameter);

            tuple = RunExpression<Complex>(complexLambdaEmitted, false, true);
            complexEmitted = tuple.Item2;


            var complexResolverCompiled = UseResolver<Complex>(complexCompiled, "Compiled");
            var complexResolverEmitted = UseResolver<Complex>(complexEmitted, "Emitted");

            Console.WriteLine();
            Console.WriteLine("Run Manual Resolver");

            var manual = new ResolverComplex(new ResolverSimple(new ResolverBasic()));
            manual.Run();

            Console.WriteLine();
            Console.WriteLine("Run Test call compiled Function");


            //tuple = RunExpression<Basic>(c => new Basic(), false, true);
            //basicEmitted = tuple.Item2;

            //ctxParameter = Expression.Parameter(typeof(Context));
            //basicCallResolveEmitted = Expression.Call(basicEmitted.Method, ctxParameter);
            //simpleNewEmitted = Expression.New(
            //    simpleCtor,
            //    Expression.Convert(basicCallResolveEmitted, basicType));
            //simpleLambdaEmitted = Expression.Lambda<Func<Context, object>>(
            //    simpleNewEmitted,
            //    ctxParameter);

            //tuple = RunExpression<Simple>(simpleLambdaEmitted, false, true);
            //simpleEmitted = tuple.Item2;

            ////tuple = RunExpression<Simple>(c => new Simple((Basic)basicEmitted(c)), false, true);
            ////simpleEmitted = tuple.Item2;

            //tuple = RunExpression<Complex>(c => new Complex((Simple) simpleEmitted(c)), false, true);
            //complexEmitted = tuple.Item2;

            Console.WriteLine("End\n");
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
            catch (Exception e)
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
                    Console.WriteLine(typeof(T).Name + " Expression Emitted: " + watch.ElapsedMilliseconds);
            }

            return new Tuple<Func<Context, object>, Func<Context, object>>(expCompiled, ExpEmitted);
        }

        private static Resolver UseResolver<T>(Func<Context, object> func, string kind)
        {
            var resolver = new Resolver(func);
            var ctx = new Context();
            var watch = new Stopwatch();

            watch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                var basic = (T)resolver.Resolve(ctx);
            }
            watch.Stop();
            Console.WriteLine($"{typeof(T).Name} Resolve {kind}:  {watch.ElapsedMilliseconds}");

            return resolver;
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

        public virtual object Resolve(Context ctx) => func(ctx);
    }

    public class ResolverBasic : Resolver
    {
        public ResolverBasic() : base(null)
        {
        }

        public override object Resolve(Context ctx)
        {
            return new Basic();
        }
    }

    public class ResolverSimple : Resolver
    {
        ResolverBasic _resolver;

        public ResolverSimple(ResolverBasic resolver) : base(null)
        {
            _resolver = resolver;
        }

        public override object Resolve(Context ctx)
        {
            return new Simple((Basic)_resolver.Resolve(ctx));
        }
    }

    public class ResolverComplex : Resolver
    {
        ResolverSimple _resolver;

        public ResolverComplex(ResolverSimple resolver) : base(null)
        {
            _resolver = resolver;
        }

        public override object Resolve(Context ctx)
        {
            return new Complex((Simple) _resolver.Resolve(ctx));
        }

        public void Run()
        {
            var ctx = new Context();
            var watch = new Stopwatch();

            watch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                var basic = (Complex)Resolve(ctx);
            }
            watch.Stop();
            Console.WriteLine($"Manual Complex: {watch.ElapsedMilliseconds}");
        }
    }
}
