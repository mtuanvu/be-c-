namespace five_birds_be.Response{
    public class ApiResponse<T>
{
    public int ErrorCode { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public ApiResponse(int errorCode, string message, T data = default)
    {
        ErrorCode = errorCode;
        Message = message;
        Data = data;
    }
    public static ApiResponse<T> Success( int errorCode,T data, string message = "Success")
    {
        return new ApiResponse<T>(errorCode, message, data);
    }
    public static ApiResponse<T> Failure(int errorCode, string message)
    {
        return new ApiResponse<T>(errorCode, message, default);
    }
}
}