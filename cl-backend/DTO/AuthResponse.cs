namespace cl_backend.DTO
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDTO? User { get; set; }
        public string? Token { get; set; }
    }
}
