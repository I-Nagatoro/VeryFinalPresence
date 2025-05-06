using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDatabase.DAO;
using Microsoft.EntityFrameworkCore;

namespace data.Repository;

public class SQLUserRepository : IUserRepository
{
    private readonly RemoteDatabaseContext _context;

    public SQLUserRepository(RemoteDatabaseContext context)
    {
        _context = context;
    }
    public List<UserDAO> GetAllUsers()
    {
        return _context.users
            .OrderBy(u => u.UserId)
            .AsNoTracking()
            .ToList();
    }

    public bool RemoveUserByUserId(int userId)
    {
        try
        {
            _context.users.Remove(_context.users.Find(userId));
            _context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public UserDAO FindUserByUserId(int UserId)
    {
        return _context.users.Find(UserId);
    }

    public bool UpdateUserByUserId(UserDAO user)
    {
        var groupExists = _context.groups.Any(x => x.Id == user.GroupId);
        if (!groupExists)
        {
            Console.WriteLine("Группа не найдена");
            return false;
        }
        var userExists = _context.users.FirstOrDefault(u=>u.UserId==user.UserId);
        if (userExists==null)
        {
            Console.WriteLine("Студент не найден");
            return false;
        }
        userExists.FIO = user.FIO;
        userExists.GroupId = user.GroupId;
        _context.SaveChanges();
        return true;
    }
}