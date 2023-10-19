namespace VolgaIT.Model.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;
        public double Balance { get; set; } = 0;
    }
}
