using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;

using TMPro;

namespace TownOfSushi.Utilities;

public static class UsefulMethods
{
    public static void ShowTextToast(string text, float delay = 1.25f)
    {
        HudManager.Instance.StartCoroutine(CoTextToast(text, delay));
    }
    private static IEnumerator CoTextToast(string text, float delay)
    {
        GameObject taskOverlay = Object.Instantiate(HudManager.Instance.TaskCompleteOverlay.gameObject, HudManager.Instance.transform);
        taskOverlay.SetActive(true);
        taskOverlay.GetComponentInChildren<TextTranslatorTMP>().DestroyImmediate();
        taskOverlay.GetComponentInChildren<TextMeshPro>().text = text;
            
        yield return Effects.Slide2D(taskOverlay.transform, new Vector2(0f, -8f), Vector2.zero, 0.25f);
            
        for (float time = 0f; time < delay; time += Time.deltaTime)
        {
            yield return null;
        }
            
        yield return Effects.Slide2D(taskOverlay.transform, Vector2.zero, new Vector2(0f, 8f), 0.25f);
            
        taskOverlay.SetActive(true);
        taskOverlay.Destroy();
    }
}