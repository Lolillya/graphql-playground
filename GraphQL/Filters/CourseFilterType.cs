using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using graphql_playground.GraphQL.Queries;
using HotChocolate.Data.Filters;

namespace graphql_playground.GraphQL.Filters
{
    public class CourseFilterType : FilterInputType<CourseType>
    {
        protected override void Configure(IFilterInputTypeDescriptor<CourseType> descriptor)
        {
            descriptor.Ignore(c => c.Students);
            
            base.Configure(descriptor);
        }
    }
}