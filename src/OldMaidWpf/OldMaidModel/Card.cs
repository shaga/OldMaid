using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldMaidModel
{
    public enum EMark
    {
        Joker = -1,
        Spade = 0,
        Heart,
        Dia,
        Club,
        Count,
    }

    public static class OldMaidModelExtention
    {
        public static string CardListString(this IEnumerable<Card> cards)
        {
            return cards != null && cards.Count() > 0 ?
                string.Join(" ", cards.Select(c => c.CardString)) :
                string.Empty;
        }
    }

    /// <summary>
    /// トランプのカード
    /// </summary>
    public class Card
    {
        #region 定数

        private const int MarkCardMinNum = 1;
        private const int MarkCardMaxNum = 13;
        public const int MarkCount = (int)EMark.Count;
        public const int MinJokerCount = 0;
        public const int MaxJokerCount = 2;
        public const int MinCardNo = 0;
        public const int MarkCardCount = MarkCardMaxNum * MarkCount;
        public const int MaxCardNo = MarkCardCount + MaxJokerCount - 1;
        private const int CardStringLength = 3;
        private const int MarkStringLength = 1;

        #endregion

        #region プロパティ

        /// <summary>
        /// カード番号
        /// </summary>
        public int CardNo { get; private set; }

        /// <summary>
        /// ジョーカー判定
        /// </summary>
        public bool IsJoker
        {
            get { return CardNo >= MarkCardCount; }
        }

        /// <summary>
        /// カード数字
        /// </summary>
        public int Number
        {
            get
            {
                if (IsJoker) return CardNo;
                return CardNo % MarkCardMaxNum + MarkCardMinNum;
            }
        }

        /// <summary>
        /// マーク
        /// </summary>
        public EMark Mark
        {
            get
            {
                if (IsJoker) return EMark.Joker;
                return (EMark)(CardNo / MarkCardMaxNum);
            }
        }

        /// <summary>
        /// カードの表示文字列
        /// </summary>
        public string CardString
        {
            get
            {
                string str;

                if (IsJoker) str = Mark.ToString().Substring(0, CardStringLength);
                else
                {
                    str = Mark.ToString().Substring(0, MarkStringLength) + Number.ToString();
                }

                return str.PadRight(CardStringLength);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 異なるマークの同一の数字であるかを判定
        /// </summary>
        /// <param name="card">対象のカード</param>
        /// <returns></returns>
        public bool IsSameNumberAndDiffMark(Card card)
        {
            return Mark != card.Mark && Number == card.Number;
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cardNo">カード番号</param>
        public Card(int cardNo)
        {
            if (cardNo < MinCardNo || MaxCardNo < cardNo)
                throw new Exception(string.Format("CardNo[{0}] is not {1} to {2}.", cardNo, MinCardNo, MaxCardNo));

            CardNo = cardNo;
        }

        #endregion
    }
}
