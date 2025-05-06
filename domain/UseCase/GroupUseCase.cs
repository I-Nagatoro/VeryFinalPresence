using System.Text.RegularExpressions;
using data.RemoteData.RemoteDatabase.DAO;
using data.Repository;

namespace domain.UseCase;

public class GroupUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public GroupUseCase(IUserRepository userRepository, IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }
    private GroupDAO ValidateGroupExistence(int groupId)
    {
        var existingGroup = _groupRepository.GetAllGroups()
            .FirstOrDefault(g => g.Id == groupId);

        if (existingGroup == null)
        {
            throw new ArgumentException("Группа не найдена.");
        }

        return existingGroup;
    }
    public List<GroupDAO> GetAllGroups()
    {
        return _groupRepository.GetAllGroups();
    }

    public void AddGroup(string groupName)
    {
        _groupRepository.AddGroup(groupName);
    }

    private void ValidateGroupName(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            throw new ArgumentException("Имя группы не может быть пустым.");
        }
    }
    public void UpdateGroupName(int groupId, string newGroupName)
    {
        ValidateGroupName(newGroupName);
        var existingGroup = ValidateGroupExistence(groupId);

        existingGroup.Name = newGroupName;
        _groupRepository.UpdateGroup(groupId, existingGroup);
    }

    public GroupDAO GetGroupById(int groupId)
    {
        return _groupRepository.GetGroupById(groupId);
    }
    
    public void DeleteGroup(int groupId)
    {
        ValidateGroupExistence(groupId);
        _groupRepository.DeleteGroup(groupId);
    }
}