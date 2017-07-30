// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringBase.cs" company="Winvision bv">
//   Copyright (c) 2016 Winvision bv. All rights reserved.
// </copyright>
// <summary>
//   The connection string base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ConnectQl.Ftp.ConnectionStrings
{
    using System.Reflection;

    /// <summary>
    ///     The connection string base.
    /// </summary>
    public abstract class ConnectionStringBase
    {
        /// <summary>
        ///     The splitter.
        /// </summary>
        protected static readonly char[] Splitter =
            {
                ':', '='
            };

        /// <summary>
        ///     The field config.
        /// </summary>
        protected class FieldConfig
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="FieldConfig" /> class.
            /// </summary>
            /// <param name="property">
            ///     The property.
            /// </param>
            /// <param name="fields">
            ///     The fields.
            /// </param>
            public FieldConfig(PropertyInfo property, string[] fields)
            {
                this.Property = property;
                this.Fields = fields;
            }

            /// <summary>
            ///     Gets the fields.
            /// </summary>
            public string[] Fields { get; }

            /// <summary>
            ///     Gets the property.
            /// </summary>
            public PropertyInfo Property { get; }
        }
    }
}