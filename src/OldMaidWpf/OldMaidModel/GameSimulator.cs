using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OldMaidModel
{
    public class GameSimulator
    {
        public bool IsRunning { get; set; }
        private GameMaster Master { get; set; }

        private Action GameFinishAction { get; set; }

        public GameSimulator(GameMaster master, Action gameFinishAction = null)
        {
            Master = master;
            GameFinishAction = gameFinishAction;
        }

        public void RunSimSync()
        {
            GameSimFlow();
        }

        public void RunSimAsync()
        {
            Task.Run(()=>GameSimFlow());
        }

        private void GameSimFlow()
        {
            if (Master == null) throw new Exception("Game Master is null");

            IsRunning = true;

            Master.ShuffleDeck();

            if (!IsRunning)
            {
                goto Finish;
            }

            Master.ShufflePlayerOrder();

            if (!IsRunning)
            {
                goto Finish;
            }

            Master.HandOutCard();

            if (!IsRunning)
            {
                goto Finish;
            }

            Master.ShowPlayersHand();

            if (!IsRunning)
            {
                goto Finish;
            }

            Master.Discard1stPairs();

            if (!IsRunning)
            {
                goto Finish;
            }

            while(IsRunning && !Master.IsFinished)
            {
                Master.AppointPlayer();
                Thread.Sleep(1);

            }

            if (IsRunning && Master.IsFinished)
            {
                Master.ShowLosePlayer();
            }
            
            Finish:
            if (GameFinishAction != null) GameFinishAction();
            IsRunning = false;
            return;
        }
    }
}
