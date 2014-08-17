using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using OldMaidWpf;
using OldMaidWpf.View;
using OldMaidModel;

namespace OldMaidWpf.View.Simulator
{
    public class GameHistoryItem : INotifyPropertyChanged
    {
        #region Implements INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null) propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        public IParson Parson { get; set; }
        public string Action { get; set; }
        public bool IsMaster { get { return Parson is GameMaster; } }
    }

    public class SimulatorViewModel : INotifyPropertyChanged
    {
        #region Implements INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null) 
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                RaiseCommandCanExecute();
            }
        }

        #endregion

        #region プロパティ

        #region 追加プレイヤー名

        private string _addPlayerName;

        /// <summary>
        // 追加プレイヤー名
        /// </summary>
        public string AddPlayerName
        {
            get { return _addPlayerName; }
            set
            {
                if (_addPlayerName == value) return;
                _addPlayerName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ゲームマスター名

        private string _masterName;

        /// <summary>
        /// ゲームマスター名
        /// </summary>
        public string MasterName
        {
            get { return _masterName; }
            set
            {
                if (_masterName == value) return;
                _masterName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region プレイヤーリスト

        private ObservableCollection<string> _playerList;

        public ObservableCollection<string> PlayerList
        {
            get { return _playerList ?? (_playerList = new ObservableCollection<string>()); }
        }

        private CollectionViewSource _playerCollectionSource;

        public CollectionViewSource PlayerCollectionSource
        {
            get
            {
                if (_playerCollectionSource == null)
                {
                    _playerCollectionSource = new CollectionViewSource() { Source = PlayerList };
                    PlayerList.CollectionChanged += (s, e) => _playerCollectionSource.View.Refresh();
                }

                return _playerCollectionSource;
            }
        }

        public ICollectionView PlayerCollectionView { get { return PlayerCollectionSource.View; } }

        #endregion

        #region ゲーム履歴

        private ObservableCollection<GameHistoryItem> _gameHistory;

        public ObservableCollection<GameHistoryItem> GameHistory
        {
            get { return _gameHistory ?? (_gameHistory = new ObservableCollection<GameHistoryItem>()); }
        }

        private CollectionViewSource _gemeHistoryCollectionSource;

        public CollectionViewSource GameHistoryCollectionSource
        { 
            get
            {
                if (_gemeHistoryCollectionSource == null)
                {
                    _gemeHistoryCollectionSource = new CollectionViewSource() { Source = GameHistory };
                    GameHistory.CollectionChanged += (s, e) => _gemeHistoryCollectionSource.View.Refresh();
                }

                return _gemeHistoryCollectionSource;
            }
        }

        public ICollectionView GameHistoryCollectionView { get { return GameHistoryCollectionSource.View; } }

        #endregion

        #region シミュレータ

        //private OldMaidGameSim _sim;

        //public OldMaidGameSim Sim {
        //    get { return _sim; } 
        //    set
        //    {
        //        if (_sim == value) return;
        //        _sim = value;
        //        OnPropertyChanged();
        //    }
        //}

        private GameSimulator _sim;

        public GameSimulator Sim
        {
            get { return _sim; }
            set
            {
                if (_sim == value) return;
                _sim = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ゲーム状態

        private bool _isGamePlaying;

        public bool IsGamePlaying
        {
            get { return _isGamePlaying; }
            set
            {
                if (_isGamePlaying == value) return;
                _isGamePlaying = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region コマンド

        #region プレイヤー追加

        private RelayCommand _addPlayerCommand;

        public RelayCommand AddPlayerCommand
        {
            get
            {
                return _addPlayerCommand ?? (_addPlayerCommand = new RelayCommand((p) => AddPlayer(), (p) => CanAddPlayer()));
            }
        }

        #endregion

        #region プレイヤー削除

        private RelayCommand _delPlayerCommand;

        public RelayCommand DelPlayerCommand
        {
            get
            {
                return _delPlayerCommand ??
                    (_delPlayerCommand = new RelayCommand((p) => DelPlayer(), (p) => CanDelPlayer()));
            }
        }

        #endregion

        #region ゲーム開始コマンド

        private RelayCommand _startGameCommand;

        public RelayCommand StartGameCommand
        {
            get
            {
                return _startGameCommand ??
                    (_startGameCommand = new RelayCommand((p) => StartGame(), (p) => CanStartGame()));
            }
        }

        #endregion

        #region ゲーム中止コマンド

        private RelayCommand _stopGameCommand;

        public RelayCommand StopGameCommand
        {
            get
            {
                return _stopGameCommand ??
                    (_stopGameCommand = new RelayCommand((p) => StopGame(), (p) => CanStopGame()));
            }
        }

        #endregion

        #endregion

        #endregion

        #region コマンド実装

        #region プレイヤー追加

        /// <summary>
        /// プレイヤー追加コマンド
        /// </summary>
        private void AddPlayer()
        {
            if (!CanAddPlayer()) return;

            PlayerList.Add(AddPlayerName);

            AddPlayerName = string.Empty;
        }

        /// <summary>
        /// プライヤー追加コマンド実行可否判定
        /// </summary>
        /// <returns>true=実行可/false=実行不可</returns>
        private bool CanAddPlayer()
        {
            // プレイヤー名の入力があるかどうか
            return !string.IsNullOrEmpty(AddPlayerName) && !PlayerList.Contains(AddPlayerName) && !IsGamePlaying;
        }
        #endregion

        #region プレイヤー削除

        private void DelPlayer()
        {
            if (!CanDelPlayer()) return;

            PlayerList.Remove(PlayerCollectionView.CurrentItem as string);
        }

        private bool CanDelPlayer()
        {
            return PlayerCollectionView.CurrentItem != null && !IsGamePlaying;
        }

        #endregion

        #region ゲーム開始

        private void StartGame()
        {
            if (!CanStartGame()) return;

            GameHistory.Clear();

            var master = new GameMaster(MasterName);

            master.OutputActionEvent += OutputParsonAction;

            foreach(var name in PlayerList)
            {
                var player = new Player(name);

                player.OutputActionEvent += OutputParsonAction;

                master.AddPlayer(player);
            }

            Sim = new GameSimulator(master, () =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsGamePlaying = false;
                    Sim = null;
                    RaiseCommandCanExecute();
                });
            });

            Sim.RunSimAsync();
            IsGamePlaying = true;
            RaiseCommandCanExecute();
        }

        private void OutputParsonAction(IParson parson, string action)
        {
            if (Application.Current == null || Application.Current.Dispatcher == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                GameHistory.Add(new GameHistoryItem() { Parson = parson, Action = action });
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private bool CanStartGame()
        {
            return !string.IsNullOrEmpty(MasterName) && PlayerList.Count >= 2 && !IsGamePlaying;
        }

        #endregion

        private void StopGame()
        {
            if (!CanStopGame()) return;

            if (Sim == null) return;

            Sim.IsRunning = false;
        }

        private bool CanStopGame()
        {
            return IsGamePlaying;
        }

        #endregion

        private void RaiseCommandCanExecute()
        {
            StartGameCommand.RaiseCanExecuteChanged();
            StopGameCommand.RaiseCanExecuteChanged();
            AddPlayerCommand.RaiseCanExecuteChanged();
            DelPlayerCommand.RaiseCanExecuteChanged();
        }
    }
}
