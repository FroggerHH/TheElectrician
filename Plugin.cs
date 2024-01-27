using BepInEx;
using TheElectrician.Libs.LocalizationManager;
using TheElectrician.Libs.PieceManager;
using TheElectrician.Models.Settings;
using TheElectrician.Objects;

namespace TheElectrician;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string ModName = "TheElectrician",
        ModAuthor = "Frogger",
        ModVersion = "0.1.0",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion, ModGUID);
        OnConfigurationChanged += UpdateConfiguration;

        Localizer.Load();
        AddItems();
    }

    private void AddItems()
    {
        LoadAssetBundle("theelectrician");

        BuildPiece coalGenerator = new(bundle, "TE_coalGenerator");
        coalGenerator.Name.English("Coal Generator");
        coalGenerator.Name.Russian("Угольный генератор");
        coalGenerator.Description.English("Consumes coal to produce electricity");
        coalGenerator.Description.Russian("Потребляет уголь для создания электричества");
        coalGenerator.RequiredItems.Add("Coal", 5, false);
        coalGenerator.RequiredItems.Add("FineWood", 15, true);
        coalGenerator.RequiredItems.Add("SurtlingCore", 4, true);
        coalGenerator.Category.Set(BuildPieceCategory.Crafting);
        Library.Register("TE_coalGenerator", new GeneratorSettings(
            typeof(Generator), 150, 0.41f, "Coal", 1.5f, 100));
    }

    private void UpdateConfiguration() { Debug("Configuration Received"); }
}