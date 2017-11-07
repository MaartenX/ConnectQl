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

namespace ConnectQl.ExtensionMethods
{
    using System;
    using System.Text;

    using ConnectQl.Interfaces;
    using ConnectQl.Parser.Ast;
    using ConnectQl.Parser.Ast.Expressions;
    using ConnectQl.Parser.Ast.Sources;
    using ConnectQl.Parser.Ast.Statements;
    using ConnectQl.Parser.Ast.Targets;
    using ConnectQl.Parser.Ast.Visitors;

    using JetBrains.Annotations;

    /// <summary>
    /// The node extensions.
    /// </summary>
    internal static class NodeExtensions
    {
        /// <summary>
        /// The get display.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="indentSize">
        /// The indent size.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDisplay(this Node node, int indentSize = 4)
        {
            var converter = new NodeToStringConverter(indentSize);

            converter.Visit(node);

            return converter.Builder.ToString();
        }

        /// <summary>
        /// The async node to string converter.
        /// </summary>
        private class NodeToStringConverter : NodeVisitor
        {
            /// <summary>
            /// The indent size.
            /// </summary>
            private readonly int indentSize;

            /// <summary>
            /// The indent.
            /// </summary>
            private int indent;

            /// <summary>
            /// Initializes a new instance of the <see cref="NodeToStringConverter"/> class.
            /// </summary>
            /// <param name="indentSize">
            /// The indent size.
            /// </param>
            public NodeToStringConverter(int indentSize)
            {
                this.indentSize = indentSize;
            }

