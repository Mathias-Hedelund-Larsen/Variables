using System.IO;
using System.Linq;
using UnityEditor;

namespace FoldergeistAssets
{
    namespace Variables
    {
        public class CreateVariableAndReference
        {
            [MenuItem("Assets/Create/FoldergeistAssets/Scripts/Variable and Reference", false, 0)]
            private static void OpenWindowToCreateVarRef()
            {
                var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

                if (Directory.Exists(path))
                {
                    CreationWindow.ShowWindow(path, false);
                }
                else
                {
                    var split = path.Split('/').ToList();

                    split.RemoveAt(split.Count - 1);

                    CreationWindow.ShowWindow(string.Join("/", split),false);
                }
            }

            [MenuItem("Assets/Create/FoldergeistAssets/Scripts/ReadOnly Variable and Reference", false, 0)]
            private static void OpenWindowToCreateReadOnlyVarRef()
            {
                var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

                if (Directory.Exists(path))
                {
                    CreationWindow.ShowWindow(path, true);
                }
                else
                {
                    var split = path.Split('/').ToList();

                    split.RemoveAt(split.Count - 1);

                    CreationWindow.ShowWindow(string.Join("/", split), true);
                }
            }
        }
    }
}