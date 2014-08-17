using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldMaidModel
{
    public class Player : IParson
    {
        #region 定数

        private const string OutputActionFmt = "{0}:{1}";
        private const string ActionShorHand = "Show Hand";
        private const string ActionDiscardPairs = "Discard Pairs";
        private const string ActionTakeCard = "Take Card";
        private const string ActionTakeCardContentFmt = "{0} From Player:{1}";
        private const string ActionDiscardCard = "Discard Cards";

        #endregion

        #region フィールド
        protected int _seed;
        #endregion

        #region プロパティ

        /// <summary>
        /// プレイヤー名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 手札
        /// </summary>
        protected Hand Hand { get; set; }

        public int HandCount { get { return Hand.CardCount; } }

        /// <summary>
        /// 勝ちフラグ
        /// </summary>
        public bool IsWin { get { return Hand.IsWin; } }

        /// <summary>
        /// 行動出力設定
        /// </summary>
        protected bool IsOutputAction { get; set; }

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
        /// <param name="isOutputAction">行動出力設定</param>
        public Player(string name,bool isOutputAction = true)
        {
            Name = name;

            Hand = new Hand();

            IsOutputAction = isOutputAction;

            _seed = Environment.TickCount;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// カードを受け取る
        /// </summary>
        /// <param name="card">カード</param>
        public void ReceiveCard(Card card)
        {
            Hand.AddCard(card);
        }

        /// <summary>
        /// 手札を表示
        /// </summary>
        public void ShowHand()
        {
            if (IsOutputAction)
                OutputAction(ActionShorHand, Hand.ToString());
        }

        /// <summary>
        /// 手札からペアを捨てる
        /// </summary>
        /// <returns></returns>
        public virtual IList<Card> DicardPairs()
        {
            var discards = Hand.DiscardPairs();

            if (IsOutputAction && discards != null && discards.Count > 0)
                OutputAction(ActionDiscardPairs, discards.CardListString());

            return discards;
        }

        /// <summary>
        /// 手札から１枚渡す
        /// </summary>
        /// <returns>カード</returns>
        public Card GiveCard()
        {
            return Hand.GiveCard();
        }

        /// <summary>
        /// 手札から指定された一枚を渡す
        /// </summary>
        /// <param name="idx">指定インデックス</param>
        /// <returns>カード</returns>
        public virtual Card TakeCard(int idx)
        {
            if (idx < 0) idx = 0;
            else if (idx >= Hand.CardCount) idx = Hand.CardCount;

            return Hand.TakeCard(idx);
        }

        /// <summary>
        /// プレイヤーからカードを取得
        /// </summary>
        /// <param name="player">取得プレイヤー</param>
        public virtual IList<Card> GetCard(Player player)
        {
            var rnd = new Random(_seed++);
            var idx = rnd.Next(player.HandCount);

            // プレイヤーからカードを取得
            var card = player.TakeCard(idx);

            if (IsOutputAction)
                OutputAction(ActionTakeCard, ActionTakeCardContentFmt, card.CardString, player.Name);

            // 同じ数字のカードを検索
            var pair = Hand.FindSameNumberCard(card);

            IList<Card> discards = null;

            if (pair != null)
            {
                // カードが見つかったら破棄
                discards = Hand.DiscardCard(card, pair);
                if (IsOutputAction)
                    OutputAction(ActionDiscardCard, discards.CardListString());
            }
            else
            {
                // カードが無かったら取得したカードを破棄
                Hand.AddCard(card);
            }

            return discards;
        }

        /// <summary>
        /// 実行結果出力
        /// </summary>
        /// <param name="action">実行内容</param>
        /// <param name="content">詳細</param>
        protected virtual void OutputAction(string action, string content, params string[] args)
        {
            if (OutputActionEvent == null) return;

            if (args != null && args.Length > 0) content = string.Format(content, args);

            OutputActionEvent(this, string.Format(OutputActionFmt, action, content));
        }

        #endregion
    }
}
