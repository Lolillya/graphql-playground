using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using graphql_playground.GraphQL.Queries;
using graphql_playground.Models;

namespace graphql_playground.GraphQL.Mutations
{
    public class CourseInputType
    {
        public string Name { get; set; }
        public Subject Subject { get; set; }
        public Guid InstructorId { get; set; }
    }
}