using graphql_playground.GraphQL.Subscriptions;
using HotChocolate.Subscriptions;
using graphql_playground.Services.Courses;
using graphql_playground.DTOs;
using HotChocolate.Authorization;
using System.Security.Claims;
using FirebaseAdminAuthentication.DependencyInjection.Models;
using graphql_playground.Validators;
using System.ComponentModel.DataAnnotations;
using AppAny.HotChocolate.FluentValidation;

namespace graphql_playground.GraphQL.Mutations
{
    public class Mutation
    {
        private readonly CoursesRepository _coursesRepository;

        public Mutation(CoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
            
        }

        // [Authorize]
        public async Task<CourseResult> CreateCourse([UseFluentValidation, UseValidator<CourseTypeInputValidator>] CourseInputType courseInput, [Service] ITopicEventSender topicEventSender, ClaimsPrincipal claimsPrincipal)
        {
            

            string? userId = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.ID);

            CourseDTO courseDTO = new CourseDTO()
            {
                Name = courseInput.Name,
                Subject = courseInput.Subject,
                InstructorId = courseInput.InstructorId,
                CreatorId = userId
            };

            courseDTO = await _coursesRepository.Create(courseDTO);

            CourseResult course = new CourseResult()
            {
                Id = courseDTO.Id,
                Name = courseDTO.Name,
                Subject = courseDTO.Subject,
                InstructorId = courseDTO.InstructorId

            };

            await topicEventSender.SendAsync(nameof(Subscription.CourseCreated), course);
            return course;

        }

        [Authorize]
        public async Task<CourseResult> UpdateCourse([UseFluentValidation, UseValidator<CourseTypeInputValidator>] Guid id, CourseInputType courseInput, [Service] ITopicEventSender topicEventSender, ClaimsPrincipal claimsPrincipal)
        {
            
            
            string? userId = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.ID);

            CourseDTO courseDTO = await _coursesRepository.GetById(id);

            if (courseDTO == null)
            {
                throw new GraphQLException(new Error("Course not found", "COURSE_NOT_FOUND"));
            }

            if (courseDTO.CreatorId != userId)
            {
                throw new GraphQLException(new Error("You do not have permission to update this course", "INVALID_PERMISSON"));
            }

            courseDTO.Id = id;
            courseDTO.Name = courseInput.Name;
            courseDTO.Subject = courseInput.Subject;
            courseDTO.InstructorId = courseInput.InstructorId;

            courseDTO = await _coursesRepository.Update(courseDTO);

            CourseResult course = new CourseResult()
            {
                Id = courseDTO.Id,
                Name = courseDTO.Name,
                Subject = courseDTO.Subject,
                InstructorId = courseDTO.InstructorId

            };

            string updateCourseTopic = $"{course.Id}_{nameof(Subscription.CourseUpdated)}";
            await topicEventSender.SendAsync(updateCourseTopic, course);

            return course;
        }

        [Authorize(Policy = "IsAdmin")]
        public async Task<bool> DeleteCourse(Guid id)
        {
            try
            {
                return await _coursesRepository.Delete(id);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
