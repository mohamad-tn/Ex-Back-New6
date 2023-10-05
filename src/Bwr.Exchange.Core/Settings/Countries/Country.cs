using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;

namespace Bwr.Exchange.Settings.Countries
{
    public class Country : FullAuditedEntity
    {
        public Country()
        {
            Provinces = new List<Province>();
        }
        public string Name { get; set; }

        public virtual IList<Province> Provinces { get; set; }
    }
}
