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

namespace ConnectQl.Internal.Validation.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using ConnectQl.Internal.Resources;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The binary operator.
    /// </summary>
    internal abstract class BinaryOperator : Operator
    {
        /// <summary>
        /// The <see cref="string.Concat(string, string)"/> method.
        /// </summary>
        private static readonly MethodInfo StringConcatMethod = typeof(string).GetRuntimeMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });

        /// <summary>
        /// Caches dynamic expressions.
        /// </summary>
        private static readonly Dictionary<Tuple<string, Type, Type>, Func<object, object, object>> CachedExpressions = new Dictionary<Tuple<string, Type, Type>, Func<object, object, object>>();

        /// <summary>
        /// Caches the overload methods that best fit the two types.
        /// </summary>
        private static readonly Dictionary<Tuple<TypeInfo, TypeInfo, string>, MethodInfo> OverloadCache = new Dictionary<Tuple<TypeInfo, TypeInfo, string>, MethodInfo>();

        /// <summary>
        /// The names that overloaded methods for the specified expressions have.
        /// </summary>
        private static readonly Dictionary<string, string> OverloadNames = new Dictionary<string, string>
                                                                               {
                                                                                   { "+", "op_Addition" },
                                                                                   { "-", "op_Subtraction" },
                                                                                   { "*", "op_Multiply" },
                                                                                   { "/", "op_Division" },
                                                                                   { "%", "op_Modulus" },
                                                                                   { "^", "op_Power" },
                                                                                   { ">", "op_GreaterThan" },
                                                                                   { ">=", "op_GreaterThanOrEqual" },
                                                                                   { "=", "op_Equality" },
                                                                                   { "<=", "op_LessThanOrEqual" },
                                                                                   { "<", "op_LessThan" },
                                                                                   { "<>", "op_inequality" },
                                                                                   { "AND", "op_LogicalAnd" },
                                                                                   { "OR", "op_LogicalOr" }
                                                                               };

        /// <summary>
        /// Maps an expression to a dynamic method which can be used in the expression.
        /// </summary>
        private static readonly Dictionary<string, MethodInfo> DynamicMethods = new Dictionary<string, MethodInfo>
                                                                                    {
                                                                                        {
                                                                                            "+",
                                                                                            ((Func<object, object, object>)BinaryOperator.DynamicAdd).GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            "-",
                                                                                            ((Func<object, object, object>)BinaryOperator.DynamicSubtract)
                                                                                            .GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            "*",
                                                                                            ((Func<object, object, object>)BinaryOperator.DynamicMultiply)
                                                                                            .GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            "/",
                                                                                            ((Func<object, object, object>)BinaryOperator.DynamicDivide).GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            "%",
                                                                                            ((Func<object, object, object>)BinaryOperator.DynamicModulo).GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            "^",
                                                                                            ((Func<object, object, object>)BinaryOperator.DynamicPower).GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            ">",
                                                                                            ((Func<object, object, bool>)BinaryOperator.DynamicGreaterThan)
                                                                                            .GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            ">=",
                                                                                            ((Func<object, object, bool>)BinaryOperator.DynamicGreaterThanOrEqual)
                                                                                            .GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            "=",
                                                                                            ((Func<object, object, bool>)BinaryOperator.DynamicEqual).GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            "<=",
                                                                                            ((Func<object, object, bool>)BinaryOperator.DynamicLessThanOrEqual)
                                                                                            .GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            "<",
                                                                                            ((Func<object, object, bool>)BinaryOperator.DynamicLessThan).GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            "<>",
                                                                                            ((Func<object, object, bool>)BinaryOperator.DynamicNotEqual).GetMethodInfo()
                                                                                        },
                                                                                        {
                                                                                            "AND",
                                                                                            ((Func<object, object, bool>)BinaryOperator.DynamicAnd).GetMethodInfo()
                                                                                        },
                                                                                        { "OR", ((Func<object, object, bool>)BinaryOperator.DynamicOr).GetMethodInfo() }
                                                                                    };

        /// <summary>
        /// Generates an <see cref="Expression"/> for the binary <see cref="ExpressionType"/>.
        /// </summary>
        /// <param name="type">
        ///     The expression type.
        /// </param>
        /// <param name="first">
        ///     The first operand of the binary expression.
        /// </param>
        /// <param name="second">
        ///     The second operand of the binary expression.
        /// </param>
        /// <param name="onError">
        ///     Called when errors occur.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression GenerateExpression(ExpressionType type, [NotNull] Expression first, [NotNull] Expression second, [CanBeNull] Action<string> onError = null)
        {
            return GenerateExpression(GetOperator(type), first, second, onError);
        }

        /// <summary>
        /// Generates an <see cref="Expression"/> for the binary operator.
        /// </summary>
        /// <param name="op">
        ///     The operator.
        /// </param>
        /// <param name="first">
        ///     The first operand of the binary expression.
        /// </param>
        /// <param name="second">
        ///     The second operand of the binary expression.
        /// </param>
        /// <param name="onError">
        ///     Called when errors occur.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression GenerateExpression([NotNull] string op, [NotNull] Expression first, [NotNull] Expression second, [CanBeNull] Action<string> onError = null)
        {
            switch (op.ToUpperInvariant())
            {
                case "+":
                    return first.Type == typeof(string) || second.Type == typeof(string)
                               ? Expression.Add(Operator.ToString(first), Operator.ToString(second), BinaryOperator.StringConcatMethod)
                               : DoConversion(op, first, second, parts => Expression.Add(parts.Item1, parts.Item2, parts.Item3));
                case "-":
                    return DoConversion(op, first, second, parts => Expression.Subtract(parts.Item1, parts.Item2, parts.Item3));
                case "/":
                    return DoConversion(op, first, second, parts => Expression.Divide(parts.Item1, parts.Item2, parts.Item3));
                case "*":
                    return DoConversion(op, first, second, parts => Expression.Multiply(parts.Item1, parts.Item2, parts.Item3));
                case "%":
                    return DoConversion(op, first, second, parts => Expression.Modulo(parts.Item1, parts.Item2, parts.Item3));
                case "^":
                    return DoConversion(op, first, second, parts => Expression.Power(parts.Item1, parts.Item2, parts.Item3));
                case ">":
                    return first.Type == typeof(string) || second.Type == typeof(string)
                               ? Expression.GreaterThan(Operator.ToString(first), Operator.ToString(second), false, ((Func<string, string, bool>)BinaryOperator.StringGreaterThan).GetMethodInfo())
                               : DoConversion(op, first, second, parts => Expression.GreaterThan(parts.Item1, parts.Item2, false, parts.Item3));
                case ">=":
                    return first.Type == typeof(string) || second.Type == typeof(string)
                               ? Expression.GreaterThanOrEqual(
                                   Operator.ToString(first),
                                   Operator.ToString(second),
                                   false,
                                   ((Func<string, string, bool>)BinaryOperator.StringGreaterThanOrEqual).GetMethodInfo())
                               : DoConversion(op, first, second, parts => Expression.GreaterThanOrEqual(parts.Item1, parts.Item2, false, parts.Item3));
                case "=":
                    return first.Type == typeof(string) || second.Type == typeof(string)
                               ? Expression.Equal(Operator.ToString(first), Operator.ToString(second), false, ((Func<string, string, bool>)BinaryOperator.StringEqual).GetMethodInfo())
                               : DoConversion(op, first, second, parts => Expression.Equal(parts.Item1, parts.Item2, false, parts.Item3));
                case "<>":
                    return first.Type == typeof(string) || second.Type == typeof(string)
                               ? Expression.NotEqual(Operator.ToString(first), Operator.ToString(second), false, ((Func<string, string, bool>)BinaryOperator.StringNotEqual).GetMethodInfo())
                               : DoConversion(op, first, second, parts => Expression.NotEqual(parts.Item1, parts.Item2, false, parts.Item3));
                case "<":
                    return first.Type == typeof(string) || second.Type == typeof(string)
                               ? Expression.LessThan(Operator.ToString(first), Operator.ToString(second), false, ((Func<string, string, bool>)BinaryOperator.StringLessThan).GetMethodInfo())
                               : DoConversion(op, first, second, parts => Expression.LessThan(parts.Item1, parts.Item2, false, parts.Item3));
                case "<=":
                    return first.Type == typeof(string) || second.Type == typeof(string)
                               ? Expression.LessThanOrEqual(
                                   Operator.ToString(first),
                                   Operator.ToString(second),
                                   false,
                                   ((Func<string, string, bool>)BinaryOperator.StringLessThanOrEqual).GetMethodInfo())
                               : DoConversion(op, first, second, parts => Expression.LessThanOrEqual(parts.Item1, parts.Item2, false, parts.Item3));
                case "AND":
                    return DoConversion(op, first, second, parts => Expression.And(parts.Item1, parts.Item2, parts.Item3));
                case "OR":
                    return DoConversion(op, first, second, parts => Expression.Or(parts.Item1, parts.Item2, parts.Item3));
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), string.Format(Messages.UnknownOperator, op));
            }
        }

        /// <summary>
        /// Infers the type of the binary expression.
        /// </summary>
        /// <param name="op">
        /// The operator.
        /// </param>
        /// <param name="first">
        /// The first operand of the binary expression.
        /// </param>
        /// <param name="second">
        /// The second operand of the binary expression.
        /// </param>
        /// <param name="errorCallback">
        /// Called when errors occur.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public static Type InferType([NotNull] string op, Type first, Type second, Action<string> errorCallback)
        {
            return GenerateExpression(op, Expression.Parameter(first), Expression.Parameter(second), errorCallback).Type;
        }

        /// <summary>
        /// Gets the operator for the specified <see cref="ExpressionType"/>.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throw when <paramref name="type"/> cannot be mapped to an operator.
        /// </exception>
        [NotNull]
        private static string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.Power:
                    return "^";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
            }

            throw new ArgumentOutOfRangeException(nameof(type), string.Format(Messages.NodeTypeNotSupported, type));
        }

        /// <summary>
        /// Applies the expression function to the arguments after converting to a common type.
        /// </summary>
        /// <param name="op">
        /// The operator for this expression.
        /// </param>
        /// <param name="first">
        /// The first expression.
        /// </param>
        /// <param name="second">
        /// The second expression.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        private static Expression DoConversion(string op, [NotNull] Expression first, [NotNull] Expression second, [NotNull] Func<Tuple<Expression, Expression, MethodInfo>, Expression> expression)
        {
            return expression(GetExpressionParts(op, first, second));
        }

        /// <summary>
        /// Converts the <see cref="Expression"/>s to the correct type, and returns the <see cref="MethodInfo"/> used for calling the expression if applicable.
        /// </summary>
        /// <param name="op">
        /// The operator for this expression.
        /// </param>
        /// <param name="first">The first expression.</param>
        /// <param name="second">The second expression.</param>
        /// <returns>The expression.</returns>
        private static Tuple<Expression, Expression, MethodInfo> GetExpressionParts(string op, Expression first, Expression second)
        {
            if (first.Type == typeof(object) || second.Type == typeof(object))
            {
                if (DynamicMethods.TryGetValue(op, out var method))
                {
                    return Tuple.Create(ToObject(first), ToObject(second), method);
                }

                throw new Exception($"Could not find dynamic method for operator {op} for types {first.Type.Name} and {second.Type.Name}.");
            }

            var firstInfo = first.Type.GetTypeInfo();
            var secondInfo = second.Type.GetTypeInfo();
            var overloaded = BinaryOperator.GetOverloadedMethod(op, firstInfo, secondInfo);

            if (overloaded != null)
            {
                var parameters = overloaded.GetParameters();

                return Tuple.Create(Operator.ToType(first, parameters[0].ParameterType), Operator.ToType(second, parameters[1].ParameterType), overloaded);
            }

            if (first.Type == second.Type)
            {
                return Tuple.Create(first, second, (MethodInfo)null);
            }

            if (firstInfo.IsEnum)
            {
                first = Operator.ToType(first, Enum.GetUnderlyingType(first.Type));
            }

            if (secondInfo.IsEnum)
            {
                second = Operator.ToType(second, Enum.GetUnderlyingType(second.Type));
            }

            if (Operator.CommonTypeOrder.Contains(first.Type) && Operator.CommonTypeOrder.Contains(second.Type))
            {
                // Try to cast the arguments to a common type, starting with the highest precision.
                foreach (var type in Operator.CommonTypeOrder)
                {
                    if (first.Type != type && second.Type != type)
                    {
                        continue;
                    }

                    first = Operator.ToType(first, type);
                    second = Operator.ToType(second, type);

                    return Tuple.Create(first, second, (MethodInfo)null);
                }
            }

            return Tuple.Create(first, second, (MethodInfo)null);
        }

        /// <summary>
        /// Looks for operator overloads in <paramref name="firstInfo"/> and <paramref name="secondInfo"/> and returns the 
        /// best fit.
        /// </summary>
        /// <param name="operatorName">
        /// The operator name.
        /// </param>
        /// <param name="firstInfo">
        /// The type of the first parameter.
        /// </param>
        /// <param name="secondInfo">
        /// The type of the second parameter.
        /// </param>
        /// <returns>
        /// The operator overload or <c>null</c> if no match is found.
        /// </returns>
        [CanBeNull]
        private static MethodInfo GetOverloadedMethod(string operatorName, TypeInfo firstInfo, TypeInfo secondInfo)
        {
            var tuple = Tuple.Create(firstInfo, secondInfo, operatorName);

            if (OverloadCache.TryGetValue(tuple, out var result))
            {
                return result;
            }

            if (!OverloadNames.TryGetValue(operatorName, out var overloadedName))
            {
                return OverloadCache[tuple] = null;
            }

            var methods = firstInfo.GetDeclaredMethods(overloadedName).Concat(secondInfo.GetDeclaredMethods(overloadedName));

            var leftSide = CommonTypeOrder.Contains(firstInfo.AsType())
                               ? CommonTypeOrder.Select((o, i) => new { Order = i, Type = o.GetTypeInfo() }).ToArray()
                               : new[] { new { Order = 0, Type = firstInfo } };

            var rightSide = CommonTypeOrder.Contains(secondInfo.AsType())
                                ? CommonTypeOrder.Select((o, i) => new { Order = i, Type = o.GetTypeInfo() }).ToArray()
                                : new[] { new { Order = 0, Type = secondInfo } };

            return OverloadCache[tuple] = methods.Select(method => new { MethodInfo = method, Parameters = method.GetParameters().Select(p => p.ParameterType.GetTypeInfo()).ToArray() })
                       .Where(m => m.Parameters.Length == 2)
                       .SelectMany(
                           method => leftSide.SelectMany(leftItem => rightSide.Select(rightItem => new { Left = leftItem, Right = rightItem, Method = method })).Where(
                               m => m.Method.Parameters[0].IsAssignableFrom(m.Left.Type) && m.Method.Parameters[1].IsAssignableFrom(m.Right.Type))).OrderBy(m => m.Left.Order)
                       .ThenBy(m => m.Right.Order).FirstOrDefault()?.Method.MethodInfo;
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="op">
        /// The operator.
        /// </param>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <param name="createExpression">
        /// Callback that creates the expression based on two <see cref="Expression"/>s and a <see cref="MethodInfo"/>.
        /// </param>
        /// <returns>
        /// The result of the evaluation.
        /// </returns>
        private static object Evaluate(string op, [CanBeNull] object first, [CanBeNull] object second, Func<Tuple<Expression, Expression, MethodInfo>, Expression> createExpression)
        {
            if (first == null || second == null)
            {
                switch (op)
                {
                    case "<>": return !Equals(first, second);
                    case "=": return Equals(first, second);
                    case ">":
                        return first != null;
                    case ">=":
                        return first != null || second == null;
                    case "<":
                        return second != null;
                    case "<=":
                        return second != null || first == null;
                    default:
                        return null;
                }
            }

            var firstType = first.GetType();
            var secondType = second.GetType();
            var tuple = Tuple.Create(op, firstType, secondType);

            if (CachedExpressions.TryGetValue(tuple, out var result))
            {
                return result(first, second);
            }

            var left = Expression.Parameter(firstType);
            var right = Expression.Parameter(secondType);
            var leftObject = Expression.Parameter(typeof(object));
            var rightObject = Expression.Parameter(typeof(object));

            try
            {
                var parts = GetExpressionParts(op, left, right);

                var expr = Expression.Lambda<Func<object, object, object>>(
                    Expression.Convert(createExpression(parts), typeof(object)).ReplaceParameter(left, Operator.ToType(leftObject, firstType))
                        .ReplaceParameter(right, Operator.ToType(rightObject, secondType)),
                    leftObject,
                    rightObject);

                return (CachedExpressions[tuple] = expr.Compile()).Invoke(first, second);
            }
            catch (Exception e)
            {
                var error = new Error(new Exception(string.Format(Messages.OperatorNotSupported, op, firstType, secondType), e));

                CachedExpressions[tuple] = (a, b) => error;

                return error;
            }
        }

        /// <summary>
        /// Calculates the modulo of two objects and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        [CanBeNull]
        private static object DynamicModulo([CanBeNull] object first, [CanBeNull] object second) => Evaluate("%", first, second, p => Expression.Modulo(p.Item1, p.Item2, p.Item3));

        /// <summary>
        /// Divides two objects and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        [CanBeNull]
        private static object DynamicDivide([CanBeNull] object first, [CanBeNull] object second) => Evaluate("*", first, second, p => Expression.Divide(p.Item1, p.Item2, p.Item3));

        /// <summary>
        /// Multiplies two objects and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        [CanBeNull]
        private static object DynamicMultiply([CanBeNull] object first, [CanBeNull] object second) => Evaluate("*", first, second, p => Expression.Multiply(p.Item1, p.Item2, p.Item3));

        /// <summary>
        /// Calculates the firt object to the power of the second and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        [CanBeNull]
        private static object DynamicPower([CanBeNull] object first, [CanBeNull] object second) => Evaluate("^", first, second, p => Expression.Power(p.Item1, p.Item2, p.Item3));

        /// <summary>
        /// Subtracts two objects and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        [CanBeNull]
        private static object DynamicSubtract([CanBeNull] object first, [CanBeNull] object second) => Evaluate("-", first, second, p => Expression.Subtract(p.Item1, p.Item2, p.Item3));

        /// <summary>
        /// Adds two objects and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        [CanBeNull]
        private static object DynamicAdd([CanBeNull] object first, [CanBeNull] object second) =>
            first is string || second is string ? string.Concat(first, second) : BinaryOperator.Evaluate("+", first, second, p => Expression.Add(p.Item1, p.Item2, p.Item3));

        /// <summary>
        /// Checks if <paramref name="first"/> is greater than <paramref name="second"/> and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        private static bool DynamicGreaterThan([CanBeNull] object first, [CanBeNull] object second) => DynamicComparison(">", first, second, Expression.GreaterThan);

        /// <summary>
        /// Checks if <paramref name="first"/> is greater than or equal to <paramref name="second"/> and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        private static bool DynamicGreaterThanOrEqual([CanBeNull] object first, [CanBeNull] object second) => DynamicComparison(">=", first, second, Expression.GreaterThanOrEqual);

        /// <summary>
        /// Checks if <paramref name="first"/> is equal to <paramref name="second"/> and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        private static bool DynamicEqual([CanBeNull] object first, [CanBeNull] object second) => DynamicComparison("=", first, second, Expression.Equal);

        /// <summary>
        /// Checks if <paramref name="first"/> is not equal to <paramref name="second"/> and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        private static bool DynamicNotEqual([CanBeNull] object first, [CanBeNull] object second) => DynamicComparison("<>", first, second, Expression.NotEqual);

        /// <summary>
        /// Checks if <paramref name="first"/> is less than <paramref name="second"/> and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        private static bool DynamicLessThan([CanBeNull] object first, [CanBeNull] object second) => DynamicComparison("<", first, second, Expression.LessThan);

        /// <summary>
        /// Checks if <paramref name="first"/> is less than or equal to <paramref name="second"/> and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        private static bool DynamicLessThanOrEqual([CanBeNull] object first, [CanBeNull] object second) => DynamicComparison("<=", first, second, Expression.LessThanOrEqual);

        /// <summary>
        /// Gets the logical AND of <paramref name="first"/> and <paramref name="second"/> and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        private static bool DynamicAnd([CanBeNull] object first, [CanBeNull] object second) => (bool)BinaryOperator.Evaluate("AND", first, second, p => Expression.And(p.Item1, p.Item2, p.Item3));

        /// <summary>
        /// Gets the logical OR of <paramref name="first"/> and <paramref name="second"/> and returns the result.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result.</returns>
        private static bool DynamicOr([CanBeNull] object first, [CanBeNull] object second) => (bool)BinaryOperator.Evaluate("OR", first, second, p => Expression.Or(p.Item1, p.Item2, p.Item3));
        
        /// <summary>
        /// Compares two strings, and returns true if <paramref name="first"/> is greater than <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first string.</param>
        /// <param name="second">The second string.</param>
        /// <returns><c>true</c> if <paramref name="first"/> is greater than <paramref name="second"/>, <c>false</c> otherwise.</returns>
        private static bool StringGreaterThan(string first, string second) => string.Compare(first, second, Operator.StringComparisionMode) > 0;

        /// <summary>
        /// Compares two strings, and returns true if <paramref name="first"/> is greater than or equal to <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first string.</param>
        /// <param name="second">The second string.</param>
        /// <returns><c>true</c> if <paramref name="first"/> is greater than or equal to <paramref name="second"/>, <c>false</c> otherwise.</returns>
        private static bool StringGreaterThanOrEqual(string first, string second) => string.Compare(first, second, Operator.StringComparisionMode) >= 0;

        /// <summary>
        /// Compares two strings, and returns true if <paramref name="first"/> is less than <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first string.</param>
        /// <param name="second">The second string.</param>
        /// <returns><c>true</c> if <paramref name="first"/> is less than <paramref name="second"/>, <c>false</c> otherwise.</returns>
        private static bool StringLessThan(string first, string second) => string.Compare(first, second, Operator.StringComparisionMode) < 0;

        /// <summary>
        /// Compares two strings, and returns true if <paramref name="first"/> is less than or equal to <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first string.</param>
        /// <param name="second">The second string.</param>
        /// <returns><c>true</c> if <paramref name="first"/> is less than or equal to <paramref name="second"/>, <c>false</c> otherwise.</returns>
        private static bool StringLessThanOrEqual(string first, string second) => string.Compare(first, second, Operator.StringComparisionMode) <= 0;

        /// <summary>
        /// Compares two strings, and returns true if <paramref name="first"/> is equal to <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first string.</param>
        /// <param name="second">The second string.</param>
        /// <returns><c>true</c> if <paramref name="first"/> is equal to <paramref name="second"/>, <c>false</c> otherwise.</returns>
        private static bool StringEqual(string first, string second) => string.Compare(first, second, Operator.StringComparisionMode) == 0;

        /// <summary>
        /// Compares two strings, and returns true if <paramref name="first"/> is not equal to <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first string.</param>
        /// <param name="second">The second string.</param>
        /// <returns><c>true</c> if <paramref name="first"/> is not equal to <paramref name="second"/>, <c>false</c> otherwise.</returns>
        private static bool StringNotEqual(string first, string second) => string.Compare(first, second, Operator.StringComparisionMode) != 0;

        /// <summary>
        /// Compares the two object using the specified operator and expression and returns the result.
        /// </summary>
        /// <param name="op">The operator.</param>
        /// <param name="first">he first object.</param>
        /// <param name="second">The second object.</param>
        /// <param name="expression">Factory for <see cref="Expression"/>.</param>
        /// <returns>The result.</returns>
        private static bool DynamicComparison(string op, [CanBeNull] object first, [CanBeNull] object second, Func<Expression, Expression, bool, MethodInfo, Expression> expression)
        {
            if (!(first is string || second is string))
            {
                return (bool)(Evaluate(op, first, second, p => expression(p.Item1, p.Item2, false, p.Item3)) ?? false);
            }

            switch (op)
            {
                case ">":
                    return StringGreaterThan(first?.ToString(), second?.ToString());
                case ">=":
                    return StringGreaterThanOrEqual(first?.ToString(), second?.ToString());
                case "=":
                    return StringEqual(first?.ToString(), second?.ToString());
                case "<>":
                    return StringNotEqual(first?.ToString(), second?.ToString());
                case "<":
                    return StringLessThan(first?.ToString(), second?.ToString());
                case "<=":
                    return StringLessThanOrEqual(first?.ToString(), second?.ToString());
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), string.Format(Messages.OperatorNotSupported, op));
            }
        }
    }
}