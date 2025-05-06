using data.RemoteData.RemoteDatabase.DAO;
using domain.UseCase;

namespace ui;

public class PresenceConsoleUI
{
    private readonly PresenceUseCase _presenceUseCase;

    public PresenceConsoleUI(PresenceUseCase presenceUseCase)
    {
        _presenceUseCase = presenceUseCase;
    }

    public void GeneratePresenceForDay(int firstLesson, int lastLesson, int groupId)
    {
        try
        {
            _presenceUseCase.GeneratePresenceForDay(firstLesson, lastLesson, groupId);
            Console.WriteLine("Посещаемость на день успешно сгенерирована.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при генерации посещаемости: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public void GeneratePresenceForWeek(int firstLesson, int lastLesson, int groupId)
    {
        for (int i = 0; i < 7; i++)
        {
            GeneratePresenceForDay(firstLesson, lastLesson, groupId);
        }
    }

    public List<PresenceDAO> ShowPresenceForDateAndGroup(DateOnly date, int groupId)
    {
        return _presenceUseCase.ShowPresenceForDateAndGroup(date, groupId);
    }

    public void MarkUserAbsent(DateTime date, int groupId, int userId, int firstLesson, int lastLesson)
    {
        bool check = _presenceUseCase.MarkUserAbsent(DateOnly.FromDateTime(date), userId, groupId, firstLesson, lastLesson);
        if (check)
        {
            Console.WriteLine("Пользователь отмечен как осутсвующий");
        }
        else
        {
            Console.WriteLine($"Посещаемость для пользователя ID: {userId} на дату {date.ToShortDateString()}" +
                              $" с {firstLesson} по {lastLesson} уроки не найдена.");
        }
    }

        public void DisplayAllPresenceByGroup(int groupId)
        {
            try
            {
                var presences = _presenceUseCase.GetAllPresenceByGroup(groupId);

                if (presences == null || presences.Count == 0)
                {
                    Console.WriteLine($"Посещаемость для группы с ID {groupId} отсутствует.");
                    return;
                }

                var groupedPresences = presences.GroupBy(p => p.Date);

                foreach (var group in groupedPresences)
                {
                    Console.WriteLine("===================================================");
                    Console.WriteLine($"Дата: {group.Key.ToString("dd.MM.yyyy")}");
                    Console.WriteLine("===================================================");

                    var groupedByLesson = group.GroupBy(p => p.LessonNumber);

                    foreach (var lessonGroup in groupedByLesson)
                    {
                        Console.WriteLine($"Занятие {lessonGroup.Key}:");

                        var userIds = new HashSet<int>();

                        foreach (var presence in lessonGroup)
                        {
                            if (userIds.Add(presence.UserId))
                            {
                                string status = presence.IsAttendance ? "Присутствует" : "Отсутствует";
                                Console.WriteLine($"Пользователь ID: {presence.UserId}, Статус: {status}");
                            }
                        }

                        Console.WriteLine("---------------------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выводе посещаемости: {ex.Message}");
            }
        }

        public void DisplayGeneralPresence(int groupId)
        {
            var statistics = _presenceUseCase.GetGeneralPresence(groupId);
            Console.WriteLine($"Человек в группе: {statistics.UserCount}, " +
                              $"Количество проведённых занятий: {statistics.TotalLessons}, " +
                              $"Общий процент посещаемости группы: {statistics.AttendancePercentage}%");

            foreach (var user in statistics.UserAttendanceDetails)
            {
                Console.ForegroundColor = user.AttendanceRate < 40 ? ConsoleColor.Red : ConsoleColor.White;
                Console.WriteLine($"ID Пользователя: {user.UserId}, Посетил: {user.Attended}, " +
                                  $"Пропустил: {user.Missed}, Процент посещаемости: {user.AttendanceRate}%");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ExportAttendanceToExcel()
        {
            try
            {
                _presenceUseCase.ExportAttendanceToExcel();
                Console.WriteLine("Данные посещаемости успешно экспортированы в Excel.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при экспорте посещаемости: {ex.Message}");
            }
        }
}