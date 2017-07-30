// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringFieldAttribute.cs" company="Winvision bv">
//   Copyright (c) 2016 Winvision bv. All rights reserved.
// </copyright>
// <summary>
//   The field config attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ConnectQl.Ftp.ConnectionStrings
{
    using System;

    /// <summary>
    /// The field config attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConnectionStringFieldAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringFieldAttribute"/> class.
        /// </summary>
        /// <param name="names">
        /// The names.
        /// </param>
        public ConnectionStringFieldAttribute(params string[] names)
        {
            this.Names = names;
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        public string[] Names { get; private set; }
    }
}