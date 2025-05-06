using data.RemoteData.RemoteDatabase.DAO;

namespace data.Repository;

public interface IPresenceRepository
{
    public DateOnly? GetLastDateByGroupId(int groupId);
    public void SavePresence(List<PresenceDAO> presences);
    public List<PresenceDAO> ShowPresenceForDateAndGroup(DateOnly date, int groupId);
    public List<PresenceDAO> GetPresenceByGroup(int groupId);
    public GroupAttendanceStatistics GetGeneralPresenceForGroup(int groupId);
    public void UpdateAbsent(int userId, int groupId, int firstLesson, int lastLesson, DateOnly date, bool b);
    public List<PresenceDAO> GetPresenceByDateGroupAndUser(DateOnly date, int groupId, int userId);
    public void DeleteAllPresence();
    public void DeletePresenceByGroup(int groupId);

}