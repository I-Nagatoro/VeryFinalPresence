using System;
using System.Globalization;
using data.RemoteData.RemoteDatabase.DAO;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using domain.UseCase;
using ui;

namespace ui
{
    public class MainMenuUI
    {
        private readonly UserConsoleUI _userConsoleUI;
        private readonly GroupConsoleUI _groupConsoleUI;
        private readonly PresenceConsoleUI _presenceConsoleUI;

        public MainMenuUI(UserUseCase userUseCase, GroupUseCase groupUseCase, PresenceUseCase presenceUseCase)
        {
            _userConsoleUI = new UserConsoleUI(userUseCase, groupUseCase);
            _groupConsoleUI = new GroupConsoleUI(userUseCase, groupUseCase);
            _presenceConsoleUI = new PresenceConsoleUI(presenceUseCase);
        }

        public void Start()
        {
            while (true)
            {
                ShowNavigation();
                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        _userConsoleUI.ShowAllUsers();
                        break;

                    case "2":
                        HandleMemberRemoval();
                        break;

                    case "3":
                        HandleMemberUpdate();
                        break;

                    case "4":
                        SearchMember();
                        break;

                    case "5":
                        _groupConsoleUI.ShowAllGroups();
                        break;

                    case "6":
                        CreateNewGroup();
                        break;

                    case "7":
                        ModifyGroupName();
                        break;

                    case "8":
                        FindGroup();
                        break;

                    case "9":
                        CreateDailyAttendance();
                        break;

                    case "10":
                        CreateWeeklyAttendance();
                        break;

                    case "11":
                        ViewAttendanceRecords();
                        break;

                    case "12":
                        RecordAbsence();
                        break;

                    case "13":
                        ShowGroupAttendanceHistory();
                        break;

                    case "14":
                        DisplayAttendanceSummary();
                        break;

                    case "15":
                        GenerateExcelReport();
                        break;

                    case "0":
                        Console.WriteLine("Завершение работы...");
                        return;

                    default:
                        Console.WriteLine("Некорректный ввод, повторите попытку.");
                        break;
                }
                Console.WriteLine();
            }
        }

        private void ShowNavigation()
        {
            Console.WriteLine("\n=== Управление системой ===\n");
            
            Console.WriteLine("Управление участниками:");
            Console.WriteLine("1. Список всех участников");
            Console.WriteLine("2. Удалить участника");
            Console.WriteLine("3. Обновить данные участника");
            Console.WriteLine("4. Найти участника\n");

            Console.WriteLine("Управление группами:");
            Console.WriteLine("5. Показать все группы");
            Console.WriteLine("6. Создать новую группу");
            Console.WriteLine("7. Переименовать группу");
            Console.WriteLine("8. Найти группу\n");

            Console.WriteLine("Управление посещаемостью:");
            Console.WriteLine("9. Создать записи за день");
            Console.WriteLine("10. Создать записи за неделю");
            Console.WriteLine("11. Просмотр посещаемости");
            Console.WriteLine("12. Зарегистрировать отсутствие");
            Console.WriteLine("13. История посещаемости группы");
            Console.WriteLine("14. Статистика посещаемости");
            Console.WriteLine("15. Экспорт в Excel\n");

            Console.Write("Выберите действие: ");
        }

