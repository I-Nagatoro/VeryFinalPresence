using data.RemoteData.RemoteDatabase.DAO;

namespace data.Repository;

public interface IUserRepository
{
    public List<UserDAO> GetAllUsers();
    public bool RemoveUserByUserId(int userId);
    public UserDAO FindUserByUserId(int userId);
    public bool UpdateUserByUserId(UserDAO user);
}