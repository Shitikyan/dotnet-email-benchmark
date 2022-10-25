using EmailListFilter.Helper;
using EmailListFilter.Redis;
using EmailListFilter.Repository;
using Newtonsoft.Json;
using System.Diagnostics;
using static EmailListFilter.Helper.Logger;

namespace EmailListFilter.Filter
{
    public delegate Task<IEnumerable<string>> FilterFunction(IEnumerable<string> emails, TableType table, UserRepository userRepository);

    public static class UserFilter
    {
        private static int DbLength;
        private static int CsvLength;

        static UserFilter()
        {
            EmptyOutputFile();
        }

        public static async Task<IEnumerable<string>> Filter(int n, int m, TableType table, UserRepository userRepository, FilterFunction filterFunc)
        {
            CsvLength = n;
            DbLength = m;
            var emails = await CSVReader.GetEmails(CsvLength);
            var emailsFiltered = await filterFunc(emails, table, userRepository);
            return emailsFiltered;
        }

        /// <summary>
        /// Fetching all data from DB and compare
        /// </summary>
        /// <param name="emails"></param>
        /// <param name="tableType"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<IEnumerable<string>> Fetching(IEnumerable<string> emails, TableType tableType, UserRepository userRepository)
        {
            HashSet<string> csvEmailsSet = new HashSet<string>(emails);
            var watch = new Stopwatch();
            watch.Start();
            HashSet<string> dbEmailsSet = await userRepository.GetEmails(tableType);

            var result = csvEmailsSet.Except(dbEmailsSet);
            watch.Stop();

            BenchmarkLog(nameof(Fetching) ,tableType, DbLength, CsvLength, watch.Elapsed);
            return result;
        }

        /// <summary>
        /// Inserting Emails into Database and then join to get relative users
        /// </summary>
        /// <param name="emails"></param>
        /// <param name="tableType"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static async Task<IEnumerable<string>> InsertingWithSQL(IEnumerable<string> emails, TableType tableType, UserRepository userRepository)
        {
            var watch = new Stopwatch();
            watch.Start();
            var result = await userRepository.GetResultEmailsRaw(emails, tableType);
            watch.Stop();
            BenchmarkLog(nameof(InsertingWithSQL), tableType, DbLength, CsvLength, watch.Elapsed);
            return result;
        }

        /// <summary>
        /// Intersection of user emails in EF
        /// </summary>
        /// <param name="emails"></param>
        /// <param name="tableType"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static async Task<IEnumerable<string>> IntersectionWithEF(IEnumerable<string> emails, TableType tableType, UserRepository userRepository)
        {
            HashSet<string> csvEmailsSet = new HashSet<string>(emails);
            var watch = new Stopwatch();
            watch.Start();
            HashSet<string> dbEmailsSet = await userRepository.GetEmailsFiltered(emails, tableType);
            var result = csvEmailsSet.Except(dbEmailsSet);
            watch.Stop();

            BenchmarkLog(nameof(IntersectionWithEF), tableType, CsvLength, DbLength, watch.Elapsed);
            return result;
        }

        /// <summary>
        /// Fetching all data from cash if exists otherwise from DB
        /// </summary>
        /// <param name="emails"></param>
        /// <param name="tableType"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<IEnumerable<string>> FetchingCached(IEnumerable<string> emails, TableType tableType, UserRepository userRepository)
        {
            var watch = new Stopwatch();
            IEnumerable<string> result;
            watch.Start();
            var cache = RedisConnector.Connection.GetDatabase();
            var key = $"Fetching-{CsvLength}-{DbLength}";
            if (cache.KeyExists(key))
            {
                var cachedValue = await cache.StringGetAsync(key);
                result = JsonConvert.DeserializeObject<IEnumerable<string>>(cachedValue);
                watch.Stop();
            }
            else
            {
                var csvEmailsSet = new HashSet<string>(emails);
                var dbEmailsSet = await userRepository.GetEmailsFiltered(emails, tableType);
                result = csvEmailsSet.Except(dbEmailsSet);
                cache.StringSet(key, JsonConvert.SerializeObject(result));
                watch.Stop();
            }

            BenchmarkLog(nameof(FetchingCached), tableType, CsvLength, DbLength, watch.Elapsed);
            return result;
        }
    }
}
