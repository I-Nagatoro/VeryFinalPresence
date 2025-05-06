using System.Text;
using data.RemoteData.RemoteDatabase.DAO;
using domain.UseCase;

namespace ui;

public class GroupConsoleUI
{
    private readonly UserUseCase _userUseCase;
    private readonly GroupUseCase _groupUseCase;

    public GroupConsoleUI(UserUseCase userUseCase, GroupUseCase groupUseCase)
    {
        _userUseCase = userUseCase;
        _groupUseCase = groupUseCase;
    }

    public void AddGroup(string groupName)
    {
        _groupUseCase.AddGroup(groupName);
    }

    public void ShowAllGroups()
    {
        Console.WriteLine("\n-=-=-=- Список всех групп -=-=-=-");
        StringBuilder outputGroups = new StringBuilder();
        foreach (var group in _groupUseCase.GetAllGroups())
        {
            outputGroups.AppendLine($"{group.Id}\t{group.Name}");
        }
        Console.WriteLine(outputGroups);
    }

    public bool UpdateGroup(int groupId, string groupName)
    {
        GroupDAO newGroup = new GroupDAO
        {
            Id = groupId,
            Name = groupName
        };
        _groupUseCase.UpdateGroupName(groupId, groupName);
        return true;
    }

    public GroupDAO GetGroupById(int groupId)
    {
        return _groupUseCase.GetGroupById(groupId);
    }
}