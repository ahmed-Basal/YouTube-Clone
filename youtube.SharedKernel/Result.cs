namespace youtube.SharedKernel
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public int StatusCode { get; }

        protected Result(bool isSuccess, string error = null, int statusCode = 200)
        {
            IsSuccess = isSuccess;
            Error = error;
            StatusCode = statusCode;
        }

        public static Result Success(int statusCode = 200) => new(true, null, statusCode);
        public static Result Failure(string error, int statusCode = 400) => new(false, error, statusCode);
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        protected Result(T value, bool isSuccess, string error = null, int statusCode = 200)
            : base(isSuccess, error, statusCode)
        {
            Value = value;
        }

        public static Result<T> Success(T value, int statusCode = 200) => new(value, true, null, statusCode);
        public static new Result<T> Failure(string error, int statusCode = 400) => new(default, false, error, statusCode);
    }
}
