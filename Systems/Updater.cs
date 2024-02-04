using System.Diagnostics.CodeAnalysis;
using TheElectrician.Systems.Config;

namespace TheElectrician.Systems;

internal static class Updater
{
    public static void Start()
    {
        Debug("Updater: Starting update");
        GetPlugin().StartCoroutine(UpdateEnumerator());
    }

    public static void Destroy()
    {
        Debug("Updater: Stopping update");
        GetPlugin().StopCoroutine(UpdateEnumerator());
    }

    private static void Update()
    {
        try
        {
            UpdateObjects();
        }
        catch (Exception e)
        {
            DebugError($"Updater error: {e}");
        }
    }

    private static void UpdateObjects()
    {
        var enumerable = Library.GetAllObjects();
        foreach (var obj in enumerable) obj.Update();
    }

    [SuppressMessage("ReSharper", "FunctionRecursiveOnAllPaths")]
    private static IEnumerator UpdateEnumerator()
    {
        yield return new WaitForSeconds(TheConfig.ObjectTickTime);
        Update();
        GetPlugin().StartCoroutine(UpdateEnumerator());
    }
}