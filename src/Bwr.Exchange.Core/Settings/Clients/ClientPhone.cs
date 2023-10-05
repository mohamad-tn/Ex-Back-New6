using Bwr.Exchange.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Settings.Clients
{
    public class ClientPhone : PhoneBase
    {
        #region Client
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        #endregion 
    }
}
