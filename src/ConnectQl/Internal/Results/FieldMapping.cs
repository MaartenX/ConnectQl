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

namespace ConnectQl.Internal.Results
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Translates wildcard fields so no duplicates are in the result set.
    /// </summary>
    internal class FieldMapping
    {
        /// <summary>
        /// Quick lookup for the added fields.
        /// </summary>
        private readonly HashSet<string> fields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Stores the field declarations.
        /// </summary>
        private readonly MappedField[] mappedFields;

        /// <summary>
        /// Translates fields to their display name.
        /// </summary>
        private readonly Dictionary<string, string> mapToInternalName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMapping"/> class.
        /// </summary>
        /// <param name="fields">
        /// The fields.
        /// </param>
        public FieldMapping(IEnumerable<string> fields)
        {
            this.mappedFields = fields.Select(field => new MappedField(field)).ToArray();
        }

        /// <summary>
        /// Gets the field display names.
        /// </summary>
        public IReadOnlyList<string> Fields { get; private set; }

        /// <summary>
        /// Gets the internal field name by the translated name.
        /// </summary>
        /// <param name="name">
        /// The translated name to get the field name for.
        /// </param>
        /// <returns>
        /// The field name.
        /// </returns>
        public string this[string name] => this.mapToInternalName.TryGetValue(name, out string result) ? result : null;

        /// <summary>
        /// Adds a field to the translator.
        /// </summary>
        /// <param name="name">
        /// The name of the field to add.
        /// </param>
        public void AddField(string name)
        {
            if (this.AddFieldInternal(name))
            {
                this.CalculateTranslations();
            }
        }

        /// <summary>
        /// Adds fields to the mapping.
        /// </summary>
        /// <param name="fieldNames">
        /// The names of the fields to add.
        /// </param>
        public void AddRowFields(IEnumerable<string> fieldNames)
        {
            if (fieldNames.Aggregate(false, (result, name) => result | this.AddFieldInternal(name)))
            {
                this.CalculateTranslations();
            }
        }

        /// <summary>
        /// Adds a field to the translator.
        /// </summary>
        /// <param name="name">
        /// The name of the field.
        /// </param>
        /// <returns>
        /// True if the field was added, false if the field was already known.
        /// </returns>
        private bool AddFieldInternal(string name)
        {
            return this.fields.Add(name) && (name.IndexOf('.') == -1
                                                 ? (this.mappedFields.FirstOrDefault(fd => fd.Field == name)?.Mapped.Add(name) ?? false)
                                                 : (this.mappedFields.Where(fd => fd.Field == "*").Take(int.Parse(name.Split('!')[0]) + 1).FirstOrDefault()?.Mapped.Add(name) ?? false));
        }

        /// <summary>
        /// Calculates the translations for the fields. Makes sure that no duplicate fields are returned, and that the aliases
        ///     appear in the correct order.
        /// </summary>
        private void CalculateTranslations()
        {
            var allFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var translatedFields = new List<string>();

            foreach (var field in this.mappedFields.Where(fd => fd.Field != "*").SelectMany(fd => fd.Mapped))
            {
                allFields.Add(field);
                this.mapToInternalName[field] = field;
            }

            foreach (var fieldDeclaration in this.mappedFields)
            {
                if (fieldDeclaration.Field == "*")
                {
                    foreach (var field in fieldDeclaration.Mapped)
                    {
                        var fieldName = field.Split(
                            new[]
                                                        {
                                                            '.',
                                                        }, 2)[1];
                        var suffix = 0;

                        while (!allFields.Add(fieldName + (suffix == 0 ? string.Empty : suffix.ToString())))
                        {
                            suffix++;
                        }

                        this.mapToInternalName[fieldName + (suffix == 0 ? string.Empty : suffix.ToString())] = field;

                        translatedFields.Add(fieldName + (suffix == 0 ? string.Empty : suffix.ToString()));
                    }
                }
                else
                {
                    translatedFields.Add(fieldDeclaration.Field);
                }
            }

            this.Fields = new ReadOnlyCollection<string>(translatedFields);
        }

        /// <summary>
        /// Maps a field from the query to one or more field names in the data set.
        /// </summary>
        private class MappedField
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MappedField"/> class.
            /// </summary>
            /// <param name="field">
            /// he field that was mapped.
            /// </param>
            public MappedField(string field)
            {
                this.Field = field;
            }

            /// <summary>
            /// Gets the field name.
            /// </summary>
            public string Field { get; }

            /// <summary>
            /// Gets the mapped fields.
            /// </summary>
            public HashSet<string> Mapped { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}