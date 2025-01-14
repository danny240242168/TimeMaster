using System;
using System.Collections.ObjectModel; // 支持數據綁定的動態數據集合
using System.Linq;
using System.ComponentModel; // 提供 INotifyPropertyChanged 介面
using System.Windows.Threading; // 提供 Windows 調度器，用於 UI 線程計時

namespace TimeMaster
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        // 宣告一個 DispatcherTimer，用於設定定時提醒
        private DispatcherTimer _reminderTimer;

        // 任務集合，支持動態更新界面
        private ObservableCollection<Task> _tasks = new ObservableCollection<Task>();

        // 任務計時器集合
        private List<TaskTimer> _taskTimers = new List<TaskTimer>();

        public TaskViewModel()
        {
            // 初始化定時器，設定每60秒檢查一次是否有需要提醒的任務
            _reminderTimer = new DispatcherTimer();
            _reminderTimer.Interval = TimeSpan.FromSeconds(60);
            _reminderTimer.Tick += CheckForReminders; // 註冊定時器事件
            _reminderTimer.Start(); // 啟動定時器
        }

        // 公開任務集合，允許外部訪問
        public ObservableCollection<Task> Tasks => _tasks;

        // 檢查是否需要對任務發送提醒
        private void CheckForReminders(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            foreach (var task in _tasks)
            {
                // 如果當前時間已達到任務的設定提醒時間，且任務未完成，則發送提醒
                if (task.ReminderTime <= currentTime && !task.IsCompleted)
                {
                    SendReminder(task);
                }
            }
        }

        // 發送提醒的具體實現，這裡以輸出至控制台為例
        private void SendReminder(Task task)
        {
            Console.WriteLine($"提醒：{task.Title} 即將開始，請準備！");
        }

        // 獲取指定任務的計時器
        public TaskTimer GetTaskTimer(Task task)
        {
            return _taskTimers.FirstOrDefault(t => t.Task == task);
        }

        // 屬性變更事件，用於通知 UI 更新
        public event PropertyChangedEventHandler PropertyChanged;

        // 觸發屬性變更事件
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 向任務集合中新增任務
        public void AddTask(Task task)
        {
            Tasks.Add(task);
        }
    }
}
