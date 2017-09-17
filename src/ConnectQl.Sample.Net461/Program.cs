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

namespace ConnectQl.Sample.Net461
{
    using System.Threading.Tasks;
    using ConnectQl.Logger.Console;
    using ConnectQl.Platform;

    /// <summary>
    /// Sample program that executes a query using .NET Core.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The asynchronous entry point.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>
        /// The task.
        /// </returns>
        public static async Task MainAsync(string[] args)
        {
            try
            {
                using (var context = new ConnectQlContext(new PluginResolver()))
                {
                    await context.ExecuteFileAsync("script3.connectql");
                }
            }
            catch
            {
            }

            /*
                var result = await context.ExecuteFileAsync("Example.connectql");
            }*/
        }

        /// <summary>
        /// The entry point.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public static void Main(string[] args) => Program.MainAsync(args).GetAwaiter().GetResult();
    }

}