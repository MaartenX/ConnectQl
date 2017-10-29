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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Tooling.Connector;
    using IExecutionContext = Interfaces.IExecutionContext;

    /// <summary>
    /// The entity data source.
    /// </summary>
    public class EntityDataSource : IDataSource, ISupportsJoin, IDescriptableDataSource
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
        /// Ugly hack to make XRM work when using this data source from VS (the Microsoft.Xrm.Sdk.dll assembly is not loaded yet).
        /// </summary>
        static EntityDataSource()
        {
            typeof(OrganizationServiceProxy).GetField("_xrmSdkAssemblyFileVersion", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, typeof(Entity).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
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
        /// Gets the types of data sources that are supported when joining.
        /// </summary>
        public IReadOnlyList<Type> SupportedDataSourceTypes { get; } = new[] { typeof(EntityDataSource) };

        /// <summary>
        /// Gets the descriptor for this data source.
        /// </summary>
        /// <param name="sourceAlias">The data source source alias.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public async Task<IDataSourceDescriptor> GetDataSourceDescriptorAsync(string sourceAlias, IExecutionContext context)
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
                    metadata.Attributes.Select(a => Descriptor.ForColumn(a.LogicalName, EntityDataSource.ToType(a.AttributeType), a.DisplayName?.UserLocalizedLabel?.Label)).Where(a => a.Type != null));
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
        public IAsyncEnumerable<Row> GetRows(IExecutionContext context, IRowBuilder rowBuilder, IQuery query)
        {
            this.GetQueryExpression(query.GetFilter(context));

            return context.CreateEmptyAsyncEnumerable<Row>();
        }

        /// <summary>
        /// Checks if this data source supports the join query.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <param name="query">
        /// The query to check.
        /// </param>
        /// <returns><c>true</c> if the <see cref="ISupportsJoin"/> supports the specified <paramref name="query"/>, <c>false</c> otherwise.</returns>
        bool ISupportsJoin.SupportsJoinQuery(IExecutionContext context, IJoinQuery query)
        {
            return false;
        }

        /// <summary>
        /// Retrieves the data from multiple sources as an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="rowBuilder">
        /// The row builder.
        /// </param>
        /// <param name="query">
        /// The join query expression. Can be <c>null</c>.
        /// </param>
        /// <returns>
        /// A task returning the data set.
        /// </returns>
        IAsyncEnumerable<Row> ISupportsJoin.GetRows(IExecutionContext context, IRowBuilder rowBuilder, IJoinQuery query)
        {
            throw new NotSupportedException("Join support is not yet implemented.");
        }

        /// <summary>
        /// Converts an <see cref="AttributeTypeCode"/> to a <see cref="Type"/>.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The type.</returns>
        [CanBeNull]
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
        /// Gets the query expression.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="QueryExpression"/>.
        /// </returns>
        [CanBeNull]
        private QueryExpression GetQueryExpression(Expression filter)
        {
            //var f = new GenericVisitor { (CompareExpression c) => c }.Visit(filter);

            return null;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The organization service.</returns>
        [NotNull]
        private IOrganizationService GetService(IExecutionContext context)
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
