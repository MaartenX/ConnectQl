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

namespace ConnectQl.Expressions.Visitors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// The generic visitor.
    /// </summary>
    public class GenericVisitor : ExpressionVisitor, IEnumerable<object>
    {
        /// <summary>
        /// The implementations.
        /// </summary>
        private readonly Dictionary<Type, Func<Expression, Expression>> implementations = new Dictionary<Type, Func<Expression, Expression>>();

        /// <summary>
        /// Stores the visited nodes.
        /// </summary>
        private readonly HashSet<Expression> visitedNodes = new HashSet<Expression>();

        /// <summary>
        /// The default implementation.
        /// </summary>
        private Func<Expression, Expression> defaultImplementation;

        /// <summary>
        /// The visit catch block.
        /// </summary>
        private Func<CatchBlock, CatchBlock> visitCatchBlock;

        /// <summary>
        /// The function for visiting the <see cref="ElementInit"/> expression.
        /// </summary>
        private Func<ElementInit, ElementInit> visitElementInit;

        /// <summary>
        /// The visit label target.
        /// </summary>
        private Func<LabelTarget, LabelTarget> visitLabelTarget;

        /// <summary>
        /// The visit member assignment.
        /// </summary>
        private Func<MemberAssignment, MemberAssignment> visitMemberAssignment;

        /// <summary>
        /// The visit member binding.
        /// </summary>
        private Func<MemberBinding, MemberBinding> visitMemberBinding;

        /// <summary>
        /// The visit member list binding.
        /// </summary>
        private Func<MemberListBinding, MemberListBinding> visitMemberListBinding;

        /// <summary>
        /// The visit member member binding.
        /// </summary>
        private Func<MemberMemberBinding, MemberMemberBinding> visitMemberMemberBinding;

        /// <summary>
        /// The visit switch case.
        /// </summary>
        private Func<SwitchCase, SwitchCase> visitSwitchCase;

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/> with the specified action.
        /// </summary>
        /// <param name="action">
        /// The action to add to the visitor.
        /// </param>
        /// <typeparam name="T">
        /// The type of the function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [NotNull]
        public static GenericVisitor Create<T>(Func<T, Expression> action)
            where T : Expression
        {
            return new GenericVisitor
                       {
                           action,
                       };
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/> with the specified action.
        /// </summary>
        /// <param name="firstAction">
        /// The first action to add to the visitor.
        /// </param>
        /// <param name="secondAction">
        /// The second action to add to the visitor.
        /// </param>
        /// <typeparam name="T1">
        /// The type of the first function parameter.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [NotNull]
        public static GenericVisitor Create<T1, T2>(Func<T1, Expression> firstAction, Func<T2, Expression> secondAction)
            where T1 : Expression
            where T2 : Expression
        {
            return new GenericVisitor
                       {
                           firstAction,
                           secondAction,
                       };
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/> with the specified action.
        /// </summary>
        /// <param name="firstAction">
        /// The first action to add to the visitor.
        /// </param>
        /// <param name="secondAction">
        /// The second action to add to the visitor.
        /// </param>
        /// <param name="thirdAction">
        /// The third action to add to the visitor.
        /// </param>
        /// <typeparam name="T1">
        /// The type of the first function parameter.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second function parameter.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [NotNull]
        public static GenericVisitor Create<T1, T2, T3>(Func<T1, Expression> firstAction, Func<T2, Expression> secondAction, Func<T3, Expression> thirdAction)
            where T1 : Expression
            where T2 : Expression
            where T3 : Expression
        {
            return new GenericVisitor
                       {
                           firstAction,
                           secondAction,
                           thirdAction,
                       };
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/> with the specified action.
        /// </summary>
        /// <param name="action">
        /// The action to add to the visitor.
        /// </param>
        /// <typeparam name="T">
        /// The type of the function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [NotNull]
        public static GenericVisitor Create<T>(Action<T> action)
            where T : Expression
        {
            return new GenericVisitor
                       {
                           action,
                       };
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/> with the specified action.
        /// </summary>
        /// <param name="firstAction">
        /// The first action to add to the visitor.
        /// </param>
        /// <param name="secondAction">
        /// The second action to add to the visitor.
        /// </param>
        /// <typeparam name="T1">
        /// The type of the first function parameter.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [NotNull]
        public static GenericVisitor Create<T1, T2>(Action<T1> firstAction, Action<T2> secondAction)
            where T1 : Expression
            where T2 : Expression
        {
            return new GenericVisitor
                       {
                           firstAction,
                           secondAction,
                       };
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/> with the specified action.
        /// </summary>
        /// <param name="firstAction">
        /// The first action to add to the visitor.
        /// </param>
        /// <param name="secondAction">
        /// The second action to add to the visitor.
        /// </param>
        /// <param name="thirdAction">
        /// The third action to add to the visitor.
        /// </param>
        /// <typeparam name="T1">
        /// The type of the first function parameter.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second function parameter.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [NotNull]
        public static GenericVisitor Create<T1, T2, T3>(Action<T1> firstAction, Action<T2> secondAction, Action<T3> thirdAction)
            where T1 : Expression
            where T2 : Expression
            where T3 : Expression
        {
            return new GenericVisitor
                       {
                           firstAction,
                           secondAction,
                           thirdAction,
                       };
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/>, visits the <paramref name="expression"/> and returns the result.
        /// </summary>
        /// <param name="action">
        /// The action to add to the visitor.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T">
        /// The type of the function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression Visit<T>(Func<T, Expression> action, Expression expression)
            where T : Expression
        {
            return new GenericVisitor
                       {
                           action,
                       }.Visit(expression);
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/>, visits the <paramref name="expression"/> and returns the result.
        /// </summary>
        /// <param name="firstAction">
        /// The first action to add to the visitor.
        /// </param>
        /// <param name="secondAction">
        /// The second action to add to the visitor.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T1">
        /// The type of the first function parameter.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression Visit<T1, T2>(Func<T1, Expression> firstAction, Func<T2, Expression> secondAction, Expression expression)
            where T1 : Expression
            where T2 : Expression
        {
            return new GenericVisitor
                       {
                           firstAction,
                           secondAction,
                       }.Visit(expression);
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/>, visits the <paramref name="expression"/> and returns the result.
        /// </summary>
        /// <param name="firstAction">
        /// The first action to add to the visitor.
        /// </param>
        /// <param name="secondAction">
        /// The second action to add to the visitor.
        /// </param>
        /// <param name="thirdAction">
        /// The third action to add to the visitor.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T1">
        /// The type of the first function parameter.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second function parameter.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression Visit<T1, T2, T3>(Func<T1, Expression> firstAction, Func<T2, Expression> secondAction, Func<T3, Expression> thirdAction, Expression expression)
            where T1 : Expression
            where T2 : Expression
            where T3 : Expression
        {
            return new GenericVisitor
                       {
                           firstAction,
                           secondAction,
                           thirdAction,
                       }.Visit(expression);
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/>, visits the <paramref name="expression"/> and returns the result.
        /// </summary>
        /// <param name="action">
        /// The action to add to the visitor.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T">
        /// The type of the function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression Visit<T>(Action<T> action, Expression expression)
            where T : Expression
        {
            return new GenericVisitor
                       {
                           action,
                       }.Visit(expression);
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/>, visits the <paramref name="expression"/> and returns the result.
        /// </summary>
        /// <param name="firstAction">
        /// The first action to add to the visitor.
        /// </param>
        /// <param name="secondAction">
        /// The second action to add to the visitor.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T1">
        /// The type of the first function parameter.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression Visit<T1, T2>(Action<T1> firstAction, Action<T2> secondAction, Expression expression)
            where T1 : Expression
            where T2 : Expression
        {
            return new GenericVisitor
                       {
                           firstAction,
                           secondAction,
                       }.Visit(expression);
        }

        /// <summary>
        /// Creates a <see cref="GenericVisitor"/>, visits the <paramref name="expression"/> and returns the result.
        /// </summary>
        /// <param name="firstAction">
        /// The first action to add to the visitor.
        /// </param>
        /// <param name="secondAction">
        /// The second action to add to the visitor.
        /// </param>
        /// <param name="thirdAction">
        /// The third action to add to the visitor.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T1">
        /// The type of the first function parameter.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second function parameter.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third function parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression Visit<T1, T2, T3>(Action<T1> firstAction, Action<T2> secondAction, Action<T3> thirdAction, Expression expression)
            where T1 : Expression
            where T2 : Expression
            where T3 : Expression
        {
            return new GenericVisitor
                       {
                           firstAction,
                           secondAction,
                           thirdAction,
                       }.Visit(expression);
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="catchBlock">
        /// The catch block.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add(Func<CatchBlock, CatchBlock> catchBlock)
        {
            this.visitCatchBlock = catchBlock;

            return this;
        }

        /// <summary>
        /// Adds an <see cref="ElementInit"/> handler to the visitor.
        /// </summary>
        /// <param name="elementInit">
        /// The <see cref="ElementInit"/> expression.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add(Func<ElementInit, ElementInit> elementInit)
        {
            this.visitElementInit = elementInit;

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="labelTarget">
        /// The label target.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add(Func<LabelTarget, LabelTarget> labelTarget)
        {
            this.visitLabelTarget = labelTarget;

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="memberAssignment">
        /// The member assignment.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add(Func<MemberAssignment, MemberAssignment> memberAssignment)
        {
            this.visitMemberAssignment = memberAssignment;

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="memberBinding">
        /// The member binding.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add(Func<MemberBinding, MemberBinding> memberBinding)
        {
            this.visitMemberBinding = memberBinding;

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="memberListBinding">
        /// The member list binding.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add(Func<MemberListBinding, MemberListBinding> memberListBinding)
        {
            this.visitMemberListBinding = memberListBinding;

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="memberMemberBinding">
        /// The member member binding.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add(Func<MemberMemberBinding, MemberMemberBinding> memberMemberBinding)
        {
            this.visitMemberMemberBinding = memberMemberBinding;

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="switchCase">
        /// The switch case.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add(Func<SwitchCase, SwitchCase> switchCase)
        {
            this.visitSwitchCase = switchCase;

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <typeparam name="T">
        /// The type of the expression this implementation is for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add<T>(Action<T> implementation)
            where T : Expression
        {
            this.implementations[typeof(T)] = e =>
                {
                    implementation((T)e);
                    return null;
                };

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <typeparam name="T">
        /// The type of the expression this implementation is for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add<T>(Func<T, Expression> implementation)
            where T : Expression
        {
            this.implementations[typeof(T)] = e => implementation((T)e);

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <typeparam name="T">
        /// The type of the expression this implementation is for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add<T>(Action<GenericVisitor, T> implementation)
            where T : Expression
        {
            this.implementations[typeof(T)] = e =>
                {
                    implementation(this, (T)e);
                    return null;
                };

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <typeparam name="T">
        /// The type of the expression this implementation is for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Add<T>(Func<GenericVisitor, T, Expression> implementation)
            where T : Expression
        {
            this.implementations[typeof(T)] = e => implementation(this, (T)e);

            return this;
        }

        /// <summary>
        /// Registers a function to call when no other handler is registered.
        /// </summary>
        /// <param name="implementation">
        /// The handler to register.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Default(Action<Expression> implementation)
        {
            this.defaultImplementation = node =>
                {
                    if (this.visitedNodes.Add(node))
                    {
                        implementation(node);
                        this.visitedNodes.Remove(node);
                    }

                    return null;
                };

            return this;
        }

        /// <summary>
        /// Registers a function to call when no other handler is registered.
        /// </summary>
        /// <param name="implementation">
        /// The handler to register.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        [NotNull]
        public GenericVisitor Default(Func<Expression, Expression> implementation)
        {
            this.defaultImplementation = node =>
                {
                    Expression result = null;

                    if (this.visitedNodes.Add(node))
                    {
                        result = implementation(node);
                        this.visitedNodes.Remove(node);
                    }

                    return result;
                };

            return this;
        }

        /// <summary>
        /// Registers a function to call when no other handler is registered.
        /// </summary>
        /// <param name="implementation">
        /// The handler to register.
        /// </param>
        /// <returns>
        /// The <see cref="GenericVisitor"/>.
        /// </returns>
        public GenericVisitor Default(Func<GenericVisitor, Expression, Expression> implementation)
        {
            return this.Default(e => implementation(this, e));
        }

        /// <summary>
        /// Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        public override Expression Visit(Expression node)
        {
            return this.VisitImplementation(node) ?? base.Visit(node);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection. Only here because we need it to use the collection
        ///     initializer.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the
        ///     collection.
        /// </returns>
        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return Enumerable.Empty<object>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<object>)this).GetEnumerator();
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitBinary(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.BlockExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitBlock(BlockExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitBlock(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.CatchBlock"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return this.visitCatchBlock?.Invoke(node) ?? base.VisitCatchBlock(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.ConditionalExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitConditional(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ConstantExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitConstant(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.DebugInfoExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitDebugInfo(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.DefaultExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitDefault(DefaultExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitDefault(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.ElementInit"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override ElementInit VisitElementInit(ElementInit node)
        {
            return this.visitElementInit?.Invoke(node) ?? base.VisitElementInit(node);
        }

        /// <summary>
        /// Visits the children of the extension expression.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitExtension(Expression node)
        {
            return this.VisitImplementation(node) ?? (this.implementations.TryGetValue(node.GetType(), out var implementation)
                                              ? (implementation(node) ?? base.VisitExtension(node))
                                              : base.VisitExtension(node));
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.GotoExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitGoto(GotoExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitGoto(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.IndexExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitIndex(IndexExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitIndex(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.InvocationExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitInvocation(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.LabelExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitLabel(LabelExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitLabel(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.LabelTarget"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return this.visitLabelTarget?.Invoke(node) ?? base.VisitLabelTarget(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.Expression`1"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        /// <typeparam name="T">
        /// The type of the delegate.
        /// </typeparam>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return this.VisitImplementation(node) ?? base.VisitLambda(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.ListInitExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitListInit(ListInitExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitListInit(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.LoopExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitLoop(LoopExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitLoop(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitMember(MemberExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitMember(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberAssignment"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return this.visitMemberAssignment?.Invoke(node) ?? base.VisitMemberAssignment(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberBinding"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return this.visitMemberBinding?.Invoke(node) ?? base.VisitMemberBinding(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberInitExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitMemberInit(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberListBinding"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            return this.visitMemberListBinding?.Invoke(node) ?? base.VisitMemberListBinding(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberMemberBinding"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return this.visitMemberMemberBinding.Invoke(node) ?? base.VisitMemberMemberBinding(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitMethodCall(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.NewExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitNew(NewExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitNew(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.NewArrayExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitNewArray(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ParameterExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitParameter(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitRuntimeVariables(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.SwitchExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitSwitch(SwitchExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitSwitch(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.SwitchCase"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            return this.visitSwitchCase?.Invoke(node) ?? base.VisitSwitchCase(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.TryExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitTry(TryExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitTry(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.TypeBinaryExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitTypeBinary(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-Expression was modified; otherwise, returns the original
        ///     Expression.
        /// </returns>
        /// <param name="node">
        /// The expression to visit.
        /// </param>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitUnary(node);
        }

        /// <summary>
        /// Visits the specified node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <typeparam name="T">
        /// The type of the expression.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [CanBeNull]
        private Expression VisitImplementation<T>(T node)
            where T : Expression
        {
            return this.implementations.TryGetValue(typeof(T), out var implementation) ? implementation(node) : this.defaultImplementation?.Invoke(node);
        }

        /// <summary>
        /// Visits the specified node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [CanBeNull]
        private Expression VisitImplementation([CanBeNull] Expression node)
        {
            if (node == null)
            {
                return null;
            }

            return this.implementations.TryGetValue(node.GetType(), out var implementation) ? implementation(node) : this.defaultImplementation?.Invoke(node);
        }
    }
}