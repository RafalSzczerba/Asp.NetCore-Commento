using System.ComponentModel;

namespace Commento.Models
{
    public class Payload
    {
        [DisplayName("token")]
        public string token { get; set; }
        [DisplayName("email")]
        public string email { get; set; }
        [DisplayName("name")]
        public string name { get; set; }
        [DisplayName("link")]
        public string link { get; set; }
        [DisplayName("photo")]
        public string photo { get; set; }
    }
}
