namespace AGILE2024_BE.Data.Requests
{
    public class RegisterRequestCustom
    {
        //TODO add id_superior, title_before, title_after, possibly id_role?
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string ConfirmPassword { get; init; }
        public required string Name { get; init; }
        public required string Surname { get; init; }
        public required int Id_Role { get; init; }
        public string? Title_Before { get; init; }
        public string? Title_After { get; init; }
        public int Id_Superior { get; init; }
    }
}
