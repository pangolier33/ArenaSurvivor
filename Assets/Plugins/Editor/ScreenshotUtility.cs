using UnityEditor;
using UnityEngine;

namespace Plugins.Editor
{
    public class ScreenshotUtility : MonoBehaviour
    {
        [MenuItem("Tools/Screenshot #F12")]
        public static void MakeScreenshot()
        {
            var name = $"screenshot_{Random.Range(100000,999999)}.png";
            ScreenCapture.CaptureScreenshot($"/../../{name}");
            Debug.Log($"скриншот {name} сохранен в {Application.dataPath}../{name}");
        }
    }
}
