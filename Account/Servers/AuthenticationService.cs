using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace Account.Servers
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetUsernameAsync(HttpContext httpContext)
        {
            var accessToken = await httpContext.GetTokenAsync("access_token");
            var response = await _httpClient.GetAsync($"http://localhost:8001/api/Authentication/Validate?accessToken={accessToken}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(content);
                return responseObject.username;
            }

            throw new Exception("Не удалось проверить токен доступа.");
        }

        public async Task<string> GetRoleAsync(HttpContext httpContext)
        {
            var accessToken = await httpContext.GetTokenAsync("access_token");
            var response = await _httpClient.GetAsync($"http://localhost:8001/api/Authentication/Validate?accessToken={accessToken}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                if (responseObject.TryGetValue("roles", out var roles))
                {
                    return roles.ToString();
                }
            }

            throw new Exception("Не удалось проверить токен доступа.");
        }
    }
}
