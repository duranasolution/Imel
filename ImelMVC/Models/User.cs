﻿namespace ImelMVC.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = "User";
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
        public int VersionNum { get; set; }
        public int isDeleted { get; set; }
    }
}
