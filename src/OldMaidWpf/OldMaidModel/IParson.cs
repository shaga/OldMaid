using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldMaidModel
{
    /// <summary>
    /// 参加者インターフェース
    /// </summary>
    public interface IParson
    {
        /// <summary>
        /// 名前
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 結果出力イベント
        /// </summary>
        event Action<IParson, string> OutputActionEvent;
    }
}
