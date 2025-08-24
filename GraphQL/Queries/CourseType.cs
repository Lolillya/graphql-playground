using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using graphql_playground.DataLoaders;
using graphql_playground.DTOs;
using graphql_playground.Models;
using graphql_playground.Services.Instructors;

namespace graphql_playground.GraphQL.Queries
{
    public class CourseType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }                 // now nullable in schema
        public Subject Subject { get; set; }
        [IsProjected(true)]
        public Guid InstructorId { get; set; }
        [GraphQLNonNullType]
        public async Task<InstructorType> Instructor([Service] InstructorDataLoader instructorDataLoader)
        {
            InstructorDTO instructorDTO = await instructorDataLoader.LoadAsync(InstructorId, CancellationToken.None);

            return new InstructorType()
            {

                Id = instructorDTO.Id,
                FirstName = instructorDTO.FirstName,
                LastName = instructorDTO.LastName,
                Salary = instructorDTO.Salary,
            };
        }
        public IEnumerable<StudentType>? Students { get; set; }


    }
}