namespace BepKhoiBackend.Shared.Helpers
{
    /* Create result method to return for flag check
    *  error, manage exception, using for API, perform 
    *  for Microservices architecture*/
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        private Result(bool isSuccess, string message, T data)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public static Result<T> Success(T data, string message = "")
        {
            return new Result<T>(true, message, data);
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T>(false, message, default);
        }
    }
}
