using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Threading;


namespace TimeMaster
{
    public partial class MainWindow : Window
    {
        private TaskViewModel _viewModel;
        private DispatcherTimer timer;

        public class TaskViewModel
        {
            public ObservableCollection<Task> Tasks { get; set; }

            public TaskViewModel()
            {
                Tasks = new ObservableCollection<Task>();
            }

            public void AddTask(Task task)
            {
                Tasks.Add(task);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new TaskViewModel();
            this.DataContext = _viewModel;
        }


        private void StartTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (EndTimePicker.Value <= StartTimePicker.Value)
            {
                EndTimePicker.Value = StartTimePicker.Value.Value.AddMinutes(1);
            }
        }

        private void EndTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (EndTimePicker.Value <= StartTimePicker.Value)
            {
                EndTimePicker.Value = StartTimePicker.Value.Value.AddMinutes(1);
            }
        }


        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text;
            string description = DescriptionTextBox.Text;
            DateTime startTime = StartTimePicker.Value ?? DateTime.Now;  // Assume current time if not set
            DateTime endTime = EndTimePicker.Value ?? DateTime.Now;      // Assume current time if not set

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("所有欄位均為必填項，請完整填寫!");
                return;
            }

            Task newTask = new Task
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                StartTime = startTime,
                EndTime = endTime,
                IsCompleted = false
            };

            _viewModel.AddTask(newTask);

            ClearInputFields();
        }
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem != null)
            {
                Task taskToDelete = TaskListBox.SelectedItem as Task;
                if (taskToDelete != null)
                {
                    _viewModel.Tasks.Remove(taskToDelete);
                }
            }
            else
            {
                MessageBox.Show("請選擇一個任務來刪除。");
            }
        }

        private DateTime ConvertToDateTime(DateTime date, string time)
        {
            return DateTime.Parse($"{date.ToShortDateString()} {time}");
        }

        private DateTime CalculateReminderTime(DateTime startTime, string reminderOffset)
        {
            switch (reminderOffset)
            {
                case "1 小時":
                    return startTime.AddHours(-1);
                case "30 分鐘":
                    return startTime.AddMinutes(-30);
                case "10 分鐘":
                    return startTime.AddMinutes(-10);
                case "1 天":
                    return startTime.AddDays(-1);
                default:
                    return startTime;  // 默认无提醒
            }
        }


        private void ClearInputFields()
        {
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            StartTimePicker.Value = null;
            EndTimePicker.Value = null;
        }

    }


}