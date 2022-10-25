namespace EmailListFilter.Helper
{
    public static class CSVReader
    {
        public static async Task<IEnumerable<string>> GetEmails(int n)
        {
            var filePath = $"../../../../EmailListFilter/CSV/emails{n}.csv";
            if (!File.Exists(filePath))
            {
                throw new DirectoryNotFoundException("There is no such a file: " + filePath);
            }

            var emails = new List<string>();

            using (var sr = new StreamReader(filePath))
            {
                await sr.ReadLineAsync();
                while (!sr.EndOfStream)
                {
                    var email = await sr.ReadLineAsync();
                    emails.Add(email);
                }
            }

            return emails;
        }
    }
}
