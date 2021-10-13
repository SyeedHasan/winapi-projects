using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

// Author: JFLAM

namespace InvokeDynamic
{
    class MyTarget
    {
        public int Foo() { return 1; }
        public int Foo(int x) { return x; }
        public int Foo(int x, string y) { return x; }
    }

    class Program
    {
        static class CallSites
        {
            public static CallSite<Func<CallSite, object, int, object>> Site1;
        }

        // Use C# compiler to generate the code
        static int InvokeDynamic()
        {
            dynamic t = new MyTarget();
            return Convert.ToInt32(t.Foo(42));
        }

        // Explicitly generate the code
        static int InvokeDirect()
        {
            Console.WriteLine(CallSites.Site1);
            if (CallSites.Site1 == null)
            {
                var arg1 = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
                var arg2 = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.UseCompileTimeType, null);
                CallSiteBinder csb = Binder.InvokeMember(CSharpBinderFlags.None, "Foo", null, typeof(Program), new[] { arg1, arg2 });
                CallSites.Site1 = CallSite<Func<CallSite, object, int, object>>.Create(csb);
            }
            return Convert.ToInt32(CallSites.Site1.Target(CallSites.Site1, new MyTarget(), 42));
        }

        // Use ET compiler to generate the code
        static int InvokeCompiler()
        {
            Expression<Func<MyTarget, int>> func = t => t.Foo(42);
            return func.Compile()(new MyTarget());
        }

        // A more dynamic use of the ET compiler to generate the code
        static int InvokeCompiler2(MyTarget t, string methodName, int p1)
        {
            var arg1 = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
            var arg2 = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.UseCompileTimeType, null);
            CallSiteBinder csb = Binder.InvokeMember(CSharpBinderFlags.None, methodName, null, typeof(Program), new[] { arg1, arg2 });
            // Q: why does the InvokeMember method always assume that the result is an object? Why does C# generate another call site to do the conversion from object to int?
            var de = Expression.Dynamic(csb, typeof(object), Expression.Constant(t), Expression.Constant(p1));
            Expression<Func<MyTarget, object>> expr = Expression.Lambda<Func<MyTarget, object>>(de, Expression.Parameter(typeof(MyTarget)));
            return Convert.ToInt32(expr.Compile()(t));
        }

        // Another level of dynamism
        static int InvokeCompiler3(object target, string methodName, object p1)
        {
            var arg1 = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
            var arg2 = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.UseCompileTimeType, null);
            CallSiteBinder csb = Binder.InvokeMember(CSharpBinderFlags.None, methodName, null, typeof(Program), new[] { arg1, arg2 });
            var de = Expression.Dynamic(csb, typeof(object), Expression.Constant(target), Expression.Constant(p1));
            LambdaExpression expr = Expression.Lambda(de);
            return Convert.ToInt32(expr.Compile().DynamicInvoke());
        }

        // Pretty damn dynamic
        static int InvokeCompiler4(object target, string methodName, params object[] args)
        {
            var targetParam = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
            CSharpArgumentInfo[] parameterFlags = new CSharpArgumentInfo[args.Length + 1];
            Expression[] parameters = new Expression[args.Length + 1];
            parameterFlags[0] = targetParam;
            parameters[0] = Expression.Constant(target);
            for (int i = 0; i < args.Length; i++)
            {
                parameterFlags[i + 1] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.UseCompileTimeType, null);
                parameters[i + 1] = Expression.Constant(args[i]);
            }
            CallSiteBinder csb = Binder.InvokeMember(CSharpBinderFlags.None, methodName, null, typeof(Program), parameterFlags);
            var de = Expression.Dynamic(csb, typeof(object), parameters);
            LambdaExpression expr = Expression.Lambda(de);
            return Convert.ToInt32(expr.Compile().DynamicInvoke());
        }
        static void Main(string[] args)
        {
            //Console.WriteLine(InvokeDynamic());
            Console.WriteLine(InvokeDirect());
            //Console.WriteLine(InvokeCompiler());
            //Console.WriteLine(InvokeCompiler2(new MyTarget(), "Foo", 42));
            //Console.WriteLine(InvokeCompiler3(new MyTarget(), "Foo", 42));
            //Console.WriteLine(InvokeCompiler4(new MyTarget(), "Foo", 42));
            //Console.WriteLine(InvokeCompiler4(new MyTarget(), "Foo", 43, "mom"));
        }
    }
}
