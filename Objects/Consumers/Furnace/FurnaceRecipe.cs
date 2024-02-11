namespace TheElectrician.Objects.Consumers.Furnace;

[Serializable]
public class FurnaceRecipe
{
    static FurnaceRecipe()
    {
        recipes = [];
        
        new FurnaceRecipe("Wood", "Coal", 1, 2, 1, 10);
        new FurnaceRecipe("FineWood", "Coal", 1, 2, 2, 10);
        new FurnaceRecipe("CoreWood", "Coal", 1, 2, 3);

        new FurnaceRecipe("TinOre", "Tin", 1, 4, 1, 32);

        new FurnaceRecipe("CopperOre", "Copper", 2, 2, 1, 50);
        new FurnaceRecipe("CopperScrap", "Copper", 2, 2, 1, 50);

        new FurnaceRecipe("IronOre", "Iron", 3, 2, 1, 120);
        new FurnaceRecipe("IronScrap", "Iron", 3, 2, 1, 120);

        new FurnaceRecipe("SilverOre", "Silver", 4, 2, 1, 200);
        new FurnaceRecipe("SilverNecklace", "Silver", 4, 1, 1, 80, 2);

        new FurnaceRecipe("BlackMetalScrap", "BlackMetal", 5, 2, 1, 350);

        new FurnaceRecipe("FlametalOre", "Flametal", 6, 2, 1, 500);
    }

    private static HashSet<FurnaceRecipe> recipes = null;

    public string input;
    public string output;
    public int minLevel;
    public int ticksOnMinLevel;
    public int outputCount;
    public int inputCount;
    public float power;

    public FurnaceRecipe(string input,
        string output,
        int minLevel,
        int ticksOnMinLevel = 2,
        int outputCount = 1,
        float power = 15,
        int inputCount = 1)
    {
        this.input = input;
        this.output = output;
        this.minLevel = minLevel;
        this.ticksOnMinLevel = ticksOnMinLevel;
        this.outputCount = outputCount;
        this.inputCount = inputCount;
        this.power = power;

        if (recipes.Any(x => x.input == input))
            throw new Exception($"Duplicate recipe: {input} -> {output}");

        recipes.Add(this);
    }

    public static FurnaceRecipe GetRecipe(string input) => recipes.FirstOrDefault(x => x.input == input);
    public static List<FurnaceRecipe> GetAllRecipes(int level) => recipes.Where(x => x.minLevel <= level).ToList();

    public float CalculatePower(int level)
    {
        //TODO: calculate power based on level using curved
        return power;
    }


    public int CalculateTicks(int level)
    {
        //TODO: calculate ticks based on level using curve
        return ticksOnMinLevel;
    }

    public bool CanProcess(int level) => level >= minLevel;

    public override string ToString() => $"{inputCount}{input} -> {outputCount}{output}";
}