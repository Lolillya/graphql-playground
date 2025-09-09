using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace graphql_playground.GraphQL.Queries
{
    [InterfaceType("SearchResult")]
    public interface ISearchResultType
    {
        Guid Id { get; }
    }
}