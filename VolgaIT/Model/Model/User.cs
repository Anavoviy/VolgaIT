﻿namespace VolgaIT.Model.Model
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public double Balance { get; set; }
    }
}
