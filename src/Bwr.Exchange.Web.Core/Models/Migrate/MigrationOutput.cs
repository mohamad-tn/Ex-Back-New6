namespace Bwr.Exchange.Models.Migrate
{
    public class MigrationOutput
    {
        public MigrationOutput(bool success)
        {
            Success = success;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
