namespace MKIdentityServer.Identity.Models
{
    public class AuthDto
    {
        public string? UserName { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? Provider { get; set; } = string.Empty;
        public string? Client_Id { get; set; } = string.Empty;
        public string? IdToken { get; set; } = string.Empty;
    }

}
