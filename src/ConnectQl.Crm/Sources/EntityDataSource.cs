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

namespace ConnectQl.Crm.Sources
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Tooling.Connector;

    /// <summary>
    /// The entity data source.
    /// </summary>
    public class EntityDataSource : IDataSource, IDescriptableDataSource
    {
        /// <summary>
        /// The entity name.
        /// </summary>
        private readonly string entityName;

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The client.
        /// </summary>
        private CrmServiceClient client;

        /// <summary>
        /// Initializes static members of the <see cref="EntityDataSource"/> class.
        /// </summary>
        static EntityDataSource()
        {
            typeof(OrganizationServiceProxy).GetField("_xrmSdkAssemblyFileVersion", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, "8.2.0.362");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDataSource"/> class.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        public EntityDataSource(string entityName)
            : this(entityName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDataSource"/> class.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="connectionString">The connection string.</param>
        public EntityDataSource(string entityName, string connectionString)
        {
            this.entityName = entityName;
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Gets the descriptor for this data source.
        /// </summary>
        /// <param name="sourceAlias">The data source source alias.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public async Task<IDataSourceDescriptor> GetDataSourceDescriptorAsync(string sourceAlias, Interfaces.IExecutionContext context)
        {
            return await Task.Run(() =>
            {
                var metadata = ((RetrieveEntityResponse)this.GetService(context).Execute(new RetrieveEntityRequest
                {
                    LogicalName = this.entityName,
                    EntityFilters = EntityFilters.Attributes
                })).EntityMetadata;

                return Descriptor.ForDataSource(
                    sourceAlias,
                    metadata.Attributes.Select(a => Descriptor.ForColumn(a.LogicalName, ToType(a.AttributeType), a.DisplayName?.UserLocalizedLabel?.Label)).Where(a => a.Type != null),
                    false);
            });
        }

        /// <summary>
        /// Retrieves the data from the source as an <see cref="T:ConnectQl.AsyncEnumerables.IAsyncEnumerable`1" />.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="rowBuilder">The row builder.</param>
        /// <param name="query">The query expression. Can be <c>null</c>.</param>
        /// <returns>
        /// A task returning the data set.
        /// </returns>
        public IAsyncEnumerable<Row> GetRows(Interfaces.IExecutionContext context, IRowBuilder rowBuilder, IQuery query)
        {
            return context.CreateEmptyAsyncEnumerable<Row>();
        }

        /// <summary>
        /// Converts an <see cref="AttributeTypeCode"/> to a <see cref="Type"/>.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The type.</returns>
        private static Type ToType(AttributeTypeCode? attributeType)
        {
            switch (attributeType)
            {
                case AttributeTypeCode.Boolean: return typeof(bool);
                case AttributeTypeCode.DateTime: return typeof(DateTime);
                case AttributeTypeCode.Money: return typeof(Money);
                case AttributeTypeCode.Decimal: return typeof(decimal);
                case AttributeTypeCode.Double: return typeof(double);
                case AttributeTypeCode.Integer: return typeof(int);
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Customer: return typeof(EntityReference);
                case AttributeTypeCode.Virtual:
                case AttributeTypeCode.EntityName:
                case AttributeTypeCode.String:
                case AttributeTypeCode.Memo: return typeof(string);
                case AttributeTypeCode.PartyList: return typeof(Entity[]);
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                case AttributeTypeCode.Picklist: return typeof(OptionSetValue);
                case AttributeTypeCode.Uniqueidentifier: return typeof(Guid);
                case AttributeTypeCode.CalendarRules: return typeof(object);
                case AttributeTypeCode.BigInt: return typeof(long);
                case AttributeTypeCode.ManagedProperty: return typeof(BooleanManagedProperty);
                default: return null;
            }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The organization service.</returns>
        private IOrganizationService GetService(Interfaces.IExecutionContext context)
        {
            var result = this.client ?? (this.client = new CrmServiceClient(this.connectionString ?? (string)context.GetDefault("connectionstring", this)));

            if (!result.IsReady)
            {
                throw new Exception($"Unable to connect to CRM: {result.LastCrmError}.");
            }

            return result;
        }
    }
}