            /// <summary>
            /// Gets the builder.
            /// </summary>
            public StringBuilder Builder { get; } = new StringBuilder();

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.AliasedConnectQlExpression"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitAliasedSqlExpression(AliasedConnectQlExpression node)
            {
                this.Visit(node.Expression);

                if (node.Alias != null)
                {
                    this.Builder.Append($" AS {node.Alias}");
                }

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Sources.ApplySource"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitApplySource(ApplySource node)
            {
                this.Visit(node.Left);

                this.Builder.Append("  " + (node.IsOuterApply ? "OUTER" : "CROSS") + " ");

                this.Visit(node.Right);

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.BinaryConnectQlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitBinarySqlExpression(BinaryConnectQlExpression node)
            {
                this.Builder.Append("(");

                this.Visit(node.First);

                this.Builder.Append($" {node.Op} ");

                this.Visit(node.Second);

                this.Builder.Append(")");

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.Block"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitBlock(Block node)
            {
                foreach (var statement in node.Statements)
                {
                    this.Visit(statement);

                    this.Builder.Append($"{Environment.NewLine}{Environment.NewLine}");
                }

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.ConstConnectQlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitConstSqlExpression(ConstConnectQlExpression node)
            {
                this.Builder.Append((object)(node.Value == null ? "NULL" : node.Value is string ? $"'{node.Value}'" : node.Value));

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.DeclareJobStatement"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitDeclareJobStatement(DeclareJobStatement node)
            {
                this.Builder.Append($"DECLARE JOB {node.Name}");

                return node;
            }

            /// <summary>
            /// Visits a declare statement.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitDeclareStatement(DeclareStatement node)
            {
                this.Builder.Append("DECLARE ");

                for (var i = 0; i < node.Declarations.Count; i++)
                {
                    this.Visit(node.Declarations[i]);

                    if (i < node.Declarations.Count - 1)
                    {
                        this.Builder.Append(", ");
                    }
                }

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.FieldReferenceConnectQlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitFieldReferenceSqlExpression(FieldReferenceConnectQlExpression node)
            {
                this.Builder.Append(node.Source == null ? $"[{node.Name}]" : $"[{node.Source}].[{node.Name}]");

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.FunctionCallConnectQlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitFunctionCallSqlExpression(FunctionCallConnectQlExpression node)
            {
                this.Builder.Append($"{node.Name?.ToUpperInvariant()}(");

                var first = true;

                foreach (var argument in node.Arguments)
                {
                    if (!first)
                    {
                        this.Builder.Append(", ");
                    }

                    first = false;

                    this.Visit(argument);
                }

                this.Builder.Append(")");

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Sources.FunctionSource"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitFunctionSource(FunctionSource node)
            {
                this.Visit(node.Function);

                this.Builder.Append($" {node.Alias}");

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Targets.FunctionTarget"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitFunctionTarget(FunctionTarget node)
            {
                this.Visit(node.Function);

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.ImportStatement"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitImportStatement(ImportStatement node)
            {
                this.Builder.Append($"IMPORT '{node.Uri}'");

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.InsertStatement"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitInsertStatement(InsertStatement node)
            {
                this.Builder.Append((node.Upsert ? "UPSERT" : "INSERT") + $" INTO{this.NewLine()}{this.Indent()}");

                this.Visit(node.Target);

                this.Builder.Append($"{this.UnIndent()}{this.NewLine()}");

                this.Visit(node.Select);

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Sources.JoinSource"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitJoinSource(JoinSource node)
            {
                this.Visit(node.First);

                switch (node.JoinType)
                {
                    case JoinType.Cross:
                        this.Builder.Append($"{this.NewLine()}{this.Indent()}CROSS JOIN ");
                        break;
                    case JoinType.Inner:
                        this.Builder.Append($"{this.NewLine()}{this.Indent()}INNER JOIN ");
                        break;
                    case JoinType.Left:
                        this.Builder.Append($"{this.NewLine()}{this.Indent()}LEFT JOIN ");
                        break;
                    case JoinType.NearestInner:
                        this.Builder.Append($"{this.NewLine()}{this.Indent()}NEAREST INNER JOIN ");
                        break;
                    case JoinType.NearestLeft:
                        this.Builder.Append($"{this.NewLine()}{this.Indent()}NEAREST LEFT JOIN ");
                        break;
                    case JoinType.SequentialInner:
                        this.Builder.Append($"{this.NewLine()}{this.Indent()}SEQUENTIAL INNER JOIN ");
                        break;
                    case JoinType.SequentialLeft:
                        this.Builder.Append($"{this.NewLine()}{this.Indent()}SEQUENTIAL LEFT JOIN ");
                        break;
                }

                this.Visit(node.Second);

                switch (node.JoinType)
                {
                    case JoinType.Left:
                    case JoinType.Inner:
                    case JoinType.NearestInner:
                    case JoinType.NearestLeft:
                        this.Builder.Append($"{this.NewLine()}{this.Indent()}ON ");
                        this.Visit(node.Expression);
                        this.Builder.Append($"{this.UnIndent()}{this.UnIndent()}");
                        break;
                    case JoinType.Cross:
                    case JoinType.SequentialInner:
                    case JoinType.SequentialLeft:
                        this.Builder.Append($"{this.UnIndent()}");
                        break;
                }

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.OrderByConnectQlExpression"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitOrderBySqlExpression(OrderByConnectQlExpression node)
            {
                this.Visit(node.Expression);

                this.Builder.Append(" " + (node.Ascending ? "ASC" : "DESC"));

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.SelectFromStatement"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitSelectFromStatement(SelectFromStatement node)
            {
                this.Builder.Append($"SELECT{this.NewLine()}{this.Indent()}");

                for (var i = 0; i < node.Expressions.Count; i++)
                {
                    this.Visit(node.Expressions[i]);

                    if (i < node.Expressions.Count - 1)
                    {
                        this.Builder.Append($",{this.NewLine()}");
                    }
                    else
                    {
                        this.Builder.Append(this.UnIndent() + this.NewLine());
                    }
                }

                this.Builder.Append($"FROM{this.NewLine()}{this.Indent()}");

                this.Visit(node.Source);

                this.UnIndent();

                if (node.Where != null)
                {
                    this.Builder.Append($"{this.NewLine()}WHERE ");
                    this.Visit(node.Where);
                }

                if (node.Groupings.Count > 0)
                {
                    this.Builder.Append($"{this.NewLine()}GROUP BY{this.NewLine() + this.Indent()}");

                    for (var i = 0; i < node.Groupings.Count; i++)
                    {
                        this.Visit(node.Groupings[i]);

                        if (i < node.Groupings.Count - 1)
                        {
                            this.Builder.Append($",{this.NewLine()}");
                        }
                        else
                        {
                            this.Builder.Append(this.UnIndent());
                        }
                    }
                }

                if (node.Having != null)
                {
                    this.Builder.Append($"{this.NewLine()}HAVING ");
                    this.Visit(node.Having);
                }

                if (node.Orders.Count > 0)
                {
                    this.Builder.Append($"{this.NewLine()}ORDER BY{this.NewLine() + this.Indent()}");
                    for (var i = 0; i < node.Orders.Count; i++)
                    {
                        this.Visit(node.Orders[i]);

                        if (i < node.Orders.Count - 1)
                        {
                            this.Builder.Append($",{this.NewLine()}");
                        }
                        else
                        {
                            this.Builder.Append(this.UnIndent());
                        }
                    }
                }

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Sources.SelectSource"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitSelectSource(SelectSource node)
            {
                this.Builder.Append($"({this.NewLine()}{this.Indent()}");

                this.Visit(node.Select);

                this.Builder.Append($"{this.UnIndent()}{this.NewLine()}) {node.Alias}");

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.SelectUnionStatement"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitSelectUnionStatement(SelectUnionStatement node)
            {
                this.Visit(node.First);
                this.Builder.Append($"UNION{this.NewLine()}");
                this.Visit(node.Second);
                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.TriggerStatement"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitTriggerStatement(TriggerStatement node)
            {
                this.Builder.Append("TRIGGER " + node.JobName);

                return base.VisitTriggerStatement(node);
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.UnaryConnectQlExpression"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitUnarySqlExpression(UnaryConnectQlExpression node)
            {
                this.Builder.Append($"{node.Op}(");

                this.Visit(node.Expression);

                this.Builder.Append($")");

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.UseStatement"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitUseStatement(UseStatement node)
            {
                this.Builder.Append("USE DEFAULT ");

                this.Visit(node.SettingFunction);

                this.Builder.Append($" FOR {node.FunctionName}");

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.VariableDeclaration"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitVariableDeclaration(VariableDeclaration node)
            {
                this.Builder.Append($"{node.Name} = ");

                this.Visit(node.Expression);

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.VariableConnectQlExpression"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitVariableSqlExpression(VariableConnectQlExpression node)
            {
                this.Builder.Append((string)node.Name);

                return node;
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.WildcardConnectQlExpression"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitWildCardSqlExpression(WildcardConnectQlExpression node)
            {
                this.Builder.Append((node.Source == null ? string.Empty : node.Source + ".") + "*");

                return node;
            }

            /// <summary>
            /// The indent.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            [NotNull]
            private string Indent()
            {
                this.indent++;
                return new string(' ', this.indentSize);
            }

            /// <summary>
            /// The new line.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            [NotNull]
            private string NewLine()
            {
                return Environment.NewLine + new string(' ', this.indent * this.indentSize);
            }

            /// <summary>
            /// The un indent.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            [NotNull]
            private string UnIndent()
            {
                this.indent--;
                return string.Empty;
            }
        }
    }
}