using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Windows.Threading;

namespace TimeMaster
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer _reminderTimer;  // 使用 DispatcherTimer
        private ObservableCollection<Task> _tasks = new ObservableCollection<Task>();
        private List<TaskTimer> _taskTimers = new List<TaskTimer>();

        public TaskViewModel()
        {
            // 初始化定時器，每60秒檢查一次任務提醒
            _reminderTimer = new DispatcherTimer();
            _reminderTimer.Interval = TimeSpan.FromSeconds(60);
            _reminderTimer.Tick += CheckForReminders;
            _reminderTimer.Start();
        }

        public ObservableCollection<Task> Tasks => _tasks;

        //public void AddTask(Task task)
        //{
        //    _tasks.Add(task);  // 添加任務
        //    _taskTimers.Add(new TaskTimer(task));
        //}

        private void CheckForReminders(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            foreach (var task in _tasks)
            {
                if (task.ReminderTime <= currentTime && !task.IsCompleted)
                {
                    SendReminder(task);
                }
            }
        }

        private void SendReminder(Task task)
        {
            // 在此處理提醒邏輯，例如可以使用 MessageBox 顯示提示
            Console.WriteLine($"提醒：{task.Title} 即將開始，請準備！");
        }

        public TaskTimer GetTaskTimer(Task task)
        {
            return _taskTimers.FirstOrDefault(t => t.Task == task);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddTask(Task task)
        {
            Tasks.Add(task);
        }
    }
}
