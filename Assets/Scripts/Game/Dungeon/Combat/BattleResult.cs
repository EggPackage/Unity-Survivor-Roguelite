namespace Game.Dungeon.Combat
{
    /// <summary>
    /// 一次地牢战斗的结算结果，作为 E_Dungeon_BattleResolved 事件的参数。
    ///
    /// ==== 设计说明 ====
    /// E_EventType 里早已声明本事件"携带 BattleResult"，但类型此前一直没建，
    /// 导致没人能真正触发它。这里补上最小实现：原型阶段只关心"胜/负 + 击杀进度"。
    /// 做成普通 C# 类（非 MonoBehaviour / 非 ScriptableObject），纯数据载体，
    /// 谁想知道战斗结果就监听事件读它，与具体 UI / 管理器解耦。
    /// </summary>
    public class BattleResult
    {
        /// <summary>是否胜利（达成击杀目标为 true；玩家死亡为 false）</summary>
        public bool IsVictory { get; }

        /// <summary>本局最终击杀数</summary>
        public int Kills { get; }

        /// <summary>本局的击杀目标</summary>
        public int KillTarget { get; }

        public BattleResult(bool isVictory, int kills, int killTarget)
        {
            IsVictory = isVictory;
            Kills = kills;
            KillTarget = killTarget;
        }

        // TODO(GDD 情绪调和)：正式版战斗结算要携带安抚值 / 执念阈值 / 是否化解执念 /
        // 咖啡·酒 Build 分项贡献 / 救回的居民 NPC 等。这些随夜晚叙事与 Build 联动一起做，
        // 届时在此扩展字段即可，事件签名不变。
    }
}
