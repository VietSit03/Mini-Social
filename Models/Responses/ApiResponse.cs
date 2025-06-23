namespace MiniSocialAPI.Models.Responses
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }

    public class ErrorResponse : ApiResponse
    {
        public List<string> Errors { get; set; } = new List<string>();
        public string ErrorCode { get; set; }
        public string StackTrace { get; set; } // only use develop enviroment
    }

    public class ValidationErrorResponse : ErrorResponse
    {
        public Dictionary<string, List<string>> FieldErrors { get; set; } = new Dictionary<string, List<string>>();
    }

    public class SuccessResponse : ApiResponse
    {
        public SuccessResponse()
        {
            Success = true;
        }

        public SuccessResponse(string message)
        {
            Success = true;
            Message = message;
        }
    }

    public class FailureResponse : ApiResponse
    {
        public FailureResponse()
        {
            Success = false;
        }

        public FailureResponse(string message)
        {
            Success = false;
            Message = message;
        }
    }
}
