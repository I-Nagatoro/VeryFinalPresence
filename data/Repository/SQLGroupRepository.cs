using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDatabase.DAO;
using Microsoft.EntityFrameworkCore;

namespace data.Repository;

public class SQLGroupRepository : IGroupRepository
{
    private readonly RemoteDatabaseContext _context;

    public SQLGroupRepository(RemoteDatabaseContext context)
    {
        _context = context;
    }
    public List<GroupDAO> GetAllGroups()
    {
        return _context.groups.ToList();
    }

    public void AddGroup(string groupName)
    {
        _context.groups.Add(new GroupDAO
        {
            Id = _context.groups.Select(g=>g.Id).Max() + 1,
            Name = groupName
        });
        _context.SaveChanges();
    }

    public bool UpdateGroup(int groupId, GroupDAO newGroup)
    {
        var GroupDAO = _context.groups
            .Include(g => g.Users)
            .FirstOrDefault(g => g.Id == groupId);
        if (GroupDAO == null) return false;

        GroupDAO.Name = newGroup.Name;
        GroupDAO.Users = newGroup.Users.Select(user => new UserDAO
        {
            UserId = user.UserId,
            FIO = user.FIO,
            GroupId = user.GroupId
        }).ToList();

        _context.SaveChanges();
        return true;
    }

    public GroupDAO GetGroupById(int groupId)
    {
        return _context.groups.FirstOrDefault(g => g.Id == groupId);
    }
    
    public void DeleteGroup(int id)
    {
        var g = _context.groups.Find(id)
                ?? throw new ArgumentException($"Группа с {id} не найдена.");
        _context.groups.Remove(g);
        _context.SaveChanges();
    }
}