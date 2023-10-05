namespace Bwr.Exchange.Settings.GeneralSettings.Dto
{
    public class MigrationOutputDto
    {
        public MigrationOutputDto(bool success)
        {
            Success = success;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
