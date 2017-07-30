// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionString.cs" company="Winvision bv">
//   Copyright (c) 2016 Winvision bv. All rights reserved.
// </copyright>
// <summary>
//   The connection string.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ConnectQl.Ftp.ConnectionStrings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    ///     The connection string.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the inheriting class.
    /// </typeparam>
    public abstract class ConnectionString<T> : ConnectionStringBase
        where T : ConnectionString<T>, new()
    {
        /// <summary>
        ///     Stores the field information.
        /// </summary>
        private static readonly FieldConfig[] Fields = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(p => new FieldConfig(p, p.GetCustomAttributes(typeof(ConnectionStringFieldAttribute), false).Cast<ConnectionStringFieldAttribute>().Select(f => f.Names).FirstOrDefault())).ToArray();

        /// <summary>
        ///     The default split character.
        /// </summary>
        private readonly char defaultSplitCharacter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConnectionString{T}" /> class.
        /// </summary>
        /// <param name="defaultSplitCharacter">
        ///     The default split character.
        /// </param>
        protected ConnectionString(char defaultSplitCharacter = ':')
        {
            this.defaultSplitCharacter = defaultSplitCharacter;
        }

        /// <summary>
        ///     Parses a connection string.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        /// <returns>
        ///     The <typeparamref name="T" />.
        ///     If connection string is null, or empty, or does not contain at least one pair with a valid key-value separator
        ///     return value is null.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when string is not empty, is of a valid format, but can not be parsed (for example due to unknown keys)
        /// </exception
        public static T Parse(string connectionString)
        {
            var property = "Unknown";

            try
            {
                if (connectionString == null)
                {
                    return null;
                }

                var result = new T();

                foreach (var parts in Split(connectionString).Select(item => item.Split(Splitter, 2)))
                {
                    if (parts.Length != 2)
                    {
                        return null;
                    }

                    var fieldConfig = Fields.FirstOrDefault(f => f.Fields.Contains(parts[0].Trim(), StringComparer.InvariantCultureIgnoreCase));

                    property = parts[0];

                    var type = fieldConfig?.Property.PropertyType;

                    if (type == null)
                    {
                        continue;
                    }

                    if (type == typeof(string))
                    {
                        fieldConfig?.Property.SetValue(result, parts[1], null);
                    }
                    else if (type == typeof(Uri))
                    {
                        fieldConfig?.Property.SetValue(result, new Uri(parts[1]), null);
                    }
                    else if (type == typeof(Guid))
                    {
                        fieldConfig?.Property.SetValue(result, new Guid(parts[1]), null);
                    }
                    else if (type == typeof(int))
                    {
                        fieldConfig?.Property.SetValue(result, int.Parse(parts[1]), null);
                    }
                    else if (type == typeof(double))
                    {
                        fieldConfig?.Property.SetValue(result, double.Parse(parts[1]), null);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Don't know how to parse a property of type {type}.");
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Unable to parse connection string. While parsing property {property}: {e.Message.TrimEnd('.')}.", e);
            }
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return string.Join(
                ";",
                Fields.Select(
                    f => new
                             {
                                 Key = f.Fields.FirstOrDefault(),
                                 Value = f.Property.GetValue(this, null)
                             }).Where(kv => kv.Key != null && kv.Value != null).Select(kv => $"{kv.Key}{this.defaultSplitCharacter}{kv.Value}"));
        }

        /// <summary>
        ///     Splits the string allowing for escaped ;-characters.
        /// </summary>
        /// <param name="value">
        ///     The value to split.
        /// </param>
        /// <returns>
        ///     The split string. All ';;' are replaced by ';' and not used as a splitter.
        /// </returns>
        private static IEnumerable<string> Split(string value)
        {
            var current = new StringBuilder();
            var state = 0;

            foreach (var character in value.ToCharArray())
            {
                switch (state)
                {
                    case 0:

                        if (character == ';')
                        {
                            state = 1;
                            break;
                        }

                        current.Append(character);
                        break;
                    case 1:

                        if (character == ';')
                        {
                            current.Append(';');
                        }
                        else
                        {
                            yield return current.ToString();

                            current.Clear();
                        }

                        current.Append(character);

                        state = 0;

                        break;
                }
            }

            if (current.Length != 0)
            {
                yield return current.ToString();
            }
        }
    }
}