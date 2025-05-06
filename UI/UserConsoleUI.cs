using data.RemoteData.RemoteDataBase;
using data.Repository;
using domain.UseCase;
using System;
using System.Text;
using data.RemoteData.RemoteDatabase.DAO;

namespace ui
{
    public class UserConsoleUI
    {
        private readonly UserUseCase _userUseCase;
        private readonly GroupUseCase _groupUseCase;

        public UserConsoleUI(UserUseCase userUseCase, GroupUseCase groupUseCase)
        {
            _userUseCase = userUseCase;
            _groupUseCase = groupUseCase;
        }

        public void ShowAllUsers()
        {
            Console.WriteLine("\n-=-=-=- Список всех студентов -=-=-=-");
            StringBuilder userOutput = new StringBuilder();
            var users = _userUseCase.GetAllUsers();
            
            if (users == null || !users.Any())
            {
                Console.WriteLine("Нет пользователей для отображения.");
                return;
            }
            
            var groups = _groupUseCase.GetAllGroups();

            foreach (var user in users)
            {
                var group = groups?.FirstOrDefault(g => g.Id == user.GroupId);
                string groupName = group != null ? group.Name : $"Группа {user.GroupId} не найдена";

                userOutput.AppendLine($"{user.UserId}\t{user.FIO}\t{groupName}");
            }

            Console.WriteLine(userOutput);
            Console.WriteLine("===============================\n");
        }

        
        public void RemoveUserByUserId(int userId)
        {
            string output = _userUseCase.RemoveUserByUserId(userId) ? "Пользователь удален" : "Пользователь не найден";
            Console.WriteLine($"\n{output}\n");
        }

        public void UpdateUserByUserId(int userId)
        {
            try
            {
                var user = _userUseCase.FindUserByUserId(userId);


                Console.WriteLine($"Текущие данные: {user.FIO}");
                Console.Write("\nВведите новое ФИО: ");
                string newFIO = Console.ReadLine();
                Console.Write("\nВведите новый ID группы (или оставьте такой же): ");
                string GroupId = Console.ReadLine();
                if (int.TryParse(GroupId, out int groupId))
                {
                    _userUseCase.UpdateUserByUserId(userId, newFIO, groupId);
                    Console.WriteLine("\nПользователь обновлен.\n");
                }
                else
                {
                    _userUseCase.UpdateUserByUserId(userId, newFIO, user.GroupId);
                    Console.WriteLine("\nПользователь обновлен.\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}\n");
            }
        }

        public void FindUserByUserId(int userId)
        {
            UserDAO? user = _userUseCase.FindUserByUserId(userId);
            if (user != null)
            {
                Console.WriteLine("Пользователь найден: ");
                Console.WriteLine($"\nID: {user.UserId}, ФИО: {user.FIO}, Группа: {user.Group.Name}\n");
            }
            else
            {
                Console.WriteLine("\nПользователь не найден.\n");
            }
        }

        public List<UserDAO> GetAllUsers()
        {
            return _userUseCase.GetAllUsers();
        }
    }
}
