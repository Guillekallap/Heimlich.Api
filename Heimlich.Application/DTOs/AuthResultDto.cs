namespace Heimlich.Application.DTOs
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
        public string Error { get; set; }
    }
}