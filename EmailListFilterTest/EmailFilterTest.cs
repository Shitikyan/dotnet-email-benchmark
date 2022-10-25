 using EmailListFilter.Filter;
using EmailListFilter.Helper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static EmailListFilter.Test.TestCases;
using UserRepository = EmailListFilter.Repository.UserRepository;

namespace EmailListFilter.Test
{
    public class EmailFilterTest
    {
        private readonly Context.UserContext userContext = new Context.UserContext();

        [Theory]
        [InlineData(CaseN1, CaseM5)]
        [InlineData(CaseN2, CaseM1)]
        [InlineData(CaseN2, CaseM5)]
        [InlineData(CaseN2, CaseM2)]
        [InlineData(CaseN2, CaseM3)]
        [InlineData(CaseN3, CaseM3)]
        [InlineData(CaseN3, CaseM5)]
        [InlineData(CaseN4, CaseM3)]
        public async void Fetching(int n, int m)
        {
            await userContext.Database.EnsureDeletedAsync();
            await userContext.Database.MigrateAsync();

            var emailsInsert = DataSeed.GetRandomEmails(m);

            var userRepository = new UserRepository(userContext);
            await userRepository.Insert(emailsInsert, TableType.Indexed);
            await userRepository.Insert(emailsInsert, TableType.Unindexed);
            var emailsFetchingIndexed = await UserFilter.Filter(n, m, TableType.Indexed, userRepository, UserFilter.Fetching);
            var emailsFetchingUnindexed = await UserFilter.Filter(n, m, TableType.Unindexed, userRepository, UserFilter.Fetching);

            await userContext.Database.EnsureDeletedAsync();
        }
        [Theory]
        [InlineData(CaseN1, CaseM5)]
        [InlineData(CaseN2, CaseM1)]
        [InlineData(CaseN2, CaseM5)]
        [InlineData(CaseN2, CaseM2)]
        [InlineData(CaseN2, CaseM3)]
        [InlineData(CaseN3, CaseM3)]
        [InlineData(CaseN3, CaseM5)]
        [InlineData(CaseN4, CaseM3)]
        public async void Inserting(int n, int m)
        {
            await userContext.Database.EnsureDeletedAsync();
            await userContext.Database.MigrateAsync();

            var emailsInsert = DataSeed.GetRandomEmails(m);

            var userRepository = new UserRepository(userContext);
            await userRepository.Insert(emailsInsert, TableType.Indexed);
            await userRepository.Insert(emailsInsert, TableType.Unindexed);

            var emailsInsertingIndexed = await UserFilter.Filter(n, m, TableType.Indexed, userRepository, UserFilter.InsertingWithSQL);
            var emailsInsertingUnindexed = await UserFilter.Filter(n, m, TableType.Unindexed, userRepository, UserFilter.InsertingWithSQL);

            await userContext.Database.EnsureDeletedAsync();
        }


        [Theory]
        [InlineData(CaseN1, CaseM5)]
        [InlineData(CaseN2, CaseM1)]
        [InlineData(CaseN2, CaseM5)]
        [InlineData(CaseN2, CaseM2)]
        [InlineData(CaseN2, CaseM3)]
        [InlineData(CaseN3, CaseM3)]
        [InlineData(CaseN3, CaseM5)]
        [InlineData(CaseN4, CaseM3)]
        public async void Intersection(int n, int m)
        {
            await userContext.Database.EnsureDeletedAsync();
            await userContext.Database.MigrateAsync();

            var emailsInsert = DataSeed.GetRandomEmails(m);

            var userRepository = new UserRepository(userContext);
            await userRepository.Insert(emailsInsert, TableType.Indexed);
            await userRepository.Insert(emailsInsert, TableType.Unindexed);

            var emailsIntersectionIndexed = await UserFilter.Filter(n, m, TableType.Indexed, userRepository, UserFilter.IntersectionWithEF);
            var emailsIntersectionUnindexed = await UserFilter.Filter(n, m, TableType.Unindexed, userRepository, UserFilter.IntersectionWithEF);

            await userContext.Database.EnsureDeletedAsync();
        }

        [Theory]
        [InlineData(CaseN1, CaseM5)]
        [InlineData(CaseN2, CaseM1)]
        [InlineData(CaseN2, CaseM5)]
        [InlineData(CaseN2, CaseM2)]
        [InlineData(CaseN2, CaseM3)]
        [InlineData(CaseN3, CaseM3)]
        [InlineData(CaseN3, CaseM5)]
        [InlineData(CaseN4, CaseM3)]
        public async void FetchingCached(int n, int m)
        {
            await userContext.Database.EnsureDeletedAsync();
            await userContext.Database.MigrateAsync();

            var emailsInsert = DataSeed.GetRandomEmails(m);

            var userRepository = new UserRepository(userContext);
            await userRepository.Insert(emailsInsert, TableType.Indexed);
            await userRepository.Insert(emailsInsert, TableType.Unindexed);

            var emailsIntersectionIndexed = await UserFilter.Filter(n, m, TableType.Indexed, userRepository, UserFilter.FetchingCached);
            var emailsIntersectionUnindexed = await UserFilter.Filter(n, m, TableType.Unindexed, userRepository, UserFilter.FetchingCached);

            await userContext.Database.EnsureDeletedAsync();
        }
    }
}