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
        private Task _task; // 關聯的任務實例
        private DispatcherTimer _timer; // 用於執行倒計時的計時器

        // 建構函數，初始化 TaskTimer 並設置計時器
        public TaskTimer(Task task)
        {
            _task = task; // 設置任務
            _timer = new DispatcherTimer(); // 創建計時器
            _timer.Interval = task.ReminderTime - DateTime.Now; // 設置計時器的觸發間隔
            _timer.Tick += Timer_Tick; // 註冊計時器觸發事件
            _timer.Start(); // 啟動計時器
        }

        // 計時器觸發事件的處理函數
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 提醒邏輯，當時間到達時顯示提醒
            System.Windows.MessageBox.Show($"提醒: 任務 '{_task.Title}' 已到期！");
            _timer.Stop();  // 如果只需提醒一次，則停止計時器
        }

        // 獲取關聯的任務
        public Task Task => _task;
    }

}
