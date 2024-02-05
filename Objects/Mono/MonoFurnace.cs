using TheElectrician.Models;
using TheElectrician.Objects.Consumers.Furnace;
using UnityEngine.UI;

namespace TheElectrician.Objects.Mono;

public class MonoFurnace : ElectricMono, Hoverable, Interactable
{
    private static readonly float HoldRepeatInterval = 0.2f;
    private GameObject enabledVisual;
    private float m_lastUseTime;
    private IFurnace furnace;

    private Canvas canvas;
    private TextMeshProUGUI progressText;
    private Image progressBarFilled;
    private Image recipeOutputImage;

    public override void SetUp()
    {
        base.SetUp();
        enabledVisual = transform.Find("EnabledVisual")?.gameObject;
        if (!enabledVisual)
        {
            enabledVisual = new GameObject("EnabledVisual");
            enabledVisual.transform.parent = transform;
            enabledVisual.transform.localPosition = Vector3.zero;

            DebugWarning($"Furnace {gameObject.GetPrefabName()} has no enabled visual");
        }

        enabledVisual.SetActive(false);
        canvas = transform.GetComponentInChildren<Canvas>(true);
        canvas?.gameObject.SetActive(false);
        var progressTextGO = canvas?.transform.FindChildByName("ProgressText")?.gameObject;
        if (progressTextGO is not null)
        {
            var old = progressTextGO.GetComponent<TextMeshProUGUI>();
            if (old) Destroy(old);
            var sign = ZNetScene.instance.GetPrefab("sign").GetComponent<Sign>();
            progressText = progressTextGO.AddComponent<TextMeshProUGUI>();
            progressText.font = sign.m_textWidget.font;
            progressText.fontSize = 75;
            progressText.text = "- 15 / 48 -";
            progressText.alignment = TextAlignmentOptions.Center;
            progressText.color = Color.white;
        }

        progressBarFilled = canvas?.transform.FindChildByName("ProgressBarFilled")?.GetComponent<Image>();
        recipeOutputImage = canvas?.transform.FindChildByName("RecipeOutputImage")?.GetComponent<Image>();
    }

    public override void Load()
    {
        if (!netView.IsValid()) return;
        furnace = Library.GetObject(netView.GetZDO()) as IFurnace;
        if (furnace is null) return;
        furnace.onProgressAdded.AddListener(OnProgressAdded);
        furnace.onProgressCompleted.AddListener(OnProgressCompleted);

        InvokeRepeating(nameof(UpdateVisual), 1.0f, 1.0f);
    }

    private void UpdateVisual()
    {
        enabledVisual.SetActive(furnace.IsInWorkingState());

        var recipe = furnace.GetCurrentRecipe();
        var progress = furnace.GetProgress();
        if (recipe == null)
        {
            if (canvas) canvas.gameObject.SetActive(false);
            if (progressText) progressText.text = string.Empty;
            if (progressBarFilled) progressBarFilled.fillAmount = 0;
            if (recipeOutputImage) recipeOutputImage.sprite = null;
            return;
        }

        if (canvas) canvas.gameObject.SetActive(true);
        if (progressText) progressText.text = $"{progress.start} / {progress.end}";
        if (progressBarFilled) progressBarFilled.fillAmount = progress.start / (float)progress.end;
        if (recipeOutputImage)
        {
            var outputItem = ObjectDB.instance.GetItem(recipe.output);
            if (outputItem != null) recipeOutputImage.sprite = outputItem.m_itemData.GetIcon();
        }
    }

    private void OnProgressCompleted()
    {
        //TODO: Some kind of animation or sfx
        UpdateVisual();
    }

    private void OnProgressAdded()
    {
        //TODO: Some kind of animation or sfx
        UpdateVisual();
    }

    public bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (furnace is null || !furnace.IsValid()) return false;
        if (MonoStorage.ConnectDisconnectWire(hold, alt, furnace)) return true;
        if (hold && (HoldRepeatInterval <= 0.0 || Time.time - m_lastUseTime < HoldRepeatInterval))
            return false;
        m_lastUseTime = Time.time;

