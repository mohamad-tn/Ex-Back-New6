namespace Bwr.Exchange.Models.Migrate
{
    public class ClearDatabaseOutput
    {
        public ClearDatabaseOutput(bool success)
        {
            Success = success;
        }

        public bool Success { get; set; }
    }
}
