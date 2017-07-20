// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableStringList.cs" company="Winvision bv">
// Copyright (c) 2017 Winvision bv. All rights reserved.
// </copyright>
// <summary>
//   The string list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace StorageSql.Internal.Intellisense.Protocol
{
    using System.Collections.Generic;

    /// <summary>
    /// The string list.
    /// </summary>
    [CollectionDataContract(Name = "strings", Namespace = "")]
    internal class SerializableStringList : List<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableStringList"/> class.
        /// </summary>
        public SerializableStringList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableStringList"/> class.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public SerializableStringList(IEnumerable<string> items)
            : base(items)
        {
        }
    }
}