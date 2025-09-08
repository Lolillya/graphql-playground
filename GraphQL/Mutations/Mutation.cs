using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using graphql_playground.GraphQL.Queries;
using graphql_playground.GraphQL.Subscriptions;
using HotChocolate.Subscriptions;
using HotChocolate.Execution;
using graphql_playground.Services.Courses;
using graphql_playground.DTOs;
using HotChocolate.Authorization;
using System.Security.Claims;
using FirebaseAdminAuthentication.DependencyInjection.Models;

namespace graphql_playground.GraphQL.Mutations
{
    public class Mutation
    {
        private readonly CoursesRepository _coursesRepository;

        public Mutation(CoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
        }

        [Authorize]
        public async Task<CourseResult> CreateCourse(CourseInputType courseInput, [Service] ITopicEventSender topicEventSender, ClaimsPrincipal claimsPrincipal)
        {
            string? userId = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.ID);
            string? userEmail = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.EMAIL);
            string? userName = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.USERNAME);
            string? verified = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.EMAIL_VERIFIED);




            CourseDTO courseDTO = new CourseDTO()
            {
                Name = courseInput.Name,
                Subject = courseInput.Subject,
                InstructorId = courseInput.InstructorId
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
        public async Task<CourseResult> UpdateCourse(Guid id, CourseInputType courseInput, [Service] ITopicEventSender topicEventSender)
        {
            CourseDTO courseDTO = new CourseDTO()
            {
                Id = id,
                Name = courseInput.Name,
                Subject = courseInput.Subject,
                InstructorId = courseInput.InstructorId
            };

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
