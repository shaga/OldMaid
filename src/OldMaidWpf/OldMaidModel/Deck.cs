using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldMaidModel
{
    /// <summary>
    /// トランプデッキ
    /// </summary>
    public class Deck
    {
        #region 定数
        private const int ShuffleCount = 1000;
        #endregion

        #region プロパティ

        /// <summary>
        /// カードリスト
        /// </summary>
        public List<Card> Cards { get; private set; }

        /// <summary>
        /// カードの残り枚数
        /// </summary>
        public int CardCount { get { return Cards != null ? Cards.Count : 0; } }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="jokerCount">ジョーカーの枚数</param>
        public Deck(int jokerCount)
        {
            if (jokerCount < Card.MinJokerCount || Card.MaxJokerCount < jokerCount)
                throw new Exception(string.Format("Joker Count[{0}] is not {1} to {2}.", jokerCount, Card.MinJokerCount, Card.MaxJokerCount));

            Cards = new List<Card>();

            for (var i = 0; i < Card.MarkCardCount + jokerCount; i++)
            {
                Cards.Add(new Card(i));
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 上から一枚取る
        /// </summary>
        /// <returns>カード</returns>
        public Card TakeCardFromTop()
        {
            if (CardCount == 0) return null;

            var card = Cards[0];
            Cards.RemoveAt(0);

            return card;
        }

        /// <summary>
        /// デッキを混ぜる
        /// </summary>
        public void ShuffleCards()
        {
            var rnd = new Random(Environment.TickCount);
            for(var i = 0; i < ShuffleCount; i++)
            {
                var selectIdx = rnd.Next(CardCount);
                var insertIdx = rnd.Next(CardCount);
                var card = Cards[selectIdx];
                Cards.RemoveAt(selectIdx);
                if (insertIdx > CardCount) Cards.Add(card);
                else Cards.Insert(insertIdx, card);
            }
        }

        #endregion
    }
}
