using UnityEngine;

/// <summary>一个事件选项的数据：按钮文字、属性变化、结果剧情和隐藏结果。</summary>
[System.Serializable]
public class EventOptionData
{
    [SerializeField] private string optionText;
    [SerializeField, TextArea(10, 20)] private string resultText;

    [Header("属性变化")]
    [SerializeField] private int moneyChange;
    [SerializeField] private int healthChange;
    [SerializeField] private int reputationChange;
    [SerializeField] private int greedChange;

    [Header("古玉事件隐藏结果")]
    [SerializeField] private AncientJadeOutcome ancientJadeOutcome;

    public string OptionText => optionText;
    public string ResultText => resultText;
    public int MoneyChange => moneyChange;
    public int HealthChange => healthChange;
    public int ReputationChange => reputationChange;
    public int GreedChange => greedChange;
    public AncientJadeOutcome AncientJadeOutcome => ancientJadeOutcome;
}

/// <summary>一份事件资料，保存事件文字和三个选项的结算数据。</summary>
[CreateAssetMenu(fileName = "NewEvent", menuName = "万贯之后/事件资料")]
public class EventData : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField, TextArea(12, 24)] private string body;
    [SerializeField] private EventOptionData[] options = new EventOptionData[3];

    public string Title => title;
    public string Body => body;
    public EventOptionData[] Options => options;
}
