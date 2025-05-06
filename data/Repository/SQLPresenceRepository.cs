using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDatabase.DAO;
using Microsoft.EntityFrameworkCore.Storage;

namespace data.Repository;

public class SQLPresenceRepository : IPresenceRepository
{
    private readonly RemoteDatabaseContext _context;

    public SQLPresenceRepository(RemoteDatabaseContext context)
    {
        _context = context;
    }

    public DateOnly? GetLastDateByGroupId(int groupId)
    {
        var lastDate = _context.presences
            .Where(p => p.GroupId == groupId)
            .OrderByDescending(p => p.Date)
            .Select(p => p.Date)
            .FirstOrDefault();

        return lastDate == default ? (DateOnly?)null : lastDate;
    }

    public void SavePresence(List<PresenceDAO> presences)
    {
        foreach (var presence in presences)
        {
            _context.presences.Add(presence);
        }
        _context.SaveChanges();
    }

    public List<PresenceDAO> ShowPresenceForDateAndGroup(DateOnly date, int groupId)
    {
        return _context.presences.Where(p => p.GroupId == groupId && p.Date == date)
            .Join(_context.users,
                presence=>presence.UserId,
                user=>user.UserId,
                (presence, user) => new PresenceDAO
                {
                    PresenceId = presence.PresenceId,
                    UserId = user.UserId,
                    GroupId = presence.GroupId,
                    Date = presence.Date,
                    LessonNumber = presence.LessonNumber,
                    IsAttendance = presence.IsAttendance
                }).ToList();
    }

    public List<PresenceDAO> GetForAbsent(DateOnly date, int groupId)
    {
        return _context.presences.Where(p => p.GroupId == groupId && p.Date == date).ToList();
    }
    
    public List<PresenceDAO> GetPresenceByGroup(int groupId)
    {
        return _context.presences.Where(p => p.GroupId == groupId)
            .OrderBy(p => p.Date)
            .ThenBy(p => p.UserId).ToList();
    }
    
    public GroupAttendanceStatistics GetGeneralPresenceForGroup(int groupId)
        {
            var presences = _context.presences.Where(p => p.GroupId == groupId)
                .OrderBy(p => p.LessonNumber).ToList();
            var dates = _context.presences;
            var distDates = dates.Select(p => p.Date).Distinct().ToList();
            int lesId = 0;
            int lesNum = 1;
            double att = 0;
            int days = -1;
            int countAllLes = 0;
            DateOnly date = DateOnly.MinValue;
            List<int> usersId = new List<int>();

            foreach (var presence in presences)
            {
                if (!usersId.Contains(presence.UserId))
                {
                    usersId.Add(presence.UserId);
                }

                if (presence.Date != date)
                {
                    date = presence.Date;
                    lesId++;
                    lesNum = presence.LessonNumber;
                    days++;
                }

                if (presence.LessonNumber != lesNum && date == presence.Date)
                {
                    lesNum = presence.LessonNumber;
                    countAllLes++;
                    lesId++;
                }


                if (presence.IsAttendance)
                {
                    att++;
                }

            }

            List<UserAttendance> a = new List<UserAttendance>();
            List<int> ids = new List<int>();
            double ok = 0;
            double skip = 0;
            int userId = 0;
            foreach (var user in usersId)
            {
                var users = _context.presences.Where(p => p.UserId == user);
                foreach (var usera in users)
                {
                    userId = usera.UserId;
                    if (!ids.Contains(usera.UserId))
                    {
                        skip = 0;
                        ok = 0;
                        ids.Add(userId);
                        a.Add(new UserAttendance { UserId = userId, Attended = ok, Missed = skip });
                        userId = usera.UserId;
                        if (usera.IsAttendance)
                        {
                            a.First(a => a.UserId == usera.UserId).Attended = ok += 1;
                        }
                        else
                        {
                            a.First(a => a.UserId == usera.UserId).Missed = skip += 1;
                        }
                    }
                    else
                    {
                        if (usera.IsAttendance)
                        {
                            a.First(a => a.UserId == usera.UserId).Attended = ok += 1;
                        }
                        else
                        {
                            a.First(a => a.UserId == usera.UserId).Missed = skip += 1;
                        }
                    }
                }
            }

            var statistics = new GroupAttendanceStatistics
            {
                UserCount = usersId.Count,
                TotalLessons = lesId,
                AttendancePercentage = att / usersId.Count / lesNum / distDates.Count() * 100
            };

            foreach (var user in a)
            {
                statistics.UserAttendanceDetails.Add(new UserAttendance
                {
                    UserId = user.UserId,
                    Attended = user.Attended,
                    Missed = user.Missed,
                    AttendanceRate = user.Attended / (user.Missed + user.Attended) * 100
                });
            }

            return statistics;
        }

    public void UpdateAbsent(int userId, int groupId, int firstLesson, int lastLesson, DateOnly date,
        bool isAttendance)
    {
        // Находим все записи по UserId, GroupId, LessonNumber (в диапазоне) и дате
        var presences = _context.presences
            .Where(p => p.UserId == userId
                        && p.GroupId == groupId
                        && p.LessonNumber >= firstLesson
                        && p.LessonNumber <= lastLesson
                        && p.Date == date)
            .ToList();

        // Обновляем значение IsAttendance для всех найденных записей
        foreach (var presence in presences)
        {
            presence.IsAttendance = isAttendance;
        }

        _context.SaveChanges(); // Сохраняем изменения в базе данных
    }
    
    public List<PresenceDAO> GetPresenceByDateGroupAndUser(DateOnly date, int groupId, int userId)
    {
        var presences = _context.presences.Where(p => p.Date == date && p.GroupId == groupId && p.UserId == userId).ToList();
        return presences;
    }
    
    public void DeleteAllPresence()
    {
        _context.presences.RemoveRange(_context.presences);
        _context.SaveChanges();
    }

    public void DeletePresenceByGroup(int groupId)
    {
        var toRemove = _context.presences
            .Where(p => p.GroupId == groupId);
        _context.presences.RemoveRange(toRemove);
        _context.SaveChanges();
    }
}