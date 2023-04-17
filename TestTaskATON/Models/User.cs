using System.ComponentModel.DataAnnotations;

namespace TestTaskATON.Models
{
    public class User
    {
        public User(string login, string password, string name, int gender, DateTime? birthday, bool admin, string createdBy)
        {
            Guid = Guid.NewGuid();
            Login = login;
            Password = password;
            Name = name;
            Gender = gender;
            Birthday = birthday;
            Admin = admin;
            CreatedOn = DateTime.Now;
            CreatedBy = createdBy;
        }
        [Key]
        public Guid Guid { get; private set; }
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
        public DateTime CreatedOn { get; private set; }
        public string CreatedBy { get; private set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? RevokedOn { get; set; }
        public string? RevokedBy { get; set; }
    }
}
