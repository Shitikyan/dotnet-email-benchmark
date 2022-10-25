using EmailListFilter.Context;
using EmailListFilter.Entity;
using EmailListFilter.Helper;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace EmailListFilter.Repository
{
    public class UserRepository
    {
        private readonly UserContext _dbContext;

        public UserRepository(UserContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Insert(IEnumerable<string> emails, TableType tableType)
        {
            if (tableType == TableType.Indexed)
            {
                await _dbContext.UsersIndexed.AddRangeAsync(emails.Select(x => new UserIndexed { Email = x, Name = "undefined" }));
            }
            else if (tableType == TableType.Unindexed)
            {
                await _dbContext.UsersUnindexed.AddRangeAsync(emails.Select(x => new UserUnindexed { Email = x, Name = "undefined" }));
            }
            else
            {
                throw new Exception("Undefined table");
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<HashSet<string>> GetEmails(TableType tableType)
        {
            if (tableType == TableType.Indexed)
            {
                return (await _dbContext.UsersIndexed.Select(x => x.Email).ToListAsync()).ToHashSet();
            }
            else if (tableType == TableType.Unindexed)
            {
                return (await _dbContext.UsersUnindexed.Select(x => x.Email).ToListAsync()).ToHashSet();
            }
            else
            {
                throw new Exception("Undefined table");
            }
        }

        public async Task<HashSet<string>> GetEmailsFiltered(IEnumerable<string> emails, TableType tableType)
        {
            if (tableType == TableType.Indexed)
            {
                return (await _dbContext.UsersIndexed.Where(x => emails.Contains(x.Email)).Select(x => x.Email).ToListAsync()).ToHashSet();
            }
            else if (tableType == TableType.Unindexed)
            {
                return (await _dbContext.UsersUnindexed.Where(x => emails.Contains(x.Email)).Select(x => x.Email).ToListAsync()).ToHashSet();
            }
            else
            {
                throw new Exception("Undefined table");
            }
        }


        public async Task<List<string>> GetResultEmailsRaw(IEnumerable<string> emails, TableType tableType)
        {
            string tableName;
            if (tableType == TableType.Indexed)
            {
                tableName = "UsersIndexed";
            }
            else if (tableType == TableType.Unindexed)
            {
                tableName = "UsersUnindexed";
            }
            else
            {
                throw new NotSupportedException("Undefined table");
            }

            var insertRows = emails.Aggregate("", (x, y) => x + $",ROW('{y}')").Substring(1);
            var result = new List<string>();
            using (MySqlConnection cn = new MySqlConnection(Constants.ConnectionString))
            {
                cn.Open();

                var cmd = new MySqlCommand("SELECT *" +
                             $"FROM (VALUES {insertRows}" +
                             ") t1 (email) " +
                             $"LEFT OUTER JOIN {tableName} ON t1.email={tableName}.email " +
                             $"WHERE {tableName}.email IS NULL"
                             , cn);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var item = reader.GetValue(0).ToString();
                    if (item != null)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }
    }
}
