using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ClaimSubmission.Web.Models;

namespace ClaimSubmission.Web.Services
{
    
    public interface IClaimApiService
    {
        Task<List<ClaimViewListModel>> GetAllClaimsAsync(string token, int pageNumber = 1, int pageSize = 10);
        Task<ClaimViewListModel?> GetClaimByIdAsync(string token, int claimId);
        Task<int> CreateClaimAsync(string token, AddClaimViewModel claim);
        Task UpdateClaimAsync(string token, EditClaimViewModel claim);
        Task DeleteClaimAsync(string token, int claimId);
    }

    public class ClaimApiService : IClaimApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ClaimApiService(HttpClient httpClient, string apiBaseUrl)
        {
            _httpClient = httpClient;
            _apiBaseUrl = apiBaseUrl;
        }

        /// <summary>
        /// Get all claims from API
        /// </summary>
        public async Task<List<ClaimViewListModel>> GetAllClaimsAsync(string token, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string url = $"{_apiBaseUrl}/api/claims?pageNumber={pageNumber}&pageSize={pageSize}";

                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            if (string.IsNullOrWhiteSpace(responseContent))
                            {
                                return new List<ClaimViewListModel>();
                            }

                            try
                            {
                                using (JsonDocument doc = JsonDocument.Parse(responseContent))
                                {
                                    JsonElement root = doc.RootElement;
                                    if (root.TryGetProperty("data", out JsonElement dataElement) &&
                                        dataElement.TryGetProperty("items", out JsonElement itemsElement))
                                    {
                                        var claims = JsonSerializer.Deserialize<List<ClaimViewListModel>>(
                                            itemsElement.GetRawText(),
                                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                        return claims ?? new List<ClaimViewListModel>();
                                    }
                                }
                            }
                            catch (JsonException ex)
                            {
                                throw new Exception($"Failed to parse API response: {ex.Message}", ex);
                            }
                        }
                        else
                        {
                            throw new Exception($"API Error: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving claims: {ex.Message}", ex);
            }

            return new List<ClaimViewListModel>();
        }

        /// <summary>
        /// Get specific claim by ID from API
        /// </summary>
        public async Task<ClaimViewListModel?> GetClaimByIdAsync(string token, int claimId)
        {
            try
            {
                string url = $"{_apiBaseUrl}/api/claims/{claimId}";

                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            if (string.IsNullOrWhiteSpace(responseContent))
                            {
                                return null;
                            }

                            try
                            {
                                using (JsonDocument doc = JsonDocument.Parse(responseContent))
                                {
                                    JsonElement root = doc.RootElement;
                                    if (root.TryGetProperty("data", out JsonElement dataElement))
                                    {
                                        return JsonSerializer.Deserialize<ClaimViewListModel>(
                                            dataElement.GetRawText(),
                                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                            }
                            catch (JsonException ex)
                            {
                                throw new Exception($"Failed to parse API response: {ex.Message}", ex);
                            }
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return null;
                        }
                        else
                        {
                            throw new Exception($"API Error: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving claim: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Create a new claim via API
        /// </summary>
        public async Task<int> CreateClaimAsync(string token, AddClaimViewModel claim)
        {
            try
            {
                string url = $"{_apiBaseUrl}/api/claims";

                var jsonContent = JsonSerializer.Serialize(claim);
                var content = new StringContent(
                    jsonContent,
                    System.Text.Encoding.UTF8,
                    "application/json");

                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Content = content;
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            if (string.IsNullOrWhiteSpace(responseContent))
                            {
                                throw new Exception("Empty response from API");
                            }

                            try
                            {
                                using (JsonDocument doc = JsonDocument.Parse(responseContent))
                                {
                                    JsonElement root = doc.RootElement;
                                    if (root.TryGetProperty("data", out JsonElement dataElement) &&
                                        dataElement.TryGetProperty("claimId", out JsonElement claimIdElement))
                                    {
                                        return claimIdElement.GetInt32();
                                    }
                                }
                            }
                            catch (JsonException ex)
                            {
                                throw new Exception($"Failed to parse API response: {ex.Message}", ex);
                            }
                        }
                        else
                        {
                            throw new Exception($"API Error: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating claim: {ex.Message}", ex);
            }

            return -1;
        }
            public async Task UpdateClaimAsync(string token, EditClaimViewModel claim)
        {
            try
            {
                if (claim == null)
                {
                    throw new ArgumentNullException(nameof(claim));
                }

                string url = $"{_apiBaseUrl}/api/claims/{claim.ClaimId}";

                var jsonContent = JsonSerializer.Serialize(claim);
                var content = new StringContent(
                    jsonContent,
                    System.Text.Encoding.UTF8,
                    "application/json");

                using (var request = new HttpRequestMessage(HttpMethod.Put, url))
                {
                    request.Content = content;
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"API Error: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating claim: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete a claim via API
        /// </summary>
        public async Task DeleteClaimAsync(string token, int claimId)
        {
            try
            {
                string url = $"{_apiBaseUrl}/api/claims/{claimId}";

                using (var request = new HttpRequestMessage(HttpMethod.Delete, url))
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"API Error: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting claim: {ex.Message}", ex);
            }
        }
    }
}
