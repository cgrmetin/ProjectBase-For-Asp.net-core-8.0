namespace ProjectBase.DTO.Auth
{
    public class Status
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
