using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace EP.U3D.LIBRARY.I18N
{
    public class ProcI18N : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                if (path.StartsWith("Packages/")) return;
                if (path.EndsWith(".prefab")) CheckText(path);
            }
            foreach (string path in movedAssets)
            {
                if (path.StartsWith("Packages/")) return;
                if (path.EndsWith(".prefab")) CheckText(path);
            }
        }

        static void CheckText(string path)
        {
            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (asset)
            {
                Text[] texts = asset.GetComponentsInChildren<Text>(true);
                if (texts.Length > 0)
                {
                    for (int i = 0; i < texts.Length; i++)
                    {
                        Text text = texts[i];
                        I18NText i18 = text.GetComponent<I18NText>();
                        if (i18 == null)
                        {
                            text.gameObject.AddComponent<I18NText>();
                        }
                    }
                }
            }
        }
    }
}
#endif