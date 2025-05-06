using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PresenceDesktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            BtnOpenGroups.Click += (_, _) => new GroupsWindow().Show();
            BtnOpenStudents.Click += (_, _) => new StudentsWindow().Show();
            BtnOpenAttendance.Click += (_, _) => new AttendanceWindow().Show();
        }
    }
}