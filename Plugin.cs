using System.Diagnostics.CodeAnalysis;
using BepInEx;
using TheElectrician.InGameDev;
using TheElectrician.Libs.LocalizationManager;
using TheElectrician.Libs.PieceManager;
using TheElectrician.Objects.Consumers.Furnace;
using TheElectrician.Settings;

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
        AddFurnaceRecipes();
        AddBuildPieces();
    }

    private void Update() { HotKeys.Update(); }

    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    private void AddFurnaceRecipes()
    {
        new FurnaceRecipe("Wood", "Coal", 1, 2, 1, 10);
        new FurnaceRecipe("FineWood", "Coal", 1, 2, 2, 10);
        new FurnaceRecipe("CoreWood", "Coal", 1, 2, 3, 15);

        new FurnaceRecipe("TinOre", "Tin", 1, 4, 1, 30);

        new FurnaceRecipe("CopperOre", "Copper", 2, 2, 1, 50);
        new FurnaceRecipe("CopperScrap", "Copper", 2, 2, 1, 50);

        new FurnaceRecipe("IronOre", "Iron", 3, 2, 1, 120);
        new FurnaceRecipe("IronScrap", "Iron", 3, 2, 1, 120);

        new FurnaceRecipe("SilverOre", "Silver", 4, 2, 1, 200);
        new FurnaceRecipe("SilverNecklace", "Silver", 4, 1, 1, 80, 2);

        new FurnaceRecipe("BlackMetalScrap", "BlackMetal", 5, 2, 1, 350);

        new FurnaceRecipe("FlametalOre", "Flametal", 6, 2, 1, 500);
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
            typeof(Generator), 1, 1, 15, 2, 1.5f, 60, 150,
            6f, "Coal", 0.4f, 100));

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
            typeof(Furnace), 1, 1, 15, 1, 1.5f, 50, 150));

        BuildPiece woodWire = new(bundle, "TE_woodWire");
        woodWire.Name.English("Wire fastening");
        woodWire.Name.Russian("Крепление провода");
        woodWire.Description.English("Allows connecting objects with wires. Conducts 32 EU per tick.");
        woodWire.Description.Russian("Позволяет соединять объекты проводами. Передаёт 32 EU в тик.");
        woodWire.RequiredItems.Add("Wood", 1, true);
        woodWire.Category.Set("TheElectrician");
        Library.Register("TE_woodWire",
            new WireSettings(typeof(Wire), 0, 0, 32f, 5f, 3, 3));

        BuildPiece woodStorage = new(bundle, "TE_woodenStorage");
        woodStorage.Name.English("Wooden power storage");
        woodStorage.Name.Russian("Деревянное хранилище энергии");
        woodStorage.Description.English("Stores 1k EU");
        woodStorage.Description.Russian("Хранит 1к EU");
        woodStorage.RequiredItems.Add("Wood", 60, true);
        woodStorage.RequiredItems.Add("Resin", 35, true);
        woodStorage.Category.Set("TheElectrician");
        Library.Register("TE_woodenStorage",
            new StorageSettings(typeof(Storage), 0, 0, 32f, 10f, 3, 1.8f, 120, 0, [Consts.storagePowerKey]));

        BuildPiece tinPipe = new(bundle, "TE_tinPipe");
        tinPipe.Name.English("");
        tinPipe.Name.Russian("");
        tinPipe.Description.English("Allows connecting objects with wires. Conducts 32 EU per tick.");
        tinPipe.Description.Russian("Позволяет соединять объекты проводами. Передаёт 32 EU в тик.");
        tinPipe.RequiredItems.Add("Wood", 1, true);
        tinPipe.Category.Set("TheElectrician");
        Library.Register("TE_tinPipe",
            new ItemPipeSettings(typeof(ItemPipe), 2, 2, 32f, 2, 120, 1));
    }
}