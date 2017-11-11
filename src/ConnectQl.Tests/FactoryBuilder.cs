using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectQl.Tests
{
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
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
        [Fact]
        public async Task Test()
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("SELECT s.Item, s.*, s.Item, *, s.* FROM SPLIT('a,b' + ',c,' + 1, ',') s")))
            {
                var context = new ExecutionContextImplementation(new ConnectQlContext(), "file");
                var parser = new ConnectQlParser(new ConnectQlScanner(stream), context.NodeData, new MessageWriter("file"));

                parser.Parse();

                var select = parser.Statements.OfType<SelectFromStatement>().First();

                select = Validator.Validate(context, select);

                var sel = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((Expression)FactoryBuilder.CreateSelect(context.NodeData, @select));
#if NET452

                var builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Test"), AssemblyBuilderAccess.RunAndSave);

                var module = builder.DefineDynamicModule("Test", "test.dll", true);
                var type = module.DefineType("GeneratedQuery.Query", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);

                var method = type.DefineMethod("ExecuteAsync", MethodAttributes.Public | MethodAttributes.Static, typeof(Task<IAsyncEnumerable<Row>>), new[] { typeof(IExecutionContext) });
                var generator = DebugInfoGenerator.CreatePdbGenerator();
                FactoryBuilder.Create(
                    (Factory<IAsyncEnumerable<Row>>)new Simplifier().Visit(FactoryBuilder.CreateSelect(context.NodeData, @select))
                    ).CompileToMethod(method, generator);

                type.CreateType();

                builder.Save("test.dll");
                
                var executeAsync = builder.GetType("GeneratedQuery.Query").GetMethod("ExecuteAsync");
                
                var result = await (await (Task<IAsyncEnumerable<Row>>)executeAsync.Invoke(null, new object[] { context })).MaterializeAsync();

#endif
            }
        }
    }

    
}
