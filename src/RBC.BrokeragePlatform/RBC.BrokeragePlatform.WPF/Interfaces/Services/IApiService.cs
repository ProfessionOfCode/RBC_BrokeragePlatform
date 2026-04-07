namespace RBC.BrokeragePlatform.WPF.Interfaces.Services;

public interface IApiService : IDisposable
{
    /// <summary>
    /// Executes an HTTP GET request against the specified endpoint and returns the response content.
    /// </summary>
    Task<string> GetAsync(string endpoint);

    /// <summary>
    /// Executes an HTTP POST request against the specified endpoint with a JSON payload.
    /// </summary>
    Task<bool> PostAsync(string endpoint, object payload);
}
