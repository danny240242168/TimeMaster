using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace TimeMaster
{
    public class TaskTimer
    {
        private Task _task;
        private DispatcherTimer _timer;

        public TaskTimer(Task task)
        {
            _task = task;
            _timer = new DispatcherTimer();
            _timer.Interval = task.ReminderTime - DateTime.Now;
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // 提醒逻辑
            MessageBox.Show($"Reminder: Task '{_task.Title}' is due!");
            _timer.Stop();  // 如果只需要提醒一次，停止计时器
        }

        public Task Task => _task;
    }

}
