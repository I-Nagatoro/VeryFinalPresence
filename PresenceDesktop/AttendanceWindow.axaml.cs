using Avalonia.Controls;
using Avalonia.Interactivity;
using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDataBase.DAO;
using System;
using System.Linq;
using data.RemoteData.RemoteDatabase.DAO;

namespace PresenceDesktop
{
    public partial class AttendanceWindow : Window
    {
        private readonly RemoteDatabaseContext _context;

        public AttendanceWindow()
        {
            InitializeComponent();
            _context = new RemoteDatabaseContext();

            GroupComboBox.SelectionChanged += (_, _) => LoadAttendance();
            DatePicker.SelectedDateChanged += (_, _) => LoadAttendance();
            BtnRefreshAttendance_Click(null, null);
        }

        private void BtnRefreshAttendance_Click(object? sender, RoutedEventArgs? e)
        {
            LoadGroups();
            LoadAttendance();
        }

        private void LoadGroups()
        {
            GroupComboBox.ItemsSource = _context.groups.ToList();

            if (GroupComboBox.SelectedIndex == -1)
                GroupComboBox.SelectedIndex = 0;

            if (DatePicker.SelectedDate == null)
                DatePicker.SelectedDate = DateTime.Today;
        }

        private void LoadAttendance()
        {
            if (GroupComboBox.SelectedItem is not GroupDAO selectedGroup || DatePicker.SelectedDate == null)
                return;

            var date = DatePicker.SelectedDate.Value.Date;

            var attendance = _context.presences
                .Where(a => a.Date == DateOnly.FromDateTime(date))
                .Join(_context.users,
                    presence => presence.UserId,
                    user => user.UserId,
                    (presence, user) => new { presence, user })
                .Where(joined => joined.user.GroupId == selectedGroup.Id)
                .Select(j => $"{j.user.FIO} - {(j.presence.IsAttendance == true ? "Присутствовал" : "Отсутствовал")} - {j.presence.Date:dd.MM.yyyy}")
                .ToList();

            AttendanceList.ItemsSource = attendance;
        }
    }
}