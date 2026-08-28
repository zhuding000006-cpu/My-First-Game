using UnityEngine;
using UnityEngine.UI;

/// <summary>只负责标题、开场和首次进入店铺；事件流程仍由原脚本处理。</summary>
public class GameFlowController : MonoBehaviour
{
    [Header("页面引用")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("开场与店铺界面")]
    [SerializeField] private ScrollRect introScrollRect;
    [SerializeField] private PlayerStatsView playerStatsView;

    private void Awake()
    {
        if (titlePanel == null || introPanel == null || shopPanel == null
            || eventPanel == null || resultPanel == null
            || introScrollRect == null || playerStatsView == null)
        {
            Debug.LogError("开场流程缺少引用，请检查 GameFlowController 的 Inspector。", this);
            enabled = false;
            return;
        }

        // 每次启动都从标题页开始，不修改任何玩家属性或事件状态。
        introPanel.SetActive(false);
        shopPanel.SetActive(false);
        eventPanel.SetActive(false);
        resultPanel.SetActive(false);
        titlePanel.SetActive(true);
    }

    // 绑定标题页的“开始游戏”按钮。
    public void StartGame()
    {
        if (!enabled || !titlePanel.activeInHierarchy) return;

        titlePanel.SetActive(false);
        introPanel.SetActive(true);
        Canvas.ForceUpdateCanvases();
        introScrollRect.verticalNormalizedPosition = 1f;
    }

    // 绑定开场页的“接下这间铺子”按钮。
    public void EnterShop()
    {
        if (!enabled || !introPanel.activeInHierarchy) return;

        introPanel.SetActive(false);
        shopPanel.SetActive(true);
        playerStatsView.Refresh();
    }
}
