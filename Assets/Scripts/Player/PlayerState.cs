using UnityEngine;

/// <summary>记录《财气冲天的古玉》的互斥结果。</summary>
public enum AncientJadeOutcome
{
    None,
    Bought,
    Investigated,
    Rejected
}

/// <summary>保存玩家属性，统一处理数值修改和范围限制。</summary>
public class PlayerState : MonoBehaviour
{
    [Header("测试用初始属性")]
    [SerializeField, Min(0)] private int money = 100;
    [SerializeField, Range(0, 100)] private int health = 80;
    [SerializeField, Range(0, 100)] private int reputation = 10;
    [SerializeField, Range(0, 100)] private int greed = 0;

    [Header("隐藏剧情状态（供调试查看）")]
    [SerializeField] private AncientJadeOutcome ancientJadeOutcome = AncientJadeOutcome.None;

    // 外部脚本可以读取属性，但修改时必须调用下面的方法。
    public int Money => money;
    public int Health => health;
    public int Reputation => reputation;
    public int Greed => greed;
    public bool BoughtAncientJade => ancientJadeOutcome == AncientJadeOutcome.Bought;
    public bool InvestigatedAncientJade => ancientJadeOutcome == AncientJadeOutcome.Investigated;
    public bool RejectedAncientJade => ancientJadeOutcome == AncientJadeOutcome.Rejected;
    public AncientJadeOutcome AncientJadeResult => ancientJadeOutcome;

    private void Awake()
    {
        ClampValues();
    }

    private void OnValidate()
    {
        // 在 Inspector 中手动输入数值时，也执行同样的范围限制。
        ClampValues();
    }

    public void ChangeMoney(int amount)
    {
        money = AddClamped(money, amount, int.MaxValue);
    }

    public void ChangeHealth(int amount)
    {
        health = AddClamped(health, amount, 100);
    }

    public void ChangeReputation(int amount)
    {
        reputation = AddClamped(reputation, amount, 100);
    }

    public void ChangeGreed(int amount)
    {
        greed = AddClamped(greed, amount, 100);
    }

    /// <summary>古玉事件只能记录一次结果；一个枚举值保证三个状态互斥。</summary>
    public bool TrySetAncientJadeOutcome(AncientJadeOutcome outcome)
    {
        if (outcome == AncientJadeOutcome.None || ancientJadeOutcome != AncientJadeOutcome.None)
        {
            return false;
        }

        ancientJadeOutcome = outcome;
        return true;
    }

    private void ClampValues()
    {
        money = Mathf.Max(0, money);
        health = Mathf.Clamp(health, 0, 100);
        reputation = Mathf.Clamp(reputation, 0, 100);
        greed = Mathf.Clamp(greed, 0, 100);
    }

    private static int AddClamped(int value, int amount, int maximum)
    {
        // 正数表示增加，负数表示减少。先用 long 相加，避免 int 加法溢出。
        long result = (long)value + amount;
        if (result < 0) return 0;
        if (result > maximum) return maximum;
        return (int)result;
    }

#if UNITY_EDITOR
    // 仅供编辑器验证，不在游戏界面添加测试按钮。
    [ContextMenu("测试/四项属性增加10（仅运行时）")]
    private void TestIncrease()
    {
        if (!Application.isPlaying) return;
        ChangeMoney(10);
        ChangeHealth(10);
        ChangeReputation(10);
        ChangeGreed(10);
    }

    [ContextMenu("测试/四项属性减少10（仅运行时）")]
    private void TestDecrease()
    {
        if (!Application.isPlaying) return;
        ChangeMoney(-10);
        ChangeHealth(-10);
        ChangeReputation(-10);
        ChangeGreed(-10);
    }
#endif
}
