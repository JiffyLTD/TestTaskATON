﻿using System.ComponentModel.DataAnnotations;

namespace TestTaskATON.ViewModels
{
    public class UpdateUserViewModel
    {
        [RegularExpression(@"[A-Za-z0-9]{1,50}", ErrorMessage = "Разрешенные символы: A-Za-z0-9")]
        public string Login { get; set; } = null!;
        [RegularExpression(@"[A-Za-zА-Яа-я]{1,50}", ErrorMessage = "Разрешенные символы: A-Za-zА-Яа-я")]
        public string Name { get; set; } = null!;
        [RegularExpression(@"[0-2]", ErrorMessage = "Разрешенные символы: 0-2")]
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public SignInViewModel User { get; set; } = null!;
    }
}
