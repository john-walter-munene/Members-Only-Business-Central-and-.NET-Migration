// using System.Net.Http.Json;
// using Microsoft.Identity.Client;

// namespace MembersOnly.Web.Services;

// public class BusinessCentralClient : IBusinessCentralClient
// {
//     private readonly IHttpClientFactory _httpClientFactory;
//     private readonly IConfiguration _configuration;

//     public BusinessCentralClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
//     {
//         _httpClientFactory = httpClientFactory;
//         _configuration = configuration;
//     }

//     public async Task<string> RegisterMemberAsync(string identityUserId, string email, string firstName, string lastName)
//     {
//         // Generate a Member No. for now. Step 4 can move this to a No. Series in AL.
//         string memberNo = "M" + Guid.NewGuid().ToString("N")[..11].ToUpperInvariant();

//         // Build the request body. AL sets the status and dates, not this app.
//         var payload = new
//         {
//             memberNo,
//             identityUserId,
//             firstName,
//             lastName,
//             email
//         };

//         var token = await GetAccessTokenAsync();
//         var client = _httpClientFactory.CreateClient("BusinessCentral");

//         // Custom API URL: publisher/group/version, then the company's collection.
//         var companyId = _configuration["BusinessCentral:CompanyId"];
//         var url = $"{BaseApiUrl()}/companies({companyId})/clubMembers";

//         using var request = new HttpRequestMessage(HttpMethod.Post, url)
//         {
//             Content = JsonContent.Create(payload)
//         };
//         request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

//         var response = await client.SendAsync(request);

//         // Throw on failure so the controller can roll back the Identity user.
//         if (!response.IsSuccessStatusCode)
//         {
//             var body = await response.Content.ReadAsStringAsync();
//             throw new InvalidOperationException($"Business Central rejected the member: {response.StatusCode} {body}");
//         }

//         return memberNo;
//     }

//     private string BaseApiUrl()
//     {
//         // Build the Business Central custom API root for this environment.
//         var baseUrl = _configuration["BusinessCentral:BaseUrl"];
//         var tenantId = _configuration["BusinessCentral:TenantId"];
//         var environment = _configuration["BusinessCentral:EnvironmentName"];
//         return $"{baseUrl}/{tenantId}/{environment}/api/membersonly/club/v1.0";
//     }

//     private async Task<string> GetAccessTokenAsync()
//     {
//         // Build an app-only token with the client secret from user secrets.
//         var app = ConfidentialClientApplicationBuilder
//             .Create(_configuration["BusinessCentral:ClientId"])
//             .WithClientSecret(_configuration["BusinessCentral:ClientSecret"])
//             .WithAuthority($"https://login.microsoftonline.com/{_configuration["BusinessCentral:TenantId"]}")
//             .Build();

//         var result = await app.AcquireTokenForClient(new[] { "https://api.businesscentral.dynamics.com/.default" })
//             .ExecuteAsync();

//         return result.AccessToken;
//     }
// }


using System.Net.Http.Json;

namespace MembersOnly.Web.Services;

public class BusinessCentralClient : IBusinessCentralClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public BusinessCentralClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<string> RegisterMemberAsync(
        string identityUserId,
        string email,
        string firstName,
        string lastName)
    {
        // Generate a Member No. for now.
        string memberNo = "M" +
            Guid.NewGuid().ToString("N")[..11].ToUpperInvariant();

        // AL sets membership status and registration dates.
        var payload = new
        {
            memberNo,
            identityUserId,
            firstName,
            lastName,
            email
        };

        var client = _httpClientFactory.CreateClient("BusinessCentral");

        var companyId = _configuration["BusinessCentral:CompanyId"];

        if (string.IsNullOrWhiteSpace(companyId))
        {
            throw new InvalidOperationException(
                "Business Central CompanyId is not configured.");
        }

        var url =
            $"{BaseApiUrl()}/companies({companyId})/clubMembers";

        using var response = await client.PostAsJsonAsync(url, payload);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                $"Business Central rejected the member: {response.StatusCode} {body}");
        }

        return memberNo;
    }

    private string BaseApiUrl()
    {
        var baseUrl = _configuration["BusinessCentral:BaseUrl"]?.TrimEnd('/');
        var publisher = _configuration["BusinessCentral:Publisher"];
        var group = _configuration["BusinessCentral:Group"];
        var version = _configuration["BusinessCentral:ApiVersion"];

        if (string.IsNullOrWhiteSpace(baseUrl) ||
            string.IsNullOrWhiteSpace(publisher) ||
            string.IsNullOrWhiteSpace(group) ||
            string.IsNullOrWhiteSpace(version))
        {
            throw new InvalidOperationException(
                "Business Central API configuration is incomplete.");
        }

        return $"{baseUrl}/api/{publisher}/{group}/{version}";
    }
}