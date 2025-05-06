using data.RemoteData.RemoteDatabase.DAO;
using data.Repository;

namespace domain.UseCase;

public class UserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public UserUseCase(IUserRepository userRepository, IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }
    public List<UserDAO> GetAllUsers()
    {
        return _userRepository.GetAllUsers().Join(
            _groupRepository.GetAllGroups(),
            user=>user.GroupId,
            group => group.Id,
            (user, group) => new UserDAO
            {
                UserId = user.UserId,
                FIO = user.FIO,
                GroupId = user.GroupId,
            }).ToList();
    }

    public bool RemoveUserByUserId(int UserId)
    {
        try
        {
            return _userRepository.RemoveUserByUserId(UserId);
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public UserDAO FindUserByUserId(int UserId)
    {
        return _userRepository.FindUserByUserId(UserId);
    }

    public bool UpdateUserByUserId(int UserId, string UserName, int GroupId)
    {
        UserDAO user = new UserDAO
        {
            UserId = UserId,
            FIO = UserName,
            GroupId = GroupId
        };
        return _userRepository.UpdateUserByUserId(user);
    }
}