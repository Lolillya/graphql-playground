using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using graphql_playground.GraphQL.Mutations;
using graphql_playground.GraphQL.Queries;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace graphql_playground.GraphQL.Subscriptions
{
    public class Subscription
    {
        [Subscribe]
        public CourseResult CourseCreated([EventMessage] CourseResult course) => course;

        // Stream provider (no attribute)
        public ValueTask<ISourceStream<CourseResult>> SubscribeToCourseUpdated(
            Guid courseId,
            [Service] ITopicEventReceiver receiver)
        {
            var topic = $"{courseId}_{nameof(CourseUpdated)}";
            return receiver.SubscribeAsync<CourseResult>(topic);
        }

        // Payload resolver (this is the GraphQL field)
        [Subscribe(With = nameof(SubscribeToCourseUpdated))]
        public CourseResult CourseUpdated([EventMessage] CourseResult course) => course;


    }
}