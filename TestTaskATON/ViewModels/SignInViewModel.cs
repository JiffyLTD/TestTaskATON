using System.ComponentModel.DataAnnotations;

namespace TestTaskATON.ViewModels
{
    public class SignInViewModel
    {
        [RegularExpression(@"[A-Za-z0-9]{1,50}", ErrorMessage = "Разрешенные символы: A-Za-z0-9")]
        public string Login { get; set; } = null!;
        [RegularExpression(@"[A-Za-z0-9]{1,50}", ErrorMessage = "Разрешенные символы: A-Za-z0-9")]
        public string Password { get; set; } = null!;
    }
}
