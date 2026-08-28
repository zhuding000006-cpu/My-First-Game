using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>显示事件、执行一次性结算，并切换到结果页面。</summary>
public class EventPanelView : MonoBehaviour
{
    [Header("事件与玩家")]
    [SerializeField] private EventData eventData;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private PlayerStatsView playerStatsView;

    [Header("三个界面面板")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("店铺入口")]
    [SerializeField] private Button enterEventButton;

    [Header("事件界面")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text bodyText;
    [SerializeField] private ScrollRect bodyScrollRect;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private Text[] optionLabels;

    [Header("结果界面")]
    [SerializeField] private Text resultChoiceText;
    [SerializeField] private Text resultStoryText;
    [SerializeField] private ScrollRect resultStoryScrollRect;
    [SerializeField] private Text attributeChangesText;

    private bool hasResolvedEvent;

    // 由“进入今日事件”按钮的 On Click 调用。
    public void ShowEvent()
    {
        if (!HasValidReferences()) return;

        if (hasResolvedEvent || playerState.AncientJadeResult != AncientJadeOutcome.None)
        {
            Debug.LogWarning("今日的古玉事件已经结束，不能再次进入。", this);
            return;
        }

        titleText.text = eventData.Title;
        bodyText.text = eventData.Body;
        for (int i = 0; i < optionLabels.Length; i++)
        {
            optionLabels[i].text = eventData.Options[i].OptionText;
            optionButtons[i].interactable = true;
        }

        shopPanel.SetActive(false);
        resultPanel.SetActive(false);
        eventPanel.SetActive(true);

        // 长正文允许滚动；每次打开事件时都从开头阅读。
        Canvas.ForceUpdateCanvases();
        bodyScrollRect.verticalNormalizedPosition = 1f;
    }

    // 三个按钮分别传入 0、1、2；数组下标从 0 开始。
    public void SelectOption(int optionIndex)
    {
        if (hasResolvedEvent || playerState.AncientJadeResult != AncientJadeOutcome.None)
        {
            Debug.LogWarning("本次事件已经结算，不能重复选择。", this);
            return;
        }

        if (!TryGetOption(optionIndex, out EventOptionData option)) return;

        // 先锁定互斥隐藏结果，再执行数值变化，避免一次点击被重复结算。
        if (!playerState.TrySetAncientJadeOutcome(option.AncientJadeOutcome))
        {
            Debug.LogWarning("本次事件已经记录过隐藏结果，不能重复结算。", this);
            return;
        }

        hasResolvedEvent = true;
        foreach (Button button in optionButtons) button.interactable = false;

        int oldMoney = playerState.Money;
        int oldHealth = playerState.Health;
        int oldReputation = playerState.Reputation;
        int oldGreed = playerState.Greed;

        playerState.ChangeMoney(option.MoneyChange);
        playerState.ChangeHealth(option.HealthChange);
        playerState.ChangeReputation(option.ReputationChange);
        playerState.ChangeGreed(option.GreedChange);

        resultChoiceText.text = option.OptionText;
        resultStoryText.text = option.ResultText;
        attributeChangesText.text = BuildAttributeChanges(
            oldMoney, oldHealth, oldReputation, oldGreed);

        eventPanel.SetActive(false);
        resultPanel.SetActive(true);

        Canvas.ForceUpdateCanvases();
        resultStoryScrollRect.verticalNormalizedPosition = 1f;
        Debug.Log($"事件《{eventData.Title}》已结算：{option.OptionText}", this);
    }

    // 由结果页“返回奇珍铺”按钮调用。
    public void ReturnToShop()
    {
        if (resultPanel == null || !resultPanel.activeInHierarchy) return;

        resultPanel.SetActive(false);
        eventPanel.SetActive(false);
        shopPanel.SetActive(true);

        // 明确刷新属性，并关闭本局已经完成的事件入口。
        playerStatsView.Refresh();
        enterEventButton.interactable = false;
        enterEventButton.GetComponentInChildren<Text>().text = "今日事件已结束";
    }

    private bool TryGetOption(int optionIndex, out EventOptionData option)
    {
        option = null;
        if (eventData == null || eventPanel == null || !eventPanel.activeInHierarchy) return false;

        if (eventData.Options == null || optionIndex < 0 || optionIndex >= eventData.Options.Length)
        {
            Debug.LogError("事件选项编号无效，请检查按钮的 On Click 参数。", this);
            return false;
        }

        option = eventData.Options[optionIndex];
        if (option == null)
        {
            Debug.LogError("事件选项资料为空，请检查 EventData。", this);
            return false;
        }
        return true;
    }

    private string BuildAttributeChanges(int oldMoney, int oldHealth, int oldReputation, int oldGreed)
    {
        var text = new StringBuilder();
        AppendChange(text, "财富", oldMoney, playerState.Money);
        AppendChange(text, "健康", oldHealth, playerState.Health);
        AppendChange(text, "声望", oldReputation, playerState.Reputation);
        AppendChange(text, "贪欲", oldGreed, playerState.Greed);
        return text.Length > 0 ? text.ToString().TrimEnd() : "属性没有变化";
    }

    private static void AppendChange(StringBuilder text, string label, int oldValue, int newValue)
    {
        // 只列出经过上下限处理后，实际发生变化的属性。
        if (oldValue != newValue)
        {
            text.AppendLine($"{label}：{oldValue} → {newValue}");
        }
    }

    private bool HasValidReferences()
    {
        if (eventData == null || playerState == null || playerStatsView == null
            || shopPanel == null || eventPanel == null || resultPanel == null
            || enterEventButton == null || titleText == null || bodyText == null
            || bodyScrollRect == null || resultChoiceText == null || resultStoryText == null
            || resultStoryScrollRect == null || attributeChangesText == null)
        {
            Debug.LogError("事件或结果面板缺少引用，请检查 EventPanelView 的 Inspector。", this);
            return false;
        }

        if (eventData.Options == null || optionButtons == null || optionLabels == null
            || eventData.Options.Length != 3 || optionButtons.Length != 3 || optionLabels.Length != 3)
        {
            Debug.LogError("当前事件界面需要三个选项，请检查资料、按钮和文本引用。", this);
            return false;
        }

        for (int i = 0; i < 3; i++)
        {
            if (eventData.Options[i] == null || optionButtons[i] == null || optionLabels[i] == null)
            {
                Debug.LogError("事件选项缺少资料或界面引用，请检查第 " + (i + 1) + " 项。", this);
                return false;
            }
        }
        return true;
    }
}
