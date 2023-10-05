using Bwr.Exchange.Settings.Countries.Dto.Provinces;

namespace Bwr.Exchange.Settings.Clients.Dto
{
    public class ReadClientDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public bool activated { get; set; }
        public int branchId { get; set; }
        public int provinceId { get; set; }
        public ReadProvinceDto province { get; set; }

    }
}
