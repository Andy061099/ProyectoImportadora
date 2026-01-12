namespace ImportadoraApi.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Operación exitosa")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                ErrorCode = null,
                Data = data
            };
        }

        public static ApiResponse<T> Fail(string message, string? errorCode = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Data = default
            };
        }
    }
}
