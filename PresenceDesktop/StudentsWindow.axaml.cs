using Avalonia.Controls;
using data.RemoteData.RemoteDataBase;
using System.Linq;

namespace PresenceDesktop
{
    public partial class StudentsWindow : Window
    {
        private readonly RemoteDatabaseContext _context;

        public StudentsWindow()
        {
            InitializeComponent();
            _context = new RemoteDatabaseContext();

            LoadStudents();

            BtnRefreshStudents.Click += (_, _) => LoadStudents();
        }

        private void LoadStudents()
        {
            var students = _context.users
                .Select(s => s.FIO)
                .ToList();

            StudentsList.ItemsSource = students;
        }
    }
}