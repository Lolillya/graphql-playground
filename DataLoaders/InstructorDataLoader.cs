using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using graphql_playground.DTOs;
using graphql_playground.GraphQL.Queries;
using graphql_playground.Services.Instructors;

namespace graphql_playground.DataLoaders
{
    public class InstructorDataLoader : BatchDataLoader<Guid, InstructorDTO>
    {
        private readonly InstructorsRepository _instructorsRepository;

        public InstructorDataLoader(IBatchScheduler batchScheduler, DataLoaderOptions options, InstructorsRepository instructorsRepository) : base(batchScheduler, options)
        {
            _instructorsRepository = instructorsRepository;
        }

        protected override async Task<IReadOnlyDictionary<Guid, InstructorDTO>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
        {
            IEnumerable<InstructorDTO> instructors = await _instructorsRepository.GetManyByIds(keys);

            return instructors.ToDictionary(i => i.Id);
        }
    }
}