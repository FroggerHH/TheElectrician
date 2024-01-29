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
        PowerFlow.Update();
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