        private int GetValidNumberInput(string prompt)
        {
            int result;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.WriteLine("Ошибка формата ввода. Повторите попытку.");
                Console.Write(prompt);
            }
            return result;
        }

        private DateTime GetValidDateInput(string prompt)
        {
            DateTime date;
            Console.Write(prompt);
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Console.WriteLine("Некорректный формат даты. Используйте дд.мм.гггг");
                Console.Write(prompt);
            }
            return date;
        }

        private void HandleMemberRemoval()
        {
            int userId = GetValidNumberInput("Введите id студента: ");
            _userConsoleUI.RemoveUserByUserId(userId);
        }

        private void HandleMemberUpdate()
        {
            int userId = GetValidNumberInput("Введите идентификатор участника: ");
            _userConsoleUI.UpdateUserByUserId(userId);
        }

        private void SearchMember()
        {
            int userId = GetValidNumberInput("Введите идентификатор участника: ");
            _userConsoleUI.FindUserByUserId(userId);
        }

        private void CreateNewGroup()
        {
            Console.Write("Введите название группы: ");
            var groupName = Console.ReadLine();
            _groupConsoleUI.AddGroup(groupName);
            Console.WriteLine($"\nГруппа {groupName} успешно добавлена");
        }

        private void ModifyGroupName()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            Console.Write("Введите новое название: ");
            _groupConsoleUI.UpdateGroup(groupId, Console.ReadLine());
        }

        private void FindGroup()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            _groupConsoleUI.GetGroupById(groupId);
        }

        private void CreateDailyAttendance()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            int firstLesson = GetValidNumberInput("Начальный номер занятия: ");
            int lastLesson = GetValidNumberInput("Конечный номер занятия: ");
            
            _presenceConsoleUI.GeneratePresenceForDay(firstLesson, lastLesson, groupId);
            Console.WriteLine("Записи за день созданы успешно.");
        }

        private void CreateWeeklyAttendance()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            int firstLesson = GetValidNumberInput("Начальный номер занятия: ");
            int lastLesson = GetValidNumberInput("Конечный номер занятия: ");
            
            _presenceConsoleUI.GeneratePresenceForWeek(firstLesson, lastLesson, groupId);
            Console.WriteLine("Записи за неделю созданы успешно.");
        }

        private void ViewAttendanceRecords()
        {
            DateTime date = GetValidDateInput("Введите дату (дд.мм.гггг): ");
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            List<PresenceDAO> presences = _presenceConsoleUI.ShowPresenceForDateAndGroup(DateOnly.FromDateTime(date), groupId);
            List<UserDAO> users = _userConsoleUI.GetAllUsers();
            if (presences == null || presences.Count == 0)
            {
                Console.WriteLine("Посещаемость на выбранную дату отсутствует.");
                return;
            }
            var sortedPresences = presences.OrderBy(p => p.LessonNumber)
                .ThenBy(p => p.UserId);

            Console.WriteLine($"\nПосещаемость на {date.ToShortDateString()} для группы с ID {groupId}:");
            Console.WriteLine("---------------------------------------------");

            int previousLessonNumber = -1;
            foreach (var presence in sortedPresences)
            {
                if (previousLessonNumber != presence.LessonNumber)
                {
                    Console.WriteLine("---------------------------------------------");
                    previousLessonNumber = presence.LessonNumber;
                }
                string status = presence.IsAttendance ? "Присутствует" : "Отсутствует";
                Console.WriteLine($"Пользователь: {users.Where(u=>u.UserId==presence.UserId).Select(u=>u.FIO).FirstOrDefault()}, Занятие {presence.LessonNumber}: {status}");
            }
            Console.WriteLine("---------------------------------------------");
        }

        private void RecordAbsence()
        {
            DateTime date = GetValidDateInput("Введите дату отсутствия (дд.мм.гггг): ");
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            int userId = GetValidNumberInput("Введите идентификатор участника: ");
            int startSession = GetValidNumberInput("Начальный номер занятия: ");
            int endSession = GetValidNumberInput("Конечный номер занятия: ");

            _presenceConsoleUI.MarkUserAbsent(date, groupId, userId, startSession, endSession);
        }

        private void ShowGroupAttendanceHistory()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            _presenceConsoleUI.DisplayAllPresenceByGroup(groupId);
        }

        private void DisplayAttendanceSummary()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            _presenceConsoleUI.DisplayGeneralPresence(groupId);
        }

        private void GenerateExcelReport()
        {
            _presenceConsoleUI.ExportAttendanceToExcel();
            Console.WriteLine("Отчёт успешно экспортирован.");
        }
    }
}