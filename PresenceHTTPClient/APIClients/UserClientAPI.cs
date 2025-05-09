﻿using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PresenceHTTPClient.ApiClients.Interface;
using PresenceHTTPClient.Models;

namespace PresenceHTTPClient.APIClients;

public class UserApiClient : BaseClientAPI, IUserClientAPI
{
    private const string BasePath = "api/Admin";

    public UserApiClient(IHttpClientFactory httpClientFactory, ILogger<GroupApiClient> logger) 
        : base(httpClientFactory, logger)
    {
    }

    public async Task<bool> DeleteUserAsync(Guid userGuid)
    {
        try
        {
            var responce = await _httpClient.DeleteAsync($"{BasePath}/user?userGuid={userGuid}");
            return responce.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка, {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateUserFioAsync(Guid userGuid, string fio)
    {
        try
        {
            var request = new FIOUpdate { Fio = fio };
        
            var response = await _httpClient.PatchAsJsonAsync(
                $"{BasePath}/updatefio/user/{userGuid}", 
                request, 
                _jsonOptions);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось обновить ФИО");
            return false;
        }
    }
    
    public async Task<bool> DeleteUsersByGroupIdAsync(int groupId)
    {
        try
        {
            var responce = await _httpClient.DeleteAsync($"{BasePath}/usersbygroupid/{groupId}");
            return responce.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при удалении пользователей из группы {groupId}");
            return false;
        }
    }

    public async Task<bool> CreateUser(string fio, string groupName)
    {
        try
        {
            var request = new CreateUserRequest { Fio = fio, GroupName = groupName };
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/usercreate", request, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка создания пользователей");
            return false;
        }
    }
}