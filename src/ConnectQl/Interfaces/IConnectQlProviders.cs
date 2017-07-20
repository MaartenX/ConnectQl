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

namespace ConnectQl.Interfaces
{
    using System;
    using System.Collections;
    using System.Linq.Expressions;

    /// <summary>
    /// The ConnectQlProviders interface.
    /// </summary>
    public interface IConnectQlProviders : IEnumerable
    {
        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TDataProvider">
        /// The type of the data provider.
        /// </typeparam>
        void Add<TDataProvider>(string key, Expression<Func<TDataProvider>> function)
            where TDataProvider : IDataAccess;

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument">
        /// The type of the argument.
        /// </typeparam>
        /// <typeparam name="TDataProvider">
        /// The type of the data provider.
        /// </typeparam>
        void Add<TArgument, TDataProvider>(string key, Expression<Func<TArgument, TDataProvider>> function)
            where TDataProvider : IDataAccess;

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TDataProvider">
        /// The type of the data provider.
        /// </typeparam>
        void Add<TArgument1, TArgument2, TDataProvider>(string key, Expression<Func<TArgument1, TArgument2, TDataProvider>> function)
            where TDataProvider : IDataAccess;

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TDataProvider">
        /// The type of the data provider.
        /// </typeparam>
        void Add<TArgument1, TArgument2, TArgument3, TDataProvider>(string key, Expression<Func<TArgument1, TArgument2, TArgument3, TDataProvider>> function)
            where TDataProvider : IDataAccess;

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TArgument4">
        /// The type of the fourth argument.
        /// </typeparam>
        /// <typeparam name="TDataProvider">
        /// The type of the data provider.
        /// </typeparam>
        void Add<TArgument1, TArgument2, TArgument3, TArgument4, TDataProvider>(string key, Expression<Func<TArgument1, TArgument2, TArgument3, TArgument4, TDataProvider>> function)
            where TDataProvider : IDataAccess;

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TArgument4">
        /// The type of the fourth argument.
        /// </typeparam>
        /// <typeparam name="TArgument5">
        /// The type of the fifth argument.
        /// </typeparam>
        /// <typeparam name="TDataProvider">
        /// The type of the data provider.
        /// </typeparam>
        void Add<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TDataProvider>(string key, Expression<Func<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TDataProvider>> function)
            where TDataProvider : IDataAccess;

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TArgument4">
        /// The type of the fourth argument.
        /// </typeparam>
        /// <typeparam name="TArgument5">
        /// The type of the fifth argument.
        /// </typeparam>
        /// <typeparam name="TArgument6">
        /// The type of the sixth argument.
        /// </typeparam>
        /// <typeparam name="TDataProvider">
        /// The type of the data provider.
        /// </typeparam>
        void Add<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TDataProvider>(string key, Expression<Func<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TDataProvider>> function)
            where TDataProvider : IDataAccess;
    }
}