        if (furnace.IsFull())
        {
            m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_itsfull");
            return false;
        }

        var inventory = user.GetInventory();
        ItemData item = null;
        foreach (var data in inventory.m_inventory)
        {
            if (data.m_dropPrefab == null) continue;
            if (!furnace.CanAdd(data.m_dropPrefab.name, 1)) continue;
            var recipe = FurnaceRecipe.GetRecipe(data.m_dropPrefab.name);
            if (recipe is null) continue;
            if (!recipe.CanProcess(furnace.GetLevel())) continue;

            item = data;
            break;
        }

        if (item == null) return false;

        return furnace.Add(item.m_dropPrefab.name, 1) && inventory.RemoveItem(item, 1);
    }

    public bool UseItem(Humanoid user, ItemData item)
    {
        if (furnace.IsFull())
        {
            m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_itsfull");
            return false;
        }

        var inventory = user.GetInventory();
        if (item.m_dropPrefab == null) return false;

        var recipe = FurnaceRecipe.GetRecipe(item.m_dropPrefab.name);
        if (recipe is null)
        {
            m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_wrongitem");
            return false;
        }

        if (!recipe.CanProcess(furnace.GetLevel()))
        {
            m_localPlayer.Message(MessageHud.MessageType.Center, $"${ModName}_furnace_low_level_for_recipe");
            return false;
        }

        return furnace.Add(item.m_dropPrefab.name, 1) && inventory.RemoveItem(item, 1);
    }

    public override string GetHoverText()
    {
        var sb = new StringBuilder();
        if (furnace == null || !furnace.IsValid()) return string.Empty;

        sb.AppendLine(piece.m_name.Localize());
        var level = furnace.GetLevel();
        if (m_debugMode)
        {
            sb.AppendLine($"ID: {furnace.GetId()}");
            sb.AppendLine($"Level: {level} ({$"${ModName}_level_{level}".Localize()})");
            var currentRecipe = furnace.GetCurrentRecipe();
            if (currentRecipe is not null)
                sb.AppendLine($"Current recipe: {currentRecipe}");
            sb.AppendLine($"Power: {furnace.GetPossiblePower()}");
        }

        sb.AppendLine();
        // sb.AppendLine($"${ModName}_level ${ModName}_level_{level}".Localize());
        if (!furnace.HaveEnoughPower())
            sb.AppendLine($"<color=#F448B2>${ModName}_furnace_low_power </color>".Localize());
        sb.AppendLine($"${ModName}_storage_capacity".Localize() + ": " + furnace.GetCapacity());
        if (furnace.IsInWorkingState())
        {
            sb.Append("<color=#F6E68B>");
            sb.AppendLine($"${ModName}_furnace_is_working".Localize());
            var progress = furnace.GetProgress();
            var recipe = furnace.GetCurrentRecipe();
            var inputItem = ObjectDB.instance.GetItem(recipe.input)?.LocalizeName() ?? "???";
            var outputItem = ObjectDB.instance.GetItem(recipe.output)?.LocalizeName() ?? "???";
            var outputCount = recipe.outputCount == 1 ? outputItem : string.Empty;
            var inputCount = recipe.inputCount == 1 ? inputItem : string.Empty;
            sb.AppendLine($"{inputCount} {inputItem} -> {outputCount} {outputItem} ({progress.start}/{progress.end})");
            sb.Append("</color>");
        } else
        {
            sb.AppendLine($"[<color=yellow><b>$KEY_Use</b></color>] ${ModName}_furnace_add".Localize());
            //TODO: Add compendium page
            sb.Append("<color=#F6C88B>");
            sb.AppendLine($"${ModName}_furnace_tip_open_compendium".Localize());
            sb.Append("</color>");
        }

        sb.AppendLine(MonoStorage.StoredText(furnace));
        return sb.ToString();
    }

    public override Guid GetId()
    {
        if (furnace == null || !furnace.IsValid()) return Guid.Empty;
        return furnace.GetId();
    }
}