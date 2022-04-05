using System.ComponentModel;

namespace Commento.Models
{
    public class LoginPage
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        public string Email { get; set; }

    }
}
