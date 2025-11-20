namespace cl_backend.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public required string Login { get; set; }
        public required string Username { get; set; }
        public required string Role { get; set; }
    }
}
