using Microsoft.AspNetCore.Http.HttpResults;

namespace LabZakazivanjeAPI.Services;

public class ServiceResult<T> where T : class
{ 
    public bool Success {get;}
    public T? Data {get;}
    public string? ErrorMessage {get;}

    private ServiceResult(bool success, T? data, string? errorMessage)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public static ServiceResult<T> Ok(T data) => new(true, data, null);
    public static ServiceResult<T> Error(string error) => new(false, null, error);
}