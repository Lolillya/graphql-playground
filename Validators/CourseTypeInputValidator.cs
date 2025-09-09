using FluentValidation;
using graphql_playground.GraphQL.Mutations;

namespace graphql_playground.Validators
{
    public class CourseTypeInputValidator : AbstractValidator<CourseInputType>
    {
        public CourseTypeInputValidator()
        {
            RuleFor(c => c.Name)
                .MinimumLength(3)
                .MaximumLength(50)
                .WithMessage("Course name must be between 3 and 50 characters.")
                .WithErrorCode("COURSE_NAME_LENGTH");
            
        }   
    }

}