using System.Net.Http.Json;
using PresenceHTTPClient.Models;
using Microsoft.Extensions.Logging;
using PresenceHTTPClient.APIClients;
using PresenceHTTPClient.Interface;

namespace presence_client.ApiClients;

public class PresenceApiClient : BaseClientAPI, IPresenceClientAPI
{
    private const string BasePath = "api/Presence";

    public PresenceApiClient(IHttpClientFactory httpClientFactory, ILogger<PresenceApiClient> logger) 
        : base(httpClientFactory, logger)
    {
    }

    public async Task<PresenceResponse?> GetPresenceAsync(int groupId, string startDate, string endDate)
    {
        try
        {
            var url = $"{BasePath}?groupID={groupId}&start={startDate}&end={endDate}";
            _logger.LogInformation($"Отправка запроса на: {url}");
    
            return await _httpClient.GetFromJsonAsync<PresenceResponse>(url);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении посещаемости");
            throw;
        }
    }

    public async Task<bool> DeletePresenceRecords(string date, int lessonNumder, Guid userGuid)
    {
        return await DeleteAsync($"{BasePath}/records/?date={date}&lessonNumber={lessonNumder}&userGuid={userGuid}");
    }
}