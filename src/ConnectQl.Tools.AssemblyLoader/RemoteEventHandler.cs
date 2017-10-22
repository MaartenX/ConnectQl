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

namespace ConnectQl.Tools.AssemblyLoader
{
    using System;

    /// <summary>
    /// The remote event handler.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the event arguments.
    /// </typeparam>
    public class RemoteEventHandler<T> : MarshalByRefObject
    {
        /// <summary>
        /// The wrapped handler.
        /// </summary>
        private readonly EventHandler<T> wrappedHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteEventHandler{T}"/> class.
        /// </summary>
        /// <param name="wrappedHandler">
        /// The wrapped handler.
        /// </param>
        public RemoteEventHandler(EventHandler<T> wrappedHandler)
        {
            this.wrappedHandler = wrappedHandler;
        }

        /// <summary>
        /// The handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        public void Handler(object sender, T eventArgs)
        {
            this.wrappedHandler.Invoke(sender, eventArgs);
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy
        ///     for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new
        ///     lifetime service object initialized to the value of the
        ///     <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">
        /// The immediate caller does not have infrastructure permission.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet>
        ///     <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/>
        /// </PermissionSet>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}