using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OldMaidWpf.View
{
    /// <summary>
    /// ICommand実装
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region 実行イベント
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        #endregion

        #region コンストラクタ
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> execute) : this(execute, (p) => true) { }
        #endregion

        #region ICommand実装
        public bool CanExecute(object parameter)
        {
            return _canExecute != null ? _canExecute(parameter) : false;
        }

        public void Execute(object parameter)
        {
            if (_execute == null) return;
            _execute(parameter);
        }

        private event EventHandler _canExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                _canExecuteChanged += value;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                _canExecuteChanged -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            var canExecuteChange = _canExecuteChanged;
            if (canExecuteChange == null) return;
            canExecuteChange(this, EventArgs.Empty);
        }
        #endregion
    }
}
