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

namespace ConnectQl.Tools.Mef.ToolBar
{
    using System;

    /// <summary>
    /// The commands.
    /// </summary>
    internal class Commands
    {
        /// <summary>
        /// The Connect Ql command set.
        /// </summary>
        public static readonly Guid ConnectQlCommandSet = new Guid("5ea5a5f2-d344-423e-99fa-1546f86445c8");

        /// <summary>
        /// The Connect Ql tool bar group identifier.
        /// </summary>
        public const int ConnectQlToolBarGroupId = 0x1000;

        /// <summary>
        /// The Connect Ql tool bar identifier.
        /// </summary>
        public const int ConnectQlToolBarId = 0x1050;

        /// <summary>
        /// The run script command identifier.
        /// </summary>
        public const int RunScriptCommandId = 0x2001;
    }
}
