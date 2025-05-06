using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDatabase.DAO;
using data.RemoteData.RemoteDataBase.DAO;

namespace PresenceDesktop
{
    public partial class GroupsWindow : Window
    {
        private readonly RemoteDatabaseContext _context;

        public GroupsWindow()
        {
            InitializeComponent();
            _context = new RemoteDatabaseContext();

            LoadGroups();

            GroupComboBox.SelectionChanged += (_, _) => LoadUsers();
            BtnRefresh.Click += (_, _) => LoadGroups();
        }

        private void LoadGroups()
        {
            GroupComboBox.ItemsSource = _context.groups.ToList();
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (GroupComboBox.SelectedItem is GroupDAO selectedGroup)
            {
                var users = _context.users
                    .Where(u => u.GroupId == selectedGroup.Id)
                    .ToList();

                UsersList.ItemsSource = users;
            }
        }

        private async void BtnAddUser_Click(object? sender, RoutedEventArgs e)
        {
            var addWindow = new AddOrEditUserWindow(_context);
            await addWindow.ShowDialog(this);
            LoadUsers();
        }

        private async void EditUser_Click(object? sender, RoutedEventArgs e)
        {
            if (UsersList.SelectedItem is UserDAO selectedUser)
            {
                var editWindow = new AddOrEditUserWindow(_context, selectedUser);
                await editWindow.ShowDialog(this);
                LoadUsers();
            }
        }

        private void DeleteUser_Click(object? sender, RoutedEventArgs e)
        {
            if (UsersList.SelectedItem is UserDAO selectedUser)
            {
                _context.users.Remove(selectedUser);
                _context.SaveChanges();
                LoadUsers();
            }
        }
    }
}