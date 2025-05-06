using data.RemoteData.RemoteDatabase.DAO;

namespace data.Repository;

public interface IGroupRepository
{
    public List<GroupDAO> GetAllGroups();
    public void AddGroup(string groupName);
    public bool UpdateGroup(int groupId, GroupDAO newGroup);
    public GroupDAO GetGroupById(int groupId);
    public void DeleteGroup(int groupId);
}