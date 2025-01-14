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
using System.Windows.Forms;
using System.Timers;
using MessageBox = System.Windows.MessageBox;
using System.Globalization;

namespace TimeMaster
{
    public partial class MainWindow : Window
    {
        private TaskViewModel _viewModel; // 用來管理任務的 ViewModel
        private DispatcherTimer timer; // 處理UI更新的計時器
        private NotifyIcon trayIcon; // 系統托盤圖標
        private System.Timers.Timer checkTimer; // 檢查任務是否需要提醒的計時器

        public class TaskViewModel
        {
            public ObservableCollection<Task> Tasks { get; set; } // 儲存任務的集合

            public TaskViewModel()
            {
                Tasks = new ObservableCollection<Task>(); // 初始化任務集合
            }

            public void AddTask(Task task)
            {
                Tasks.Add(task); // 新增任務
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeTrayIcon(); // 初始化系統托盤圖標
            SetupBackgroundTaskChecker(); // 設置後台任務檢查計時器
            _viewModel = new TaskViewModel(); // 初始化ViewModel
            this.DataContext = _viewModel; // 設置數據上下文
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("task.ico"), // 設置圖標路徑
                Text = "TimeMaster",
                Visible = true
            };

            trayIcon.MouseClick += (sender, args) =>
            {
                this.Show(); // 顯示窗口
                this.WindowState = WindowState.Normal; // 正常狀態
            };

            this.StateChanged += MainWindow_StateChanged; // 註冊狀態變化事件
            this.Closing += MainWindow_Closing; // 註冊關閉事件
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized) // 如果窗口最小化
            {
                this.Hide(); // 隱藏窗口
                trayIcon.ShowBalloonTip(1000, "Notification", "Your application is still running.", ToolTipIcon.Info); // 顯示通知
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Do you really want to close?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // 取消關閉
                this.WindowState = WindowState.Minimized; // 最小化窗口
            }
            else
            {
                trayIcon.Dispose(); // 釋放資源
            }
        }

        private void SetupBackgroundTaskChecker()
        {
            checkTimer = new System.Timers.Timer(60000); // 每分鐘檢查一次
            checkTimer.Elapsed += CheckTasks; // 註冊檢查任務事件
            checkTimer.Start(); // 開始計時器
        }

        private void CheckTasks(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            foreach (var task in _viewModel.Tasks)
            {
                if (!task.IsCompleted && (task.EndTime - task.ReminderOffset) <= now)
                {
                    ShowReminder(task); // 顯示提醒
                    task.IsCompleted = true;  // 假設任務完成後不再提醒
                }
            }
        }

        private void ShowReminder(Task task)
        {
            // 在這裡顯示任務提醒
            this.Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"提醒: {task.Title} - {task.Description}", "任務提醒", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void StartTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (EndTimePicker.Value <= StartTimePicker.Value)
            {
                EndTimePicker.Value = StartTimePicker.Value.Value.AddMinutes(1); // 確保結束時間大於開始時間
            }
        }

        private void EndTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (EndTimePicker.Value <= StartTimePicker.Value)
            {
                EndTimePicker.Value = StartTimePicker.Value.Value.AddMinutes(1); // 確保結束時間大於開始時間
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text; // 獲取標題
            string description = DescriptionTextBox.Text; // 獲取描述
            DateTime startTime = StartTimePicker.Value ?? DateTime.Now; // 獲取開始時間
            DateTime endTime = EndTimePicker.Value ?? DateTime.Now; // 獲取結束時間
            TimeSpan reminderOffset = TimeSpan.FromMinutes(Convert.ToDouble((ReminderTimeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString().Split(' ')[0]));

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("所有欄位均為必填項，請完整填寫!");
                return;
            }

            Task newTask = new Task
            {
                Id = Guid.NewGuid(), // 生成新的ID
                Title = title,
                Description = description,
                StartTime = startTime,
                EndTime = endTime,
                ReminderOffset = reminderOffset,
                IsCompleted = false
            };

            _viewModel.AddTask(newTask); // 新增任務

            ClearInputFields(); // 清除輸入框
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem != null)
            {
                Task taskToDelete = TaskListBox.SelectedItem as Task;
                if (taskToDelete != null)
                {
                    _viewModel.Tasks.Remove(taskToDelete); // 刪除任務
                }
            }
            else
            {
                MessageBox.Show("請選擇一個任務來刪除。");
            }
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var task = button.CommandParameter as Task;
            if (task != null)
            {
                task.IsCompleted = true; // 標記任務為完成
                // 更新UI
                _viewModel.Tasks.Add(task);
                _viewModel.Tasks.Remove(task);
            }
        }

        private void UncompleteTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var task = button.CommandParameter as Task;
            if (task != null)
            {
                task.IsCompleted = false; // 標記任務為未完成
                // 更新UI
                _viewModel.Tasks.Add(task);
                _viewModel.Tasks.Remove(task);
            }
        }

        private void ClearInputFields()
        {
            TitleTextBox.Clear(); // 清除標題欄位
            DescriptionTextBox.Clear(); // 清除描述欄位
            StartTimePicker.Value = null; // 重設開始時間
            EndTimePicker.Value = null; // 重設結束時間
            ReminderTimeComboBox.Items.Clear(); // 清除提醒時間選項
        }
    }
    // 轉換器，用於顯示任務完成狀態的顏色
    public class TaskCompletionColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
                return System.Windows.Media.Brushes.Green; // 完成顯示綠色
            return System.Windows.Media.Brushes.Red; // 未完成顯示紅色
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    // 轉換器，用於將布林值轉換為「完成」或「未完成」的字符串
    public class CompletionStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "    完成" : "未完成"; // 如果是 true 則返回 "完成"
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
