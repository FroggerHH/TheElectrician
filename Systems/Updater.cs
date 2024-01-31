namespace TheElectrician.Systems;

public static class Updater
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
            PowerFlow.Update();
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

    private static IEnumerator UpdateEnumerator()
    {
        yield return new WaitForSeconds(Consts.tickTime);
        Update();
        GetPlugin().StartCoroutine(UpdateEnumerator());
    }
}