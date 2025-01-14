using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TimeMaster
{
    public class Task : INotifyPropertyChanged
    {
        private Guid _id;
        private string _title;
        private string _description;
        private DateTime _startTime;
        private DateTime _endTime;
        private DateTime _reminderTime;
        private int _priority;
        public TimeSpan ReminderOffset { get; set; }  // 提醒前的時間偏移量
        private bool _isCompleted;

        public ICommand CompleteCommand { get; set; }  // 完成任務的命令
        public ICommand UncompleteCommand { get; set; }  // 未完成任務的命令

        public Task()
        {
            CompleteCommand = new RelayCommand(CompleteTask);  // 初始化完成命令
            UncompleteCommand = new RelayCommand(UncompleteTask);  // 初始化未完成命令
        }

        // 完成任務的方法
        private void CompleteTask()
        {
            IsCompleted = true;  // 標記為完成
            OnPropertyChanged(nameof(IsCompleted));
        }

        // 未完成任務的方法
        private void UncompleteTask()
        {
            IsCompleted = false;  // 標記為未完成
            OnPropertyChanged(nameof(IsCompleted));
        }

        // 各個屬性的公開訪問器，並在設置時觸發屬性變更通知
        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }

        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value;
                OnPropertyChanged(nameof(EndTime));
            }
        }

        public DateTime ReminderTime
        {
            get => _reminderTime;
            set
            {
                _reminderTime = value;
                OnPropertyChanged(nameof(ReminderTime));
            }
        }

        public int Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged(nameof(Priority));
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // 觸發屬性變更事件的受保護方法
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // 一個用於命令綁定的簡單類，允許在 XAML 中綁定按鈕動作
    public class RelayCommand : ICommand
    {
        private Action _execute;
        private Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
