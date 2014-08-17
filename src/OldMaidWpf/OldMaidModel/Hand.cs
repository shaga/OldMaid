using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldMaidModel
{
    /// <summary>
    /// 手札クラス
    /// </summary>
    public class Hand
    {
        #region フィールド

        private int _seed;

        #endregion

        #region プロパティ

        /// <summary>
        /// 手札
        /// </summary>
        private List<Card> Cards { get; set; }

        /// <summary>
        /// 手札の枚数
        /// </summary>
        public int CardCount { get { return Cards != null ? Cards.Count : 0; } }

        public bool IsWin { get { return CardCount == 0; } }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Hand()
        {
            Cards = new List<Card>();
            _seed = Environment.TickCount;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 手札にカードを追加
        /// </summary>
        /// <param name="card">追加するカード</param>
        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        /// <summary>
        /// 手札の文字列表記
        /// </summary>
        public override string ToString()
        {
            return Cards.CardListString();
        }

        /// <summary>
        /// 手札からペアを破棄
        /// </summary>
        /// <returns>破棄したカードリスト</returns>
        public IList<Card> DiscardPairs()
        {
            var discards = new List<Card>();

            for (var i = 0; i < CardCount; i++)
            {
                var check = Cards[i];
                var pair = FindSameNumberCard(check);
                if (pair != null)
                {
                    discards.AddRange(DiscardCard(check, pair));
                    i--;
                }
            }

            return discards;
        }

        /// <summary>
        /// 同じカード数字のを検索
        /// </summary>
        /// <param name="card">対象のカード</param>
        /// <returns></returns>
        public Card FindSameNumberCard(Card card)
        {
            return Cards.FirstOrDefault(c => c.IsSameNumberAndDiffMark(card));
        }

        /// <summary>
        /// カードを破棄
        /// </summary>
        /// <param name="c1">カード１枚目</param>
        /// <param name="c2">カード２枚目</param>
        /// <returns>破棄したカードリスト</returns>
        public IList<Card> DiscardCard(Card c1, Card c2)
        {
            if (c1 != null && Cards.Contains(c1)) Cards.Remove(c1);
            if (c2 != null && Cards.Contains(c2)) Cards.Remove(c2);

            return new List<Card>() { c1, c2 };
        }

        /// <summary>
        /// 指定されたカードを渡す
        /// </summary>
        /// <param name="idx">カードインデックス</param>
        /// <returns>カード</returns>
        public Card TakeCard(int idx)
        {
            if (idx < 0) idx = 0;
            if (CardCount <= idx) idx = CardCount - 1;

            var card = Cards[idx];

            Cards.RemoveAt(idx);

            return card;
        }

        /// <summary>
        /// カードを一枚渡す
        /// </summary>
        /// <returns>カード</returns>
        public Card GiveCard()
        {
            var rnd = new Random(_seed++);

            var idx = rnd.Next(CardCount);

            var card = Cards[idx];

            Cards.RemoveAt(idx);

            return card;
        }

        public void ShuffleCardOrder()
        {
            var rnd = new Random(_seed++);

            var newList = new List<Card>();

            while(Cards.Any())
            {
                var idx = rnd.Next(Cards.Count);

                newList.Add(Cards[idx]);

                Cards.RemoveAt(idx);
            }

            Cards = newList;
        }

        #endregion
    }
}
