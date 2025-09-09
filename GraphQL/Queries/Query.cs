using graphql_playground.Services;
using Microsoft.EntityFrameworkCore;

namespace graphql_playground.GraphQL.Queries
{
    public class Query
    {
        public async Task<IEnumerable<ISearchResultType>> Search(string term, [Service] SchoolDbContext schoolDbContext)
        {
            IEnumerable<CourseType> courses = await schoolDbContext.Courses.Where(c => c.Name.Contains(term)).Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId,
                CreatorId = c.CreatorId
            }).ToListAsync();

            IEnumerable<InstructorType> instructors = await schoolDbContext.Instructors
            .Where(i => i.FirstName.Contains(term) || i.LastName.Contains(term))
            .Select(i => new InstructorType()
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Salary = i.Salary

            }).ToListAsync();

            return new List<ISearchResultType>()
            .Concat(courses)
            .Concat(instructors);
        }

        [GraphQLDeprecated("query deprecated")]
        public string Hello() => "Hello, GraphQL!";
    }
}
