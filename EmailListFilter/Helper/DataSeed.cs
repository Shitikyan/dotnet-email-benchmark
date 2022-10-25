namespace EmailListFilter.Helper
{
    public static class DataSeed
    {
        private static readonly Random random = new Random();
        private const string chars = "abcdefghijklmnopqrstuvwxyz";

        public static IEnumerable<string> GetRandomEmails(int n)
        {
            var emails = new List<string>();
            for (int i = 1; i <= n; i++)
            {
                var randomString = String.Concat(Enumerable.Repeat(chars, 10)
                    .Select(s => s[random.Next(s.Length)]));
                var email = randomString + "@gmail.com";
                emails.Add(email);
            }
            return emails;
        }
    }
}
