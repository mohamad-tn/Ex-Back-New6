using Abp.Domain.Entities;

namespace Bwr.Exchange.Shared
{
    public class FileUploadBase: Entity
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
    }
}
