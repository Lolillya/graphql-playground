using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using graphql_playground.DTOs;
using graphql_playground.GraphQL.Filters;
using graphql_playground.Models;
using graphql_playground.Services;
using graphql_playground.Services.Courses;
using HotChocolate;
using HotChocolate.Data;

namespace graphql_playground.GraphQL.Queries
{
    public class Query
    {
        private readonly CoursesRepository _coursesRepository;

        public Query(CoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
        }

        public async Task<IEnumerable<CourseType>> GetCourses()
        {
            IEnumerable<CourseDTO> courseDTOs = await _coursesRepository.GetAll();
            return courseDTOs.Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId
            });
        }

        [UsePaging(IncludeTotalCount = true, DefaultPageSize = 1)]
        [UseFiltering(typeof(CourseFilterType))]
        public IQueryable<CourseType> GetPaginatedCourses([Service] SchoolDbContext context)
        {
            return context.Courses.Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId
            });
        }

        public async Task<CourseType> GetCourseByIdAsync(Guid id)
        {
            CourseDTO courseDTO = await _coursesRepository.GetById(id);

            return new CourseType()
            {
                Id = courseDTO.Id,
                Name = courseDTO.Name,
                Subject = courseDTO.Subject,
                InstructorId = courseDTO.InstructorId
            };
        }

        [GraphQLDeprecated("query deprecated")]
        public string Hello() => "Hello, GraphQL!";
    }
}