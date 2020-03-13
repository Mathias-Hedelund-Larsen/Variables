using System.IO;
using System.Linq;
using UnityEditor;

namespace HephaestusForge
{
    namespace Variables
    {
        public class CreateVariableAndReference
        {
            [MenuItem("Assets/Create/HephaestusForge/Scripts/Variable and (ReadOnly)Reference", false, 0)]
            private static void OpenWindowToCreateVarRef()
            {
                var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

                if (Directory.Exists(path))
                {
                    CreationWindow.ShowWindow(path);
                }
                else
                {
                    var split = path.Split('/').ToList();

                    split.RemoveAt(split.Count - 1);

                    CreationWindow.ShowWindow(string.Join("/", split));
                }
            }
        }
    }
}