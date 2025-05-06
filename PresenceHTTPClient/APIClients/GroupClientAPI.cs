using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PresenceHTTPClient.Interface;
using PresenceHTTPClient.Models;

namespace PresenceHTTPClient.APIClients;

public class GroupApiClient : BaseClientAPI, IGroupClientAPI
{
    private const string BasePath = "Group";
    
    public GroupApiClient(IHttpClientFactory httpClientFactory, ILogger<GroupApiClient> logger) 
        : base(httpClientFactory, logger)
    {
    }

    public async Task<List<GroupDAO>> GetGroupsAsync()
    {
        return await GetAsync<List<GroupDAO>>(BasePath) ?? new List<GroupDAO>();
    }

    public async Task<List<GroupWithStudentDAO>> GetGroupsWithUsersAsync()
    {
        return await GetAsync<List<GroupWithStudentDAO>>("Admin") ?? new List<GroupWithStudentDAO>();
    }
}