namespace ProjectBase.Entity.Response
{
    public class GenericResponse
    {
        public bool IsError { get; set; }
        public ApiError ApiError { get; set; }
        public object Response { get; set; }
    }
}
