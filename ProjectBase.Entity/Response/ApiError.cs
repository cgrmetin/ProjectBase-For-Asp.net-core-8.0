namespace ProjectBase.Entity.Response
{
    public class ApiError
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Detail { get; set; }
    }
}
