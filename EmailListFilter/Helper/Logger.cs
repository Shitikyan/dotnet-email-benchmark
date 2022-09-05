namespace EmailListFilter.Helper
{
    public static class Logger
    {
        const string OutputPath = Constants.OutputFilePath;

        public static void EmptyOutputFile()
        {
            File.WriteAllText(OutputPath, "");
        }

        public static void Append(string text)
        {
            File.AppendAllText(OutputPath, text);
        }

        public static void BenchmarkLog(string algorithmName, TableType tableType, int n, int m, TimeSpan time)
        {
            Append($"{algorithmName} {tableType}: n = {n}, m = {m} --> Time: {time}\n");
        }
    }
}
