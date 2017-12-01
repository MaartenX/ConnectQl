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

namespace ConnectQl.Query
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Comparers;
    using ConnectQl.DataSources.Joins;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal;
    using ConnectQl.Parser.Ast;
    using ConnectQl.Parser.Ast.Expressions;
    using ConnectQl.Parser.Ast.Sources;
    using ConnectQl.Parser.Ast.Statements;
    using ConnectQl.Parser.Ast.Visitors;
    using ConnectQl.Query.Factories;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The select generator.
    /// </summary>
    internal static class SelectGenerator
    {
        /// <summary>
        /// Generates a expr that returns the queries result.
        /// </summary>
        /// <param name="generator">
        /// The generator.
        /// </param>
        /// <param name="select">
        /// The statement.
        /// </param>
        /// <returns>
        /// The expr.
        /// </returns>
        [NotNull]
        public static Expr<IExecuteResult> GenerateSelect(this FactoryGenerator generator, SelectStatement select)
        {
            return Expr.Create(
                    (a, b) => new ExecuteResult(a, b),
                    Expr.Constant(0L),
                    generator.GenerateSelectInternal(select))
                .ConvertTo<IExecuteResult>();
        }

        /// <summary>
        /// Generates a expr that returns the queries result as an async enumerable of rows.
        /// </summary>
        /// <param name="generator">
        /// The generator.
        /// </param>
        /// <param name="select">
        /// The statement.
        /// </param>
        /// <returns>
        /// The expr.
        /// </returns>
        [CanBeNull]
        internal static Expr<IAsyncEnumerable<Row>> GenerateSelectInternal(this FactoryGenerator generator, SelectStatement select)
        {
            if (select is SelectFromStatement selectFrom)
            {
                Expr<IAsyncEnumerable<Row>> BuildRowsFactory(SelectFromStatement selectFromStatement)
                {
                    var rows = generator.NodeData
                        .ConvertToDataSource(selectFromStatement.Source)
                        .Call(
                            (ds, ctx, query) => ds.GetRows(ctx, query),
                            FactoryGenerator.Context,
                            generator.CreateMultiPartQuery(selectFromStatement));

                    var rowTransform = generator.CreateRowTransform(selectFromStatement.Expressions);
                    
                    return rowTransform.HasTasks
                               ? Expr.Call(AsyncEnumerableExtensions.Select, rows, rowTransform.MakeAsync())
                               : Expr.Call(AsyncEnumerableExtensions.Select, rows, rowTransform);
                }

                if (selectFrom.Groupings.Any())
                {
                    var groupQuery = new GroupQuery(generator.NodeData, selectFrom);
                    
                    var enumerable = Expr.Array(groupQuery.Groupings.Select(g => FactoryGenerator.Row.Call((r, index) => r[index], Expr.Constant(g)))).ToFunc().AddArgument(FactoryGenerator.Row);

                    var p2 = Expr.Call(
                        AsyncEnumerableExtensions.GroupBy,
                        BuildRowsFactory(groupQuery.InnerSelect),
                        enumerable,
                        Expr.Constant<IComparer<object[]>>(ArrayOfObjectComparer.Default));

                    //var fields = node.Expressions.Select(f => f.Expression is WildcardConnectQlExpression ? "*" : f.Alias);

                    //Expr.ReplaceLambdaParameters(
                    //    ())

                    var rows = Expr.ReplaceLambdaParameters(
                        (row, rowBuilder, values) => rowBuilder.CreateRow(row.UniqueId, values),
                        FactoryGenerator.Row,
                        CreateRowBuilderFactory(groupQuery.Expressions),
                        GenerateGroupValues(generator, groupQuery.Expressions)).ToFunc().AddArgument(FactoryGenerator.Rows);

                    var v2 = rows.HasTasks
                               ? Expr.Call(AsyncEnumerableExtensions.Select, p2, rows.MakeAsync())
                               : Expr.Call(AsyncEnumerableExtensions.Select, p2, rows);

                    var fieldReplace = new GenericVisitor
                                       {
                                           (SourceFieldExpression e) => e.CreateGetter(FactoryGenerator.Row)
                                       };

                    if (groupQuery.Having != null)
                    {
                        var where = fieldReplace.Visit((new Expr<bool>(ConnectQlExpression.MakeSourceField(null, "$having", typeof(bool))).ToFunc().AddArgument(FactoryGenerator.Row)));

                        v2 = Expr.Call(AsyncEnumerableExtensions.Where, v2, where);
                    }

                    if (groupQuery.OrderBy.Any())
                    {
                        var order = groupQuery.OrderBy[0];

                        var result =
                            order.Ascending
                                ? Expr.Call(
                                    AsyncEnumerableExtensions.OrderBy,
                                    v2,
                                    fieldReplace.Visit(Expr.Object(generator.NodeData.ConvertToLinqExpression(order.Expression)).ToFunc().AddArgument(FactoryGenerator.Row)),
                                    Expr.Constant((IComparer<object>)null))
                                : Expr.Call(
                                    AsyncEnumerableExtensions.OrderByDescending,
                                    v2,
                                    fieldReplace.Visit(Expr.Object(generator.NodeData.ConvertToLinqExpression(order.Expression)).ToFunc().AddArgument(FactoryGenerator.Row)),
                                    Expr.Constant((IComparer<object>)null));

                        result = groupQuery.OrderBy.Skip(1)
                            .Aggregate(
                                result,
                                (current, then) =>
                                    then.Ascending
                                        ? Expr.Call(AsyncEnumerableExtensions.ThenBy, current, fieldReplace.Visit(Expr.Object(generator.NodeData.ConvertToLinqExpression(order.Expression)).ToFunc().AddArgument(FactoryGenerator.Row)), Expr.Constant((IComparer<object>)null))
                                        : Expr.Call(AsyncEnumerableExtensions.ThenByDescending, current, fieldReplace.Visit(Expr.Object(generator.NodeData.ConvertToLinqExpression(order.Expression)).ToFunc().AddArgument(FactoryGenerator.Row)), Expr.Constant((IComparer<object>)null)));

                        v2 = result.ConvertTo<IAsyncEnumerable<Row>>();
                    }

                    //    var groupQuery = GroupQueryVisitor.GetGroupQuery(node, this.data);
                    //    var groupFactories = this.GetGroupValueFactory(groupQuery.Expressions.Concat(new[]
                    //                                                                                     {
                    //                                                                                         new AliasedConnectQlExpression(groupQuery.Having, "$having"),
                    //                                                                                     }).Concat(groupQuery.OrderBy.Select((o, i) => new AliasedConnectQlExpression(o.Expression, $"$order{i}"))).Where(e => e.Expression != null));
                    //    var plan = this.CreateSelectQueryPlan(groupQuery.RowSelect);
                    //    var fields = node.Expressions.Select(f => f.Expression is WildcardConnectQlExpression ? "*" : f.Alias);
                    //    var orders = groupQuery.OrderBy.Select((o, i) => new OrderByExpression(ConnectQlExpression.MakeSourceField(null, $"$order{i}", true), o.Ascending));
                    //    var having = groupQuery.Having == null ? null : ConnectQlExpression.MakeSourceField(null, "$having", true, typeof(bool));

                    //    var aliases = groupQuery.Expressions;

                    //    this.data.SetQueryPlanExpression(node, new SelectGroupByQueryPlan(plan, groupFactories, groupQuery.Groupings, aliases, having, orders, fields));
                    return v2;
                }
                else
                {
                    return BuildRowsFactory(selectFrom);
                }
            }

            return null;
        }

        //private static string ExpressionToString(Expression expression)
        //{
        //    var ops = new Dictionary<ExpressionType, string> { { ExpressionType.Add, "+" }, { ExpressionType.AddAssign, "+=" } };
        //    var result = new StringBuilder();

        //    void VisitChild(GenericVisitor visitor, Expression parent, Expression child)
        //    {
        //        result.Append("(");
        //        visitor.Visit(child);
        //        result.Append(")");
        //    }

        //    new GenericVisitor
        //    {
        //        (GenericVisitor v, BinaryExpression b) =>
        //        {
        //            VisitChild(v, b, b.Left);
                    
        //            VisitChild(v, b, b.Right);
        //        }
        //    };
        //}

        private static Expr<IEnumerable<KeyValuePair<string, object>>> GenerateGroupValues(this FactoryGenerator generator, [NotNull] IReadOnlyCollection<AliasedConnectQlExpression> expressions)
        {
            var result = Expr.Enumerable(
                expressions.Select(
                    expression =>
                        Expr.Create(
                            (a, b) => new KeyValuePair<string, object>(a, b),
                            Expr.Constant(expression.Alias),
                            Expr.Object(generator.NodeData.ConvertToLinqExpression(expression.Expression)))));

            return GenericVisitor.Visit((SourceFieldExpression e) => e.CreateGroupGetter(FactoryGenerator.Rows), result);
        }

        /// <summary>
        /// Creates a lambda that transforms a row retrieved from the nodeData source into a result row.
        /// </summary>
        /// <param name="generator">
        /// The generator.
        /// </param>
        /// <param name="expressions">
        /// The expressions to add to the result row.
        /// </param>
        /// <returns>
        /// A Expr for a <see cref="Func{Row, Row}"/>.
        /// </returns>
        [NotNull]
        private static Expr<Func<Row, Row>> CreateRowTransform(this FactoryGenerator generator, [NotNull] IReadOnlyCollection<AliasedConnectQlExpression> expressions)
        {
            return Expr.ReplaceLambdaParameters(
                    (row, rowBuilder, values) => rowBuilder.CreateRow(row.UniqueId, values),
                    FactoryGenerator.Row,
                    CreateRowBuilderFactory(expressions),
                    generator.CreateFieldsFactory(expressions))
                .ToFunc()
                .AddArgument(FactoryGenerator.Row);
        }
        
        /// <summary>
        /// Combines all fields and wildcards into a expr that returns an <see cref="IEnumerable{T}"/> of string/object pairs.
        /// </summary>
        /// <param name="generator">
        /// The generator.
        /// </param>
        /// <param name="fields">
        /// The <see cref="AliasedConnectQlExpression"/>s specifying which fields to retrieve.
        /// </param>
        /// <returns>
        /// The expr.
        /// </returns>
        [NotNull]
        private static Expr<IEnumerable<KeyValuePair<string, object>>> CreateFieldsFactory(this FactoryGenerator generator, [NotNull] IReadOnlyCollection<AliasedConnectQlExpression> fields)
        {
            Expr<IEnumerable<KeyValuePair<string, object>>> fieldsExpr = null;
            var mappedFields = new List<Expr<KeyValuePair<string, object>>>();
            var wildcardIndex = 0;

            foreach (var field in fields)
            {
                if (field.Expression is WildcardConnectQlExpression wildcard)
                {
                    fieldsExpr = fieldsExpr
                        .Concat(Expr.Enumerable(mappedFields))
                        .Concat(CreateWildcard(wildcardIndex++, wildcard));

                    mappedFields.Clear();
                }
                else
                {
                    mappedFields.Add(
                        Expr.Create(
                            (a, b) => new KeyValuePair<string, object>(a, b), 
                            Expr.Constant(field.Alias), 
                            Expr.Object(generator.NodeData.ConvertToLinqExpression(field.Expression))));
                }
            }

            if (mappedFields.Any())
            {
                fieldsExpr = fieldsExpr.Concat(Expr.Enumerable(mappedFields));
            }

            var result = fieldsExpr ?? Expr.Enumerable(Enumerable.Empty<Expr<KeyValuePair<string, object>>>());

            return GenericVisitor.Visit((SourceFieldExpression e) => e.CreateGetter(FactoryGenerator.Row), result);
        }

        /// <summary>
        /// Creates a expr for the fields retrieved by a wildcard.
        /// </summary>
        /// <param name="index">
        /// The id of the wildcard.
        /// </param>
        /// <param name="wildcard">
        /// The wildcard expression.
        /// </param>
        /// <returns>
        /// The expr.
        /// </returns>
        [NotNull]
        private static Expr<IEnumerable<KeyValuePair<string, object>>> CreateWildcard(int index, [NotNull] WildcardConnectQlExpression wildcard)
        {
            if (string.IsNullOrEmpty(wildcard.Source))
            {
                return Expr.ReplaceLambdaParameters(
                    (row, idx) =>
                        row.ToDictionary()
                            .Select(kv => new KeyValuePair<string, object>(string.Concat(index, kv.Key), kv.Value)),
                    FactoryGenerator.Row,
                    Expr.Constant(index + "!"));
            }

            return Expr.ReplaceLambdaParameters(
                (row, sourcePrefix, idx) => row.ToDictionary()
                    .Where(kv => kv.Key.StartsWith(sourcePrefix))
                    .Select(kv => new KeyValuePair<string, object>(string.Concat(index, kv.Key), kv.Value)),
                FactoryGenerator.Row,
                Expr.Constant(wildcard.Source + "."),
                Expr.Constant(index + "!"));
        }

        /// <summary>
        /// Creates a expr that returns a new <see cref="RowBuilder"/>
        /// </summary>
        /// <param name="fields">
        /// The filed names that will be mapped to in the row builder.
        /// </param>
        /// <returns>
        /// The row builder.
        /// </returns>
        [NotNull]
        private static Expr<RowBuilder> CreateRowBuilderFactory([NotNull] IEnumerable<AliasedConnectQlExpression> fields)
        {
            var fieldNames = fields.Select(f => Expr.Constant(f.Expression is WildcardConnectQlExpression ? "*" : f.Alias));

            return Expr.Create(fm => new RowBuilder(fm), Expr.Create(fn => new FieldMapping(fn), Expr.Enumerable(fieldNames)));
        }

        /// <summary>
        /// Gets all used fields from a <see cref="SelectFromStatement"/>, ignoring all fields that are
        /// already retrieved by wildcards.
        /// </summary>
        /// <param name="selectFrom">
        /// The statement.
        /// </param>
        /// <param name="wildcards">
        /// The aliases of the wildcards in the query.
        /// </param>
        /// <returns>
        /// The fields.
        /// </returns>
        [NotNull]
        private static IEnumerable<Field> GetUsedFields([NotNull] SelectFromStatement selectFrom, HashSet<string> wildcards)
        {
            var fields = new HashSet<Field>();

            new GenericNodeVisitor
            {
                (FieldReferenceConnectQlExpression e) =>
                {
                    if (!wildcards.Contains(e.Source))
                    {
                        fields.Add(new Field(e.Source, e.Name));
                    }
                }
            }.Visit(selectFrom.Expressions);

            return fields;
        }

        /// <summary>
        /// Gets all sources that use a wildcard from the SELECT statement.
        /// </summary>
        /// <param name="selectFrom">The SELECT statement.</param>
        /// <returns>A <see cref="HashSet{T}"/> that holds all sources that use a wildcard in the query.</returns>
        [NotNull]
        private static HashSet<string> GetUsedWildcards(SelectFromStatement selectFrom)
        {
            var wildcards = new HashSet<string>();
            var aliases = new HashSet<string>();
            var globalWildcard = false;

            new GenericNodeVisitor
            {
                (FunctionSource n) => aliases.Add(n.Alias),
                (SelectSource n) => aliases.Add(n.Alias),
                (WildcardConnectQlExpression n) =>
                {
                    if (n.Source != null)
                    {
                        wildcards.Add(n.Source);
                    }
                    else
                    {
                        globalWildcard = true;
                    }
                }
            }.Visit(selectFrom);

            return globalWildcard ? aliases : wildcards;
        }

        /// <summary>
        /// Creates a expr that returns a <see cref="IMultiPartQuery"/>.
        /// </summary>
        /// <param name="generator">
        /// The generator.
        /// </param>
        /// <param name="select">
        /// The <see cref="SelectFromStatement"/> to base the query on.
        /// </param>
        /// <returns>
        /// A expr that creates a <see cref="IMultiPartQuery"/>.
        /// </returns>
        [NotNull]
        private static Expr<IMultiPartQuery> CreateMultiPartQuery([NotNull] this FactoryGenerator generator, [NotNull] SelectFromStatement select)
        {
            var wildcards = GetUsedWildcards(select);
            var fields = GetUsedFields(select, wildcards);

            return Expr.Constant(
                new MultiPartQuery
                {
                    Fields = fields,
                    WildcardAliases = wildcards,
                    FilterExpression = generator.NodeData.ConvertToLinqExpression(select.Where, false),
                    OrderByExpressions = select.Orders.Select(o => new OrderByExpression(generator.NodeData.ConvertToLinqExpression(o.Expression, false), o.Ascending)).ToArray(),
                }).ConvertTo<IMultiPartQuery>();
        }
    }
}
