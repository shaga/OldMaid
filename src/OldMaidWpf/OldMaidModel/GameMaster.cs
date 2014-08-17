using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldMaidModel
{
    /// <summary>
    /// ババ抜き進行役
    /// </summary>
    public class GameMaster : IParson
    {
        #region 定数

        private const int JokerCount = 1;
        private const string ActionShowPlayerHand = "Show Player's Hand";
        private const string ActionDicardPairs = "Discard 1st Pairs";
        private const string ActionCallWinnerFmt = "Player:{0} is Win.";
        private const string ActionCallNextPlayer = "Next Player is {0}";
        private const string ActionCallLosePlayer = "Player:{0} is Lose.";

        #endregion
        #region プロパティ

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// トランプデッキ
        /// </summary>
        private Deck Deck { get; set; }

        /// <summary>
        /// プレイヤーリスト
        /// </summary>
        private List<Player> Players { get; set; }

        /// <summary>
        /// ゲーム終了状態
        /// </summary>
        public bool IsFinished { get { return Players != null && Players.Count(p => !p.IsWin) == 1; } }

        /// <summary>
        /// 最終プレイヤー
        /// </summary>
        private Player LastPlayer { get; set; }

        #endregion


        #region イベント

        /// <summary>
        /// 行動出力イベント
        /// </summary>
        public event Action<IParson, string> OutputActionEvent;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">名前</param>
        public GameMaster(string name)
        {
            Name = name;

            Deck = new Deck(JokerCount);

            Players = new List<Player>();
        }

        #endregion

        #region メソッド

        /// <summary>
        /// プレイヤー追加
        /// </summary>
        /// <param name="player">追加プレイヤー</param>
        public void AddPlayer(Player player)
        {
            // すでにいるプレイヤーは追加しない
            if (Players == null || Players.Contains(player)) return;

            Players.Add(player);
        }

        /// <summary>
        /// プレイヤー順序設定
        /// </summary>
        public void ShufflePlayerOrder()
        {
            if (Players == null || Players.Count < 2) return;

            var newList = new List<Player>();

            var rnd = new Random(Environment.TickCount);

            while(Players.Any())
            {
                var idx = rnd.Next(Players.Count);
                newList.Add(Players[idx]);
                Players.RemoveAt(idx);
            }

            Players = newList;
        }

        /// <summary>
        /// デッキからカードを配る
        /// </summary>
        public void HandOutCard()
        {
            if (Players == null || Players.Count < 2) return;

            var idx = 0;
            while (Deck.CardCount > 0)
            {
                Players[idx].ReceiveCard(Deck.TakeCardFromTop());
                idx = (idx + 1) % Players.Count;
            }
        }

        /// <summary>
        /// デッキを混ぜる
        /// </summary>
        public void ShuffleDeck()
        {
            Deck.ShuffleCards();
        }

        /// <summary>
        /// 各プレイヤーの手札を出力
        /// </summary>
        public void ShowPlayersHand()
        {
            OutputAction(ActionShowPlayerHand);
            foreach(var player in Players)
            {
                player.ShowHand();
            }
        }

        /// <summary>
        /// 各プレイヤーの手札内のペアを破棄
        /// </summary>
        public void Discard1stPairs()
        {
            OutputAction(ActionDicardPairs);

            foreach(var player in Players)
            {
                player.DicardPairs();
                if (player.IsWin) OutputAction(ActionCallWinnerFmt, player.Name);
            }
        }

        /// <summary>
        /// プレイヤーを指名
        /// </summary>
        public void AppointPlayer()
        {
            // ゲームが終わっていたら何もしない
            if (IsFinished) return;

            // 次のプレイヤーを取得
            var next = GetNextPlayer();

            if (next == null)
                throw new Exception("Game is Finished");

            OutputAction(ActionCallNextPlayer, next.Name);

            // カードを引くプレイヤーを指名
            next.GetCard(LastPlayer);

            // カードを引かれたプレイヤーの勝ち抜けを判定
            if (LastPlayer.IsWin) OutputAction(ActionCallWinnerFmt, LastPlayer.Name);
            // カードを引いたプレイヤーの勝ち抜けを判定
            if (next.IsWin) OutputAction(ActionCallWinnerFmt, next.Name);

            // 最後のプレイヤーを更新
            LastPlayer = next;
        }

        /// <summary>
        /// 負けたプレイヤーを表示
        /// </summary>
        public void ShowLosePlayer()
        {
            try
            {
                // 負けている一人を取得
                var player = Players.SingleOrDefault(p => !p.IsWin);

                OutputAction(ActionCallLosePlayer, player.Name);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 次のプレイヤーを取得
        /// </summary>
        /// <returns>次にカードを引くプレイヤー</returns>
        private Player GetNextPlayer()
        {
            if (LastPlayer == null)
            {
                // 最後のプレイヤーがnullなら最初なので、カードをひかれるのは最後のプレイヤー
                LastPlayer = Players.LastOrDefault(p => !p.IsWin);
                // カードを引くのは最初のプレイヤー
                return Players.FirstOrDefault(p => !p.IsWin);
            }

            if (LastPlayer.IsWin)
            {
                // 最後のプレイヤーが勝ち抜けている場合

                // 最後のプレイヤーのリストインデックスを取得
                var lastIdx = Players.IndexOf(LastPlayer);
                LastPlayer = null;

                if (lastIdx > 0)
                {
                    // 最後のプレイヤーがリストの先頭以外ならその前の最後の残っているを最終プレイヤーとする
                    LastPlayer = Players.Take(lastIdx).LastOrDefault(p => !p.IsWin);
                }
                if (LastPlayer == null)
                {
                    // 見つからない場合は、最後のプレイヤーの後で残っいるプレイヤーを最終プレイヤーとする
                    LastPlayer = Players.Skip(lastIdx + 1).FirstOrDefault(p => !p.IsWin);
                }
            }

            Player next = null;

            // 最終プレイヤーのリストインデックスを取得
            var idx = Players.IndexOf(LastPlayer);

            // 最終プレイヤーがリストの末尾でなければ最終プレイヤーの後ろの最初のプレイヤーを次とする
            if (idx < Players.Count - 1) next = Players.Skip(idx + 1).FirstOrDefault(p => !p.IsWin);

            // 見つからない場合は、最初に見つかった残っているプレイヤーを次とする
            if (next == null) next = Players.FirstOrDefault(p => !p.IsWin);

            return next;
        }

        /// <summary>
        /// 行動出力
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="args">フォーマット指定時の追加情報</param>
        private void OutputAction(string content, params string[] args)
        {
            if (OutputActionEvent == null) return;

            if (args != null && args.Length > 0) content = string.Format(content, args);

            OutputActionEvent(this, content);
        }

        #endregion
    }
}
