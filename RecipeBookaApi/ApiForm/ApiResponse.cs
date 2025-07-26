namespace RecipeBookApi.ApiForm
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; } 
        public List<string>? Errors { get; set; } 

         public ApiResponse(T? data, string message = "Operation successful.")
        {
            Success = true;
            Message = message;
            Data = data;
            Errors = null;  
        }

         public ApiResponse(string message = "Operation failed.", List<string>? errors = null)
        {
            Success = false;
            Message = message;
            Data = default(T);  
            Errors = errors ?? new List<string>();  
        }

         public static ApiResponse<T> SuccessResponse(T? data, string message = "Operation successful.")
        {
            return new ApiResponse<T>(data, message);
        }

         public static ApiResponse<T> ErrorResponse(string message = "Operation failed.", List<string>? errors = null)
        {
            return new ApiResponse<T>(message, errors);
        }
    }
}
