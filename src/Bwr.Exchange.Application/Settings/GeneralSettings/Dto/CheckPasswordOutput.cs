namespace Bwr.Exchange.Settings.GeneralSettings.Dto
{
    public class CheckPasswordOutput
    {
        public CheckPasswordOutput(bool success)
        {
            Success = success;
        }
        public bool Success { get; set; }
    }
}
