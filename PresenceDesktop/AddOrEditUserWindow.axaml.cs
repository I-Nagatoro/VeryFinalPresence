using Avalonia.Controls;
using Avalonia.Interactivity;
using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDataBase.DAO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using data.RemoteData.RemoteDatabase.DAO;

namespace PresenceDesktop
{
    public partial class AddOrEditUserWindow : Window
    {
        private readonly RemoteDatabaseContext _context;
        private readonly UserDAO? _editingUser;

        public AddOrEditUserWindow()
        {
            InitializeComponent();
        }

        public AddOrEditUserWindow(RemoteDatabaseContext context, UserDAO? user = null)
            : this()
        {
            _context = context;
            _editingUser = user;

            var groups = _context.groups.ToList();
            GroupComboBox.ItemsSource = groups;

            if (_editingUser != null)
            {
                FIOTextBox.Text = _editingUser.FIO;
                GroupComboBox.SelectedItem = groups.FirstOrDefault(g => g.Id == _editingUser.GroupId);
            }
            else
            {
                GroupComboBox.SelectedIndex = 0;
            }
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            var fio = FIOTextBox.Text?.Trim();
            var group = GroupComboBox.SelectedItem as GroupDAO;

            if (string.IsNullOrWhiteSpace(fio) || group == null)
            {
                ShowMessageBox("Пожалуйста, заполните все поля.");
                return;
            }

            if (_editingUser != null)
            {
                _editingUser.FIO = fio;
                _editingUser.GroupId = group.Id;
            }
            else
            {
                var newUser = new UserDAO
                {
                    FIO = fio,
                    GroupId = group.Id
                };
                _context.users.Add(newUser);
            }

            _context.SaveChanges();
            Close();
        }

        private async void ShowMessageBox(string message)
        {
            await MessageBox.Show(this, message, "Ошибка", MessageBox.MessageBoxButtons.Ok);
        }
    }
    
    public static class MessageBox
    {
        public enum MessageBoxButtons { Ok }

        public static async Task Show(Window owner, string message, string title = "Сообщение", MessageBoxButtons buttons = MessageBoxButtons.Ok)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Thickness(20),
                    Spacing = 10,
                    Children =
                    {
                        new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap },
                        new Button
                        {
                            Content = "OK",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Width = 80
                        }
                    }
                }
            };

            await dialog.ShowDialog(owner);
        }
    }

}
