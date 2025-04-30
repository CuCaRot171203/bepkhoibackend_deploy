namespace BepKhoiBackend.BusinessObject.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool? IsVerify { get; set; }
        public string Password { get; set; } = null!;
        public string RoleName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

    }

}
