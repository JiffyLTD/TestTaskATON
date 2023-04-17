using System.ComponentModel.DataAnnotations;

namespace TestTaskATON.ViewModels
{
    public class UserRegisterViewModel
    {
        [RegularExpression(@"[A-Za-z0-9]{1,50}", ErrorMessage = "Разрешенные символы: A-Za-z0-9")]
        public string Login { get; set; }
        [RegularExpression(@"[A-Za-z0-9]{1,50}", ErrorMessage = "Разрешенные символы: A-Za-z0-9")]
        public string Password { get; set; }
        [RegularExpression(@"[A-Za-zА-Яа-я]{1,50}", ErrorMessage = "Разрешенные символы: A-Za-zА-Яа-я")]
        public string Name { get; set; }
        [RegularExpression(@"[0-2]", ErrorMessage = "Разрешенные символы: 0-2")]
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Admin { get; set; }
        public AdminViewModel AdminModel { get; set; } = null!;
    }
}
