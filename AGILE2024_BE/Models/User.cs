namespace AGILE2024_BE.Models
{
    public class User
    {
        public int Id_user { get; set; }
        public Role Role { get; set; }
        public User? Superior { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string? Title_after { get; set; }
        public string? Title_before { get; set; }
        public byte[] Salt { get; set; }
        public string Password { get; set; }
    }
}
