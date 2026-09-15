using System.ComponentModel.DataAnnotations;

namespace ST10438767_CLDV6212.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Customer_Email { get; set; }

        [Required]
        [DataType(DataType.Password)] //do it so the passowrd is not visible bc the model controls what you see on the screen
        public string Password { get; set; }
    }
}
