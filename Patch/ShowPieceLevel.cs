using System.Text.RegularExpressions;
using TheElectrician.Models.Settings;
using UnityEngine.SceneManagement;

namespace TheElectrician.Patch;

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake)), UsedImplicitly, HarmonyWrapSafe]
file static class ShowPieceLevel
{
    private static readonly string pattern = " <color=(.*?)>[.*?]</color>";
    private static readonly string addPattern = $" <color={{0}}>[${ModName}_level ${ModName}_level_{{1}}]</color>";
    private static readonly string color = "yellow";

    [UsedImplicitly, HarmonyPostfix]
    private static void Postfix()
    {
        if (SceneManager.GetActiveScene().name != "main") return;

        var pieces = ZNetScene.instance.GetPrefabs(Library.GetAllSettings().Keys.ToArray())
            .Select(x => x?.GetComponent<Piece>()).Where(x => x != null).ToList();
        foreach (var piece in pieces)
        {
            if (!Library.IsEO(piece)) continue;
            var settings = Library.GetSettings(piece.GetPrefabName()) as LevelableSettings;
            if (settings == null) continue;
            var level = settings.startLevel;
            piece.m_name = Regex.Replace(piece.m_name, pattern, "");
            var format = string.Format(addPattern, color, level);
            // Debug($"Adding {format} to {piece.m_name}");
            piece.m_name += format;
        }
    }
}