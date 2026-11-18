using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }
    public float displayTime = 4f;

    VisualElement healthBar;
    Label healthValue;
    Label repairCount;
    VisualElement dialogue;
    VisualElement startScreen;
    VisualElement winScreen;
    VisualElement loseScreen;
    float dialogueRemaining;
    IVisualElementScheduledItem resetHealthTint;

    static readonly Color FullHealthColor = new Color32(255, 215, 157, 255);
    static readonly Color DamageColor = new Color32(255, 125, 138, 255);
    static readonly Color HealColor = new Color32(164, 250, 193, 255);

    void Awake()
    {
        instance = this;
    }

    public void ConfigureScreens(Action start, Action restart)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        healthBar = root.Q("HealthBar");
        healthValue = root.Q<Label>("HealthValue");
        repairCount = root.Q<Label>("RepairCount");
        dialogue = root.Q("NPCDialogue");
        startScreen = root.Q("StartScreenContainer");
        winScreen = root.Q("WinScreenContainer");
        loseScreen = root.Q("LoseScreenContainer");

        dialogue.style.display = DisplayStyle.None;
        root.Q<Button>("StartScreenContainerButton").clicked += start;
        root.Q<Button>("WinScreenContainerButton").clicked += restart;
        root.Q<Button>("LoseScreenContainerButton").clicked += restart;
    }

    public void SetHealthValue(int current, int maximum, bool? tookDamage = null)
    {
        healthValue.text = $"{current} / {maximum}";
        healthBar.style.width = Length.Percent(100f * current / maximum);
        resetHealthTint?.Pause();
        healthValue.style.color = tookDamage.HasValue
            ? (tookDamage.Value ? DamageColor : HealColor)
            : FullHealthColor;
        if (tookDamage.HasValue)
        {
            resetHealthTint = healthValue.schedule.Execute(
                () => healthValue.style.color = FullHealthColor).StartingIn(450);
        }
    }

    public void SetRepairCount(int repaired, int total)
    {
        repairCount.text = $"Автомати {repaired} / {total}";
    }

    void Update()
    {
        if (dialogueRemaining <= 0f) return;

        dialogueRemaining -= Time.deltaTime;
        if (dialogueRemaining <= 0f) dialogue.style.display = DisplayStyle.None;
    }

    public void DisplayDialogue()
    {
        dialogue.style.display = DisplayStyle.Flex;
        dialogueRemaining = displayTime;
    }

    public void DisplayStartScreen(bool visible)
    {
        startScreen.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void DisplayResult(bool won)
    {
        dialogue.style.display = DisplayStyle.None;
        VisualElement result = won ? winScreen : loseScreen;
        result.style.display = DisplayStyle.Flex;
    }
}
