using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectQl.Tests
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal;
    using ConnectQl.Parser;
    using ConnectQl.Parser.Ast.Statements;
    using ConnectQl.Query;
    using ConnectQl.Query.Factories;
    using ConnectQl.Results;
    using ConnectQl.Validation;

    using Xunit;

    public class FactoryBuilderTests
    {
        private static async Task<T> DoTest<T>(T item)
        {
            return item;
        }

        [Fact]
        public async Task Test()
        {
            //"SELECT s.Item, COUNT(s.*), s.Item, COUNT(*), COUNT(s.*) FROM SPLIT('a,b' + ',c,' + 1, ',') s GROUP BY s.Item"
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("SELECT s.Item, s.Item2, s.Item + s.Item2, COUNT(s.first) FROM SPLIT('a,b' + ',c,' + 1, ',') s GROUP BY s.Item, s.Item2 HAVING COUNT(s.first) = 1 ORDER BY COUNT(s.first)")))
            {
                var i = 3;

                Expression<Func<int>> drie = () => i;

                //var param1 = Expr.Parameter<int>("param1");
                //var param2 = Expr.Parameter<int>("param2");

                //var block = Expr.Block(
                //    new Expr[]
                //    {
                //        Expr.Call(DoTest, Expr.Constant(3)),
                //        Expr.Call(DoTest, param1),
                //        Expr.Call(DoTest, param2)
                //    },
                //    Expr.Call(DoTest, param1)).ToFunc().AddArgument(param1).AddArgument(param2);

                //var asyncBlock = block.MakeAsync();

                var result = Expr.Default<int>().Call((o, c) => o.ToString(c), Expr.Default<CultureInfo>());
                var ctx1 = new ConnectQlContext();
                var ctx = ctx1 as IQueryPlanGenerator;
                var plan = await ctx.GetQueryPlanAsync("untitled.connectql", stream);

                new GenericVisitor
                {
                    (ExecutionContextExpression e) =>
                    {
                        
                    },
                    (FieldExpression e) =>
                    {
                        
                    },
                    (RangeExpression e) =>
                    {
                        
                    },
                    (SourceFieldExpression e) =>
                    {
                        
                    },
                    (TaskExpression e) =>
                    {
                        
                    }
                }.Visit(plan);

                var result1 = await plan.Compile()(new ExecutionContextImplementation(ctx1, "untitled.connectql"));

                var sel = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(plan);
                

//#if NET452

//                var builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Test"), AssemblyBuilderAccess.RunAndSave);

//                var module = builder.DefineDynamicModule("Test", "test.dll", true);
//                var type = module.DefineType("ConnectQl.GeneratedQuery", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);

//                var method = type.DefineMethod("ExecuteAsync", MethodAttributes.Public | MethodAttributes.Static, typeof(Task<IAsyncEnumerable<Row>>), new[] { typeof(IExecutionContext) });
//                var generator = DebugInfoGenerator.CreatePdbGenerator();
//                FactoryGenerator.Create(
//                    (Expr<IAsyncEnumerable<Row>>)new Simplifier().Visit(FactoryGenerator.GenerateSelect(context.NodeData, @select))
//                    ).CompileToMethod(method, generator);

//                type.CreateType();

//                builder.Save("test.dll");
                
//                var executeAsync = builder.GetType("ConnectQl.GeneratedQuery").GetMethod("ExecuteAsync");
                
//                var result = await (await (Task<IAsyncEnumerable<Row>>)executeAsync.Invoke(null, new object[] { context })).MaterializeAsync();
//#endif
            }
        }
    }

    
}
