// MIT License
//
// Copyright (c) 2017 Maarten van Sambeek.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace ConnectQl.Internal.Query.Plans
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The declare variable query plan.
    /// </summary>
    internal class DeclareVariableQueryPlan : IQueryPlan
    {
        /// <summary>
        /// The <see cref="IInternalExecutionContext.SetVariable{T}"/> method.
        /// </summary>
        private static readonly MethodInfo SetVariable = typeof(IInternalExecutionContext).GetGenericMethod(nameof(IInternalExecutionContext.SetVariable), typeof(string), null);

        /// <summary>
        /// The setter.
        /// </summary>
        private readonly Func<IInternalExecutionContext, Task> evaluateVariable;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeclareVariableQueryPlan"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public DeclareVariableQueryPlan(string name, Expression expression)
        {
            var context = Expression.Parameter(typeof(IInternalExecutionContext), "context");

            expression = GenericVisitor.Visit(
                (ExecutionContextExpression e) => context,
                Expression.Call(context, DeclareVariableQueryPlan.SetVariable.MakeGenericMethod(expression.Type), Expression.Constant(name), expression).RewriteTasksToAsyncExpression());

            if (!expression.Type.IsConstructedGenericType || expression.Type.GetGenericTypeDefinition() != typeof(Task<>))
            {
                var setVariable = Expression.Lambda<Action<IInternalExecutionContext>>(expression, context).Compile();

                this.evaluateVariable = ctx =>
                    {
                        setVariable(ctx);
                        return Task.FromResult(true);
                    };
            }
            else
            {
                this.evaluateVariable = Expression.Lambda<Func<IExecutionContext, Task>>(expression, context).Compile();
            }
        }

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [ItemNotNull]
        public async Task<ExecuteResult> ExecuteAsync(IInternalExecutionContext context)
        {
            await this.evaluateVariable(context);

            return new ExecuteResult();
        }
    }
}