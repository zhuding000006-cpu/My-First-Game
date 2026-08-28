using UnityEngine;
using UnityEngine.UI;

/// <summary>把玩家属性显示到界面，不负责修改玩家数据。</summary>
public class PlayerStatsView : MonoBehaviour
{
    [Header("玩家数据")]
    [SerializeField] private PlayerState playerState;

    [Header("四项属性文本")]
    [SerializeField] private Text moneyText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text reputationText;
    [SerializeField] private Text greedText;

    private void Start()
    {
        // 引用缺失时给出明确提示，避免每帧重复报错。
        if (playerState == null || moneyText == null || healthText == null
            || reputationText == null || greedText == null)
        {
            Debug.LogError("属性面板缺少引用，请检查 PlayerStatsView 的五个引用栏。", this);
            enabled = false;
            return;
        }

        Refresh();
    }

    private void Update()
    {
        // 第一阶段只有四项文本：每帧读取，保持逻辑直观，修改后立即可见。
        Refresh();
    }

    // 其他界面返回店铺时可以主动刷新一次。
    public void Refresh()
    {
        moneyText.text = $"金钱：{playerState.Money}";
        healthText.text = $"健康：{playerState.Health}";
        reputationText.text = $"声望：{playerState.Reputation}";
        greedText.text = $"贪欲：{playerState.Greed}";
    }
}
