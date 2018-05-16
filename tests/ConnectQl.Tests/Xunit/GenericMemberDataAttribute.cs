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

namespace ConnectQl.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using Xunit;

    /// <summary>
    /// Calls a generic method to create the data for a test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class GenericMemberDataAttribute : MemberDataAttributeBase
    {
        private readonly Type[] genericArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMemberDataAttribute"/> class.
        /// Marks this method as a test with data created by a generic member.
        /// </summary>
        /// <param name="memberName">
        /// The name of the member.
        /// </param>
        /// <param name="parameters">
        /// The parameters of the member. The generic arguments will be inferred using these parameters.
        /// </param>
        public GenericMemberDataAttribute(string memberName, params object[] parameters)
            : base(memberName, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMemberDataAttribute"/> class.
        /// Marks this method as a test with data created by a generic member.
        /// </summary>
        /// <param name="memberName">
        /// The name of the member.
        /// </param>
        /// <param name="genericArguments">
        /// The generic arguments for the method.
        /// </param>
        /// <param name="parameters">
        /// The parameters of the member. The missing generic arguments will be inferred using these parameters.
        /// </param>
        public GenericMemberDataAttribute(string memberName, Type[] genericArguments, params object[] parameters)
            : base(memberName, parameters)
        {
            this.genericArguments = genericArguments;
        }

        /// <summary>
        /// Gets the data for the test method.
        /// </summary>
        /// <param name="testMethod">
        /// The test method to get the data for.
        /// </param>
        /// <returns>
        /// An enumerable of object arrays that are the parameters for the method.
        /// </returns>
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var type = this.MemberType ?? testMethod.DeclaringType;

            var accessor = this.GetMethodAccessor(type, this.genericArguments);

            if (accessor == null)
            {
                var parameterText = this.Parameters?.Length > 0 ? $" with parameter types: {string.Join(", ", this.Parameters.Select(p => p?.GetType().FullName ?? "(null)"))}" : string.Empty;

                throw new ArgumentException($"Could not find public static member (property, field, or method) named '{this.MemberName}' on {type.FullName}{parameterText}");
            }

            var obj = accessor();

            if (obj == null)
            {
                return null;
            }

            var dataItems = obj as IEnumerable<object>;

            if (dataItems == null)
            {
                throw new ArgumentException($"Property {this.MemberName} on {type.FullName} did not return IEnumerable<object>");
            }

            return dataItems.Select(item => this.ConvertDataItem(testMethod, item));
        }

        /// <inheritdoc/>
        protected override object[] ConvertDataItem(MethodInfo testMethod, object item)
        {
            if (item == null)
            {
                return null;
            }

            var array = item as object[];

            if (array == null)
            {
                throw new ArgumentException($"Property {this.MemberName} on {this.MemberType ?? testMethod.DeclaringType} yielded an item that is not an object[]");
            }

            return array;
        }

        /// <summary>
        /// Gets the generic method.
        /// </summary>
        /// <param name="method">
        /// The generic method info.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="argumentTypes">
        /// The argument types.
        /// </param>
        /// <returns>
        /// The instantiated method info.
        /// </returns>
        private static MethodInfo GetGenericMethod(MethodInfo method, object[] parameters, Type[] argumentTypes)
        {
            var methodParameters = method.GetParameters();

            if (methodParameters.Length > 0 && methodParameters[methodParameters.Length - 1].GetCustomAttribute<ParamArrayAttribute>() != null)
            {
                var paramList = parameters.Take(methodParameters.Length - 1).ToList();

                paramList.Add(parameters.Skip(methodParameters.Length - 1).ToArray());

                parameters = paramList.ToArray();
            }

            var parameterTypes = parameters?.Select(p => p?.GetType()).ToArray() ?? new Type[0];

            if (methodParameters?.Length != parameterTypes.Length)
            {
                return null;
            }

            var argumentIndex = 0;

            var genericTypes = (argumentTypes ?? new Type[0]).ToList();

            for (var idx = 0; idx < methodParameters.Length; ++idx)
            {
                if (methodParameters[idx].ParameterType.IsGenericParameter)
                {
                    if (++argumentIndex > genericTypes.Count)
                    {
                        genericTypes.Add(parameterTypes[idx]);
                    }
                }

                if (methodParameters[idx].ParameterType.IsConstructedGenericType)
                {
                    var genericArgumentIdx = 0;
                    foreach (var argument in methodParameters[genericArgumentIdx].ParameterType.GenericTypeArguments)
                    {
                        if (argument.IsGenericParameter)
                        {
                            if (++argumentIndex > genericTypes.Count)
                            {
                                var type = parameterTypes[idx].GetBaseType(methodParameters[idx].ParameterType.GetGenericTypeDefinition())?.GenericTypeArguments[genericArgumentIdx++];

                                if (type != null)
                                {
                                    genericTypes.Add(type);
                                }
                            }
                        }
                    }
                }
            }

            var result = method.MakeGenericMethod(genericTypes.ToArray());

            methodParameters = result.GetParameters();

            for (var idx = 0; idx < methodParameters.Length; ++idx)
            {
                if (parameterTypes[idx] != null && !methodParameters[idx].ParameterType.GetTypeInfo().IsAssignableFrom(parameterTypes[idx].GetTypeInfo()))
                {
                    return null;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the method accessor for the type and generic arguments.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="genericArguments">
        /// The generic arguments.
        /// </param>
        /// <returns>
        /// A lambda returning an object.
        /// </returns>
        private Func<object> GetMethodAccessor(Type type, Type[] genericArguments)
        {
            MethodInfo methodInfo = null;

            var parameters = new object[0];

            for (var reflectionType = type; reflectionType != null; reflectionType = type.GetTypeInfo().BaseType == reflectionType ? null : type.GetTypeInfo().BaseType)
            {
                methodInfo = reflectionType.GetRuntimeMethods()
                    .Where(m => m.IsGenericMethodDefinition && m.Name == this.MemberName)
                    .Select(m => GetGenericMethod(m, this.Parameters, genericArguments))
                    .FirstOrDefault();

                if (methodInfo != null)
                {
                    var methodParameters = methodInfo.GetParameters();

                    if (methodParameters.Length > 0 && methodParameters[methodParameters.Length - 1].GetCustomAttribute<ParamArrayAttribute>() != null)
                    {
                        var paramList = this.Parameters.Take(methodParameters.Length - 1).ToList();

                        paramList.Add(this.Parameters.Skip(methodParameters.Length - 1).ToArray());

                        parameters = paramList.ToArray();
                    }
                    else
                    {
                        parameters = this.Parameters;
                    }

                    break;
                }
            }

            if (!(methodInfo?.IsStatic ?? false))
            {
                return null;
            }

            return () => methodInfo.Invoke(null, parameters);
        }
    }
}
