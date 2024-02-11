namespace TheElectrician.Objects.Consumers.Furnace;

[Serializable]
public class FurnaceRecipe
{
    private static HashSet<FurnaceRecipe> recipes = [];

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
        //TODO: calculate power based on level using curve
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