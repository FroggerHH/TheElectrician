using TheElectrician.Objects.Mono;
using TheElectrician.Patch;

namespace TheElectrician.InGameDev;

[HarmonyPatch]
public static class TerminalCommands
{
    [HarmonyPatch(typeof(Terminal), nameof(Terminal.InitTerminal))] [HarmonyPostfix]
    private static void AddCommands()
    {
        _ = new ConsoleCommand("SetLevel",
            "Sets level of the hovering mechanism to the specified value. \nArgs:\n1. Target level - int, 2. Ignore max level - true/false",
            args =>
                RunCommand(args1 =>
                {
                    if (!IsAdmin) throw new ConsoleCommandException("You are not an admin on this server");
                    if (args1.Length < 2)
                        throw new ConsoleCommandException("First argument must be a level to set");
                    if (!int.TryParse(args1[1], out var level))
                        throw new ConsoleCommandException($"{args1[1]} is not an valid integer");
                    if (!bool.TryParse(args1[2], out var ignoreMaxLevel))
                        throw new ConsoleCommandException($"{args1[2]} is not an valid boolean");

                    var eo = HotKeys.GetHoveringLevelable();
                    if (eo is not null)
                    {
                        eo.SetLevel(level, ignoreMaxLevel);
                        ElectricMono.UpdateLevelText(eo, ElectricMono.GetAll().Find(x => x.GetId() == eo.GetId()));
                        args1.Context.AddString("Done!");
                    } else
                        throw new ConsoleCommandException("Hovering ElectricObject not found");
                }, args),
            true);
    }
}