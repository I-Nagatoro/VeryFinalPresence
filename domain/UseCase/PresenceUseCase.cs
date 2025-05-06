using ClosedXML.Excel;
using data.RemoteData.RemoteDatabase.DAO;
using data.RemoteData.RemoteDataBase.DAO;
using data.Repository;

namespace domain.UseCase;

public class PresenceUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IPresenceRepository _presenceRepository;

    public PresenceUseCase(IUserRepository userRepository, IGroupRepository groupRepository,
        IPresenceRepository presenceRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _presenceRepository = presenceRepository;
    }

    public void GeneratePresenceForDay(int firstLesson, int lastLesson, int groupId)
    {
        var users = _userRepository.GetAllUsers().Where(u=>u.GroupId == groupId).ToList();
        DateOnly startDate = _presenceRepository.GetLastDateByGroupId(groupId)?.AddDays(1)??DateOnly.FromDateTime(DateTime.Now);
        List<PresenceDAO> presences = new List<PresenceDAO>();
        for (int lessonNumber = firstLesson; lessonNumber <= lastLesson; lessonNumber++)
        {
            foreach (var user in users)
            {
                presences.Add(new PresenceDAO
                {
                    UserId = user.UserId,
                    GroupId = user.GroupId,
                    Date = startDate,
                    LessonNumber = lessonNumber,
                    IsAttendance = true
                });
            }
        }
        _presenceRepository.SavePresence(presences);
    }

    public List<PresenceDAO> ShowPresenceForDateAndGroup(DateOnly date, int groupId)
    {
        return _presenceRepository.ShowPresenceForDateAndGroup(date, groupId);
    }

    public bool MarkUserAbsent(DateOnly date, int userId, int groupId, int firstLesson, int lastLesson)
    {
        List<PresenceDAO> presences = _presenceRepository.GetPresenceByDateGroupAndUser(date, groupId,userId);
        if (presences.Where(p => p.UserId == userId).Count() > 0)
        {
            // Обновляем состояние присутствия для указанных занятий
            foreach (var presence in presences.Where(p => p.UserId == userId && p.LessonNumber >= firstLesson && p.LessonNumber <= lastLesson))
            { 
                presence.IsAttendance = false; // Устанавливаем отсутствие
            }
            // Сохраняем изменения в репозитории
            _presenceRepository.UpdateAbsent(userId, groupId, firstLesson, lastLesson, date, false);
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<PresenceDAO> GetAllPresenceByGroup(int groupId)
    {
        return _presenceRepository.GetPresenceByGroup(groupId);
    }

    public GroupAttendanceStatistics GetGeneralPresence(int groupId)
    {
        return _presenceRepository.GetGeneralPresenceForGroup(groupId);
    }
    
    public Dictionary<string, List<AttendanceRecord>> GetAllAttendanceByGroups()
    {
        var attendanceByGroup = new Dictionary<string, List<AttendanceRecord>>();
        var allGroups = _groupRepository.GetAllGroups();

        foreach (var group in allGroups)
        {
            var groupAttendance = _presenceRepository.GetPresenceByGroup(group.Id);
            var attendanceRecords = new List<AttendanceRecord>();

            foreach (var record in groupAttendance)
            {
                var names = _userRepository.GetAllUsers().Where(u => u.UserId == record.UserId);
                foreach (var name in names)
                {
                    attendanceRecords.Add(new AttendanceRecord
                    {
                        UserName = name.FIO,
                        UserId = name.UserId,
                        Date = record.Date,
                        IsAttedance = record.IsAttendance,
                        LessonNumber = record.LessonNumber,
                        GroupName = group.Name
                    });
                }
            }

            attendanceByGroup.Add(group.Name, attendanceRecords);
        }

        return attendanceByGroup;
    }

    public void ExportAttendanceToExcel()
    {
        var attendanceByGroup = GetAllAttendanceByGroups();
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        string reportsFolderPath = Path.Combine(projectDirectory, "Reports");
        string filePath = Path.Combine(reportsFolderPath, "AttendanceReport.xlsx");

        // Создаем папку, если она не существует
        if (!Directory.Exists(reportsFolderPath))
        {
            Directory.CreateDirectory(reportsFolderPath);
        }
        using (var workbook = new XLWorkbook())
        {
            foreach (var group in attendanceByGroup)
            {
                var worksheet = workbook.Worksheets.Add($"{group.Key}");
                worksheet.Cell(1, 1).Value = "ФИО";
                worksheet.Cell(1, 2).Value = "Группа";
                worksheet.Cell(1, 3).Value = "Дата";
                worksheet.Cell(1, 4).Value = "Занятие";
                worksheet.Cell(1, 5).Value = "Статус";

                int row = 2;
                int lesNum = 1;
                foreach (var record in group.Value.OrderBy(r => r.Date).ThenBy(r => r.LessonNumber).ThenBy(r => r.UserId))
                {
                    if (lesNum != record.LessonNumber)
                    {
                        row++;
                    }
                    worksheet.Cell(row, 1).Value = record.UserName;
                    worksheet.Cell(row, 2).Value = record.GroupName;
                    worksheet.Cell(row, 3).Value = record.Date.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 4).Value = record.LessonNumber;
                    worksheet.Cell(row, 5).Value = record.IsAttedance ? "Присутствует" : "Отсутствует";
                    row++;



                    lesNum = record.LessonNumber;
                }

                worksheet.Columns().AdjustToContents();
            }
            workbook.SaveAs(filePath);
        }
    }

    public void DeleteAllPresence()
    {
        _presenceRepository.DeleteAllPresence();
    }

    public void DeletePresenceByGroup(int groupId)
    {
        _presenceRepository.DeletePresenceByGroup(groupId);
    }
    
    public void CreatePresences(IEnumerable<PresenceDAO> presences)
    {
        if (presences == null || !presences.Any())
            throw new ArgumentException("Список посещений пуст.");

        _presenceRepository.SavePresence(presences.ToList());
    }
    
    public void UpdatePresence(DateOnly date, int userId, int groupId, int lessonNumber, bool isAttendance)
    {
        var presences = _presenceRepository
            .GetPresenceByDateGroupAndUser(date, groupId, userId)
            .Where(p => p.LessonNumber == lessonNumber)
            .ToList();

        if (!presences.Any())
            throw new ArgumentException("Посещение не найдено для обновления.");

        foreach (var presence in presences)
            presence.IsAttendance = isAttendance;

        _presenceRepository.UpdateAbsent(userId, groupId, lessonNumber, lessonNumber, date, isAttendance);
    }
}