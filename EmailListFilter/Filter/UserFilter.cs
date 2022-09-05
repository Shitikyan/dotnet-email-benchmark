using EmailListFilter.Helper;
using EmailListFilter.Repository;
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

            BenchmarkLog(tableType, DbLength, CsvLength, watch.Elapsed);
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
            BenchmarkLog(tableType, DbLength, CsvLength, watch.Elapsed);
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

            BenchmarkLog(tableType, CsvLength, DbLength, watch.Elapsed);
            return result;
        }
    }
}
