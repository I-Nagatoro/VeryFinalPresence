using PresenceHTTPClient.Models;

namespace PresenceHTTPClient.Interface
{
    public interface IGroupClientAPI
    {
        Task<List<GroupDAO>> GetGroupsAsync();
        Task<List<GroupWithStudentDAO>> GetGroupsWithUsersAsync();
    }
}