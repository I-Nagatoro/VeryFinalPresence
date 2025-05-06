namespace PresenceHTTPClient.Interface
{
    public interface IPresenceClientAPI
    {
        Task<PresenceResponse?> GetPresenceAsync(int groupId, string startDate, string endDate);
        Task<bool> DeletePresenceRecords(string date, int lessonNumder, Guid userGuid);
    }
}