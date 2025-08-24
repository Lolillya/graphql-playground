using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace graphql_playground.GraphQL.Queries
{
    public class StudentType
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double GPA { get; set; }
    }
}