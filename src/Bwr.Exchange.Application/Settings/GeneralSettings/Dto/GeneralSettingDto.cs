using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.GeneralSettings.Dto
{
    public class GeneralSettingDto : EntityDto
    {
        public string CompanyName { get; set; }
        public string DbSuffex { get; set; }
        public string EditPassword { get; set; }
    }
}
