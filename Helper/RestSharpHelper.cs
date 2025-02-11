using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public class RestSharpHelper
{
    private readonly IConfiguration _configuration;
    private readonly RestClient _client;
    private const string _scope = "https://api.businesscentral.dynamics.com/.default";
    private const string _grantType = "client_credentials";

    public RestSharpHelper(IConfiguration configuration)
    {
        _configuration = configuration;
        _client = new RestClient();
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var tokenEndpoint = $"https://login.microsoftonline.com/{_configuration["AzureAD:TenantId"]}/oauth2/v2.0/token";

        var request = new RestRequest(tokenEndpoint, Method.Post);
        request.AddParameter("client_id", _configuration["AzureAD:ClientId"]);
        request.AddParameter("client_secret", _configuration["AzureAD:ClientSecret"]);
        request.AddParameter("scope", _scope);
        request.AddParameter("grant_type", _grantType);

        try
        {
            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to retrieve access token: {response.StatusCode} - {response.ErrorMessage}");
            }

            var tokenResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content ?? "{}");

            if (tokenResult == null || !tokenResult.ContainsKey("access_token"))
            {
                throw new Exception("Access token missing in response.");
            }

            return tokenResult["access_token"];
        }
        catch (Exception ex)
        {
            throw new Exception($"Exception during token retrieval: {ex.Message}", ex);
        }
    }

    public async Task<List<dynamic>> GetDataAsync(string urlBase, int pageSize)
    {
        var accessToken = await GetAccessTokenAsync();
        var client = new RestClient(urlBase);
        client.AddDefaultHeader("Authorization", $"Bearer {accessToken}");
        client.AddDefaultHeader("Accept", "application/json");

        // Start measuring time
        var stopwatch = Stopwatch.StartNew();

        // Get total count of records
        var countUrl = "?$count=true&$top=0";
        var countRequest = new RestRequest(countUrl, Method.Get);

        var countResponse = await client.ExecuteAsync(countRequest);
        if (!countResponse.IsSuccessful)
        {
            throw new Exception($"Failed to Get record count: {countResponse.StatusCode} - {countResponse.ErrorMessage}");
        }

        var countResult = JsonConvert.DeserializeObject<CountResponse>(countResponse.Content ?? "{}");
        int totalRecordsToGet = countResult?.ODataCount ?? 0;
        int totalPages = (int)Math.Ceiling((double)totalRecordsToGet / pageSize);

        Console.WriteLine($"Total records to Get: {totalRecordsToGet}");

        var allData = new List<dynamic>();

        var tasks = new List<Task<RestResponse>>();

        for (int page = 0; page < totalPages; page++)
        {
            var skip = page * pageSize;
            var pagedUrl = $"?$skip={skip}&$top={pageSize}";
            var request = new RestRequest(pagedUrl, Method.Get);

            tasks.Add(client.ExecuteAsync(request));
        }

        // Execute all requests concurrently
        var responses = await Task.WhenAll(tasks);

        // Process responses
        foreach (var response in responses)
        {
            if (!response.IsSuccessful)
            {
                throw new Exception($"Error Get data: {response.StatusCode} - {response.ErrorMessage}");
            }

            var pageData = JsonConvert.DeserializeObject<dynamic>(response.Content ?? "{}");
            if (pageData?.value != null)
            {
                allData.AddRange(pageData.value);
            }
        }

        // Stop the stopwatch and print elapsed time
        stopwatch.Stop();
        Console.WriteLine($"Total Get Data Completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds");

        return allData;
    }

    private class CountResponse
    {
        [JsonProperty("@odata.count")]
        public int ODataCount { get; set; }
    }
}