using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FoldergeistAssets
{
    namespace Variables
    {
        public class CreationWindow : EditorWindow
        {
            private List<string> _checkedAvailable = new List<string>();
            private List<string> _availableTypes = new List<string>();
            private List<string> _availableTypesShortened = new List<string>();
            private string _type = string.Empty;
            private string _variableName = string.Empty;
            private string _referenceName = string.Empty;
            private int _selected;
            private int _typeIndex = -1;
            private string _path;
            private bool _readOnly;

            public static void ShowWindow(string path, bool readOnly)
            {
                var window = GetWindow<CreationWindow>();
                window._path = path;
                window._readOnly = readOnly;
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.Contains("Editor") && !a.FullName.Contains("firstpass")).ToList();


                for (int i = 0; i < assemblies.Count; i++)
                {
                    var assemblyClasses = assemblies[i].GetTypes();

                    for (int t = 0; t < assemblyClasses.Length; t++)
                    {
                        if (!assemblyClasses[t].IsGenericType && !assemblyClasses[t].IsAbstract && !assemblyClasses[t].FullName.Contains('+') &&
                            !assemblyClasses[t].FullName.Contains("Attribute") && !assemblyClasses[t].FullName.Contains("Mono") && !assemblyClasses[t].FullName.Contains("Exception") &&
                            !assemblyClasses[t].FullName.Contains("GC") && !assemblyClasses[t].FullName.Contains("Empty") &&
                            !assemblyClasses[t].FullName.Contains("Internal") && !assemblyClasses[t].FullName.Contains("Type"))
                        {
                            string[] display = assemblyClasses[t].ToString().Split('.');

                            window._availableTypesShortened.Add(display[display.Length - 1]);
                            window._availableTypes.Add($"{assemblyClasses[t].FullName}, {assemblies[i].FullName.Split(',')[0]}");
                        }
                    }
                }
            }

            private void OnGUI()
            {
                _type = EditorGUILayout.TextField("On which type should this be made", _type);

                if (_typeIndex > -1)
                {
                    _variableName = EditorGUILayout.TextField("Name the variable class", _variableName);
                    _referenceName = EditorGUILayout.TextField("Name the reference class", _referenceName);

                    if (_variableName != string.Empty && _referenceName != string.Empty && _variableName != _referenceName)
                    {
                        if (!_availableTypesShortened.Contains(_variableName) && !_availableTypesShortened.Contains(_referenceName))
                        {
                            if (GUILayout.Button("Create scripts"))
                            {
                                using (StreamWriter writer = new StreamWriter($"{_path}/{_variableName}.cs"))
                                {
                                    writer.WriteLine("using UnityEngine;");
                                    writer.WriteLine("using FoldergeistAssets.Variables;");
                                    writer.WriteLine("");
                                    writer.WriteLine($"[CreateAssetMenu(fileName = \"{_variableName}\", menuName = \"FoldergeistAssets/Variables/{_variableName}\", order = 0)]");
                                    writer.WriteLine($"public sealed class {_variableName} : {(_readOnly ? "ReadOnly" : "")}Variable<{Type.GetType(_availableTypes[_typeIndex]).FullName}>");
                                    writer.WriteLine("{");
                                    writer.WriteLine("}");
                                }

                                using (StreamWriter writer = new StreamWriter($"{_path}/{_referenceName}.cs"))
                                {
                                    writer.WriteLine("using System;");
                                    writer.WriteLine("using FoldergeistAssets.Variables;");
                                    writer.WriteLine("");
                                    writer.WriteLine("[Serializable]");
                                    writer.WriteLine($"public sealed class {_referenceName} : {(_readOnly ? "ReadOnly" : "")}VariableReference<{Type.GetType(_availableTypes[_typeIndex]).FullName}, {_variableName}>");
                                    writer.WriteLine("{");
                                    writer.WriteLine("}");
                                }

                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

                                Close();
                            }
                        }
                    }
                }

                if (_typeIndex < 0 && _checkedAvailable.Count > 0)
                {
                    _selected = EditorGUILayout.Popup(_selected, _checkedAvailable.ToArray());

                    if (GUILayout.Button("Check type"))
                    {
                        _typeIndex = _availableTypesShortened.IndexOf(_checkedAvailable[_selected]);
                        var type = Type.GetType(_availableTypes[_typeIndex]);

                        if (type == null)
                        {
                            _typeIndex = -1;
                        }
                        else
                        {
                            _variableName = $"{(_readOnly ? "ReadOnly" : "")}{type.Name}Variable";
                            _referenceName = $"{(_readOnly ? "ReadOnly" : "")}{type.Name}Reference";
                        }
                    }
                }

                if (GUILayout.Button("Find matching type"))
                {
                    if (_type == string.Empty)
                    {
                        Debug.LogError("You need a type to find");
                    }
                    else
                    {
                        _typeIndex = -1;
                        _checkedAvailable = _availableTypesShortened.Where(s => s.ToLower().Contains(_type.ToLower())).ToList();
                    }
                }
            }
        }
    }
}