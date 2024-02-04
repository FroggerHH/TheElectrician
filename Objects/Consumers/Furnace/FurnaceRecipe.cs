using TheElectrician.Models;

namespace TheElectrician.Objects.Consumers.Furnace;

[Serializable]
public class FurnaceRecipe(
    string input,
    string output,
    int minLevel,
    int ticksOnMinLevel = 2,
    int outputCount = 1,
    float power = 15,
    int inputCount = 1)
{
    private static HashSet<FurnaceRecipe> recipes =
    [
        new FurnaceRecipe("Wood", "Coal", 1, 2, 1, 10),
        new FurnaceRecipe("FineWood", "Coal", 1, 2, 2, 10),
        new FurnaceRecipe("CoreWood", "Coal", 1, 2, 3),

        new FurnaceRecipe("TinOre", "Tin", 1, 4, 1, 32),

        new FurnaceRecipe("CopperOre", "Copper", 2, 2, 1, 50),
        new FurnaceRecipe("CopperScrap", "Copper", 2, 2, 1, 50),

        new FurnaceRecipe("IronOre", "Iron", 3, 2, 1, 120),
        new FurnaceRecipe("IronScrap", "Iron", 3, 2, 1, 120),

        new FurnaceRecipe("SilverOre", "Silver", 4, 2, 1, 200),
        new FurnaceRecipe("SilverNecklace", "Silver", 4, 1, 1, 80, 2),

        new FurnaceRecipe("BlackMetalScrap", "BlackMetal", 5, 2, 1, 350),

        new FurnaceRecipe("FlametalOre", "Flametal", 6, 2, 1, 500)
    ];

    public string input = input;
    public string output = output;
    public int minLevel = minLevel;
    public int ticksOnMinLevel = ticksOnMinLevel;
    public int outputCount = outputCount;
    public int inputCount = inputCount;
    public float power = power;

    public static FurnaceRecipe GetRecipe(string input) => recipes.FirstOrDefault(x => x.input == input);
    public static List<FurnaceRecipe> GetAllRecipes(int level) => recipes.Where(x => x.minLevel <= level).ToList();

    public void CalculatePower() => power = ticksOnMinLevel * 1.0f / outputCount;


    public int CalculateTicks(int level)
    {
        //TODO: calculate ticks based on level using curve
        return ticksOnMinLevel;
    }

    public bool CanProcess(IFurnace furnace) => furnace.GetLevel() >= minLevel;

    public override string ToString() => $"{inputCount}{input} -> {outputCount}{output}";
}