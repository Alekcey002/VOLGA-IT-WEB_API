using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Timetable.Servers
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetValidateAsync(HttpContext httpContext)
        {
            var accessToken = await httpContext.GetTokenAsync("access_token");
            var response = await _httpClient.GetAsync($"http://localhost:8001/api/Authentication/Validate?accessToken={accessToken}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                if (responseObject.TryGetValue("id", out var id) && responseObject.TryGetValue("roles", out var role))
                {
                    var result = new { id, role };
                    return JsonConvert.SerializeObject(result);
                }
            }

            return null;
        }

        public async Task<int?> GetHospitalAsync(int id, string token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:8003/api/");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync($"Hospitals/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var hospitalObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    if (hospitalObject.TryGetValue("id", out var hospitalId) && hospitalId is long)
                    {
                        return (int)(long)hospitalId;
                    }
                }
            }
            return null;
        }

        public async Task<int?> GetDoctorAsync(int id, string token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:8001/api/");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync($"Doctors/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var doctorObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    if (doctorObject.TryGetValue("id", out var doctorId) && doctorId is long)
                    {
                        return (int)(long)doctorId;
                    }
                }
            }
            return null;
        }

        public async Task<string> GetRoomAsync(int id, string token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:8003/api/");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync($"Hospitals/{id}/Rooms");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var roomObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    if (roomObject.TryGetValue("rooms", out var rooms))
                    {
                        return rooms.ToString();
                    }
                }
            }
            return null;
        }
    }
}
