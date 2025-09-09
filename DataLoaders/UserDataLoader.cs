using FirebaseAdmin;
using FirebaseAdmin.Auth;
using graphql_playground.GraphQL.Queries;

namespace graphql_playground.DataLoaders
{
    public class UserDataLoader : BatchDataLoader<string, UserType>
    {
        private const int MAX_FIREBASE_USERS_BATCH_SIZE = 100;
        private readonly FirebaseAuth _firebaseAuth;


        public UserDataLoader(IBatchScheduler batchScheduler, DataLoaderOptions options, FirebaseApp firebaseApp) : base(batchScheduler, options)
        {
            options.MaxBatchSize = MAX_FIREBASE_USERS_BATCH_SIZE;
            _firebaseAuth = FirebaseAuth.GetAuth(firebaseApp);
        }

        protected override async Task<IReadOnlyDictionary<string, UserType>> LoadBatchAsync(IReadOnlyList<string> userdIds, CancellationToken cancellationToken)
        {
            List<UidIdentifier> userIdentifiers = userdIds.Select(id => new UidIdentifier(id)).ToList();

            GetUsersResult usersResult = await _firebaseAuth.GetUsersAsync(userIdentifiers);

            return usersResult.Users.Select(u => new UserType()
            {
                Id = u.Uid,
                Username = u.DisplayName,
                PhotoUrl = u.PhotoUrl
            }).ToDictionary(u => u.Id);
        }
    }

}
