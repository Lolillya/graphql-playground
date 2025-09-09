using graphql_playground.DTOs;
using graphql_playground.GraphQL.Filters;
using graphql_playground.GraphQL.Sorters;
using graphql_playground.Services;
using graphql_playground.Services.Courses;
using HotChocolate.Authorization;

namespace graphql_playground.GraphQL.Queries
{
    public class Query
    {
        private readonly CoursesRepository _coursesRepository;

        public Query(CoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
        }

        [Authorize]
        [UsePaging(IncludeTotalCount = true, DefaultPageSize = 1)]
        [UseProjection]
        [UseFiltering(typeof(CourseFilterType))]
        [UseSorting(typeof(CourseSortType))]
        public IQueryable<CourseType> GetCourses([Service] SchoolDbContext context)
        {
            return context.Courses.Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId,
                CreatorId = c.CreatorId
            });
        }

        [Authorize]
        public async Task<CourseType> GetCourseByIdAsync(Guid id)
        {
            CourseDTO courseDTO = await _coursesRepository.GetById(id);

            return new CourseType()
            {
                Id = courseDTO.Id,
                Name = courseDTO.Name,
                Subject = courseDTO.Subject,
                InstructorId = courseDTO.InstructorId,
                CreatorId = courseDTO.CreatorId
            };
        }

        [GraphQLDeprecated("query deprecated")]
        public string Hello() => "Hello, GraphQL!";
    }
}
