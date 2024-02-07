using BepInEx;
using TheElectrician.Libs.LocalizationManager;
using TheElectrician.Libs.PieceManager;
using TheElectrician.Models.Settings;
using TheElectrician.Objects;
using TheElectrician.Objects.Consumers;
using TheElectrician.Objects.Consumers.Furnace;
using TheElectrician.Systems.Config;

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
        TheConfig.Init();
        OnConfigurationChanged += TheConfig.UpdateConfiguration;

        Localizer.Load();
        AddBuildPieces();
    }

    private void AddBuildPieces()
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
        coalGenerator.Category.Set("TheElectrician");
        Library.Register("TE_coalGenerator", new GeneratorSettings(
            typeof(Generator), 1, 1, 32, 15, 2, 150,
            1.5f, "Coal", 0.41f, 100));

        BuildPiece stoneFurnace = new(bundle, "TE_stoneFurnace");
        stoneFurnace.Name.English("Electric Furnace");
        stoneFurnace.Name.Russian("Электрическая печь");
        stoneFurnace.Description.English("Uses electricity to melt ore and make coal");
        stoneFurnace.Description.Russian("Использует электричество для плавки руды и изготовления угля");
        stoneFurnace.RequiredItems.Add("Stone", 40, false);
        stoneFurnace.RequiredItems.Add("FineWood", 5, true);
        stoneFurnace.RequiredItems.Add("RoundLog", 30, true);
        stoneFurnace.RequiredItems.Add("SurtlingCore", 20, true);
        stoneFurnace.Category.Set("TheElectrician");
        Library.Register("TE_stoneFurnace", new FurnaceSettings(
            typeof(Furnace), 1, 1, 32, 15, 2, 150));

        BuildPiece woodWire = new(bundle, "TE_woodWire");
        woodWire.Name.English("Wire fastening");
        woodWire.Name.Russian("Крепление провода");
        woodWire.Description.English("Allows connecting objects with wires. Conducts 32 EU per tick.");
        woodWire.Description.Russian("Позволяет соединять объекты проводами. Передаёт 32 EU в тик.");
        woodWire.RequiredItems.Add("Wood", 1, true);
        woodWire.Category.Set("TheElectrician");
        Library.Register("TE_woodWire",
            new WireSettings(typeof(Wire), 0, 0, 32f, 5f, 4));

        BuildPiece woodStorage = new(bundle, "TE_woodenStorage");
        woodStorage.Name.English("Wooden power storage");
        woodStorage.Name.Russian("Деревянное хранилище энергии");
        woodStorage.Description.English("Stores 1k EU");
        woodStorage.Description.Russian("Хранит 1к EU");
        woodStorage.RequiredItems.Add("Wood", 60, true);
        woodStorage.RequiredItems.Add("Resin", 35, true);
        woodStorage.Category.Set("TheElectrician");
        Library.Register("TE_woodenStorage",
            new StorageSettings(typeof(Storage), 0, 0, 32f, 10f, 3, 100, [Consts.storagePowerKey]));
    }
}