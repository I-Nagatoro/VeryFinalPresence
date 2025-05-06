using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PresenceHTTPClient.ApiClients.Interface;

public interface IUserClientAPI
{
    Task<bool> DeleteUserAsync(Guid userGuid);
    Task<bool> UpdateUserFioAsync(Guid userGuid, string fio);
    Task<bool> DeleteUsersByGroupIdAsync(int groupId);
    Task<bool> CreateUser(string fio, string groupName);
}