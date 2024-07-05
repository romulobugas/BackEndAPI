namespace BackEndAPI.Models
{
    public class JwtPayload
    {
        public string Validator { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string String { get; set; }
    }
}
