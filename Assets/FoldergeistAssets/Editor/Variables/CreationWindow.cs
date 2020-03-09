using System;
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
            private string _typeString = string.Empty;
            private string _variableName = string.Empty;
            private string _referenceName = string.Empty;
            private string _readOnlyReferenceName = string.Empty;
            private int _selected;
            private int _typeIndex = -1;
            private Type _type;
            private string _path;
            private bool _isList;

            public static void ShowWindow(string path)
            {
                var window = GetWindow<CreationWindow>();
                window._path = path;
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.Contains("Editor") && !a.FullName.Contains("firstpass")).ToList();


                for (int i = 0; i < assemblies.Count; i++)
                {
                    var assemblyClasses = assemblies[i].GetTypes();

                    for (int t = 0; t < assemblyClasses.Length; t++)
                    {
                        if (!assemblyClasses[t].IsGenericType && !assemblyClasses[t].IsAbstract && !assemblyClasses[t].FullName.Contains('+') &&
                            !assemblyClasses[t].FullName.Contains("Attribute") && !assemblyClasses[t].FullName.Contains("Mono") && !assemblyClasses[t].FullName.Contains("Exception") &&
                            !assemblyClasses[t].FullName.Contains("GC") && !assemblyClasses[t].FullName.Contains("Empty") &&
                            !assemblyClasses[t].FullName.Contains("Internal") && !assemblyClasses[t].FullName.Contains("Type") && !assemblyClasses[t].FullName.Contains("NUnit"))
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
                _typeString = EditorGUILayout.TextField("On which type should this be made", _typeString);

                if (_typeIndex > -1)
                {
                    bool wasList = _isList;
                    _isList = EditorGUILayout.Toggle("List", _isList);
                    _variableName = EditorGUILayout.TextField("Name the variable class", _variableName);
                    _referenceName = EditorGUILayout.TextField("Name the reference class", _referenceName);
                    _readOnlyReferenceName = EditorGUILayout.TextField("Name the read only reference class", _readOnlyReferenceName);

                    if(wasList != _isList)
                    {
                        _variableName = $"{_type.Name}{(_isList ? "List" : "")}Variable";
                        _referenceName = $"{_type.Name}{(_isList ? "List" : "")}Reference";
                        _readOnlyReferenceName = $"ReadOnly{_type.Name}{(_isList ? "List" : "")}Reference";
                    }

                    if (_variableName != string.Empty && _referenceName != string.Empty && _readOnlyReferenceName != string.Empty &&
                        _variableName != _referenceName && _variableName != _readOnlyReferenceName && _referenceName != _readOnlyReferenceName)
                    {
                        if (!_availableTypesShortened.Contains(_variableName) && !_availableTypesShortened.Contains(_referenceName) &&
                            !_availableTypesShortened.Contains(_readOnlyReferenceName))
                        {
                            if (GUILayout.Button("Create scripts"))
                            {
                                using (StreamWriter writer = new StreamWriter($"{_path}/{_variableName}.cs"))
                                {
                                    writer.WriteLine("using UnityEngine;");
                                    writer.WriteLine("using FoldergeistAssets.Variables;");
                                    writer.WriteLine("");
                                    writer.WriteLine($"[CreateAssetMenu(fileName = \"{_variableName}\", menuName = \"FoldergeistAssets/Variables/{_variableName}\", order = 0)]");
                                    writer.WriteLine($"public sealed class {_variableName} : Variable<{(_isList ? "System.Collections.Generic.List<" : "")}" +
                                        $"{Type.GetType(_availableTypes[_typeIndex]).FullName}{(_isList ? ">" : "")}>");
                                    writer.WriteLine("{");
                                    writer.WriteLine("}");
                                }

                                using (StreamWriter writer = new StreamWriter($"{_path}/{_referenceName}.cs"))
                                {
                                    writer.WriteLine("using System;");
                                    writer.WriteLine("using FoldergeistAssets.Variables;");
                                    writer.WriteLine("");
                                    writer.WriteLine("[Serializable]");
                                    writer.WriteLine($"public sealed class {_referenceName} : VariableReference<{(_isList ? "System.Collections.Generic.List<" : "")}" +
                                        $"{Type.GetType(_availableTypes[_typeIndex]).FullName}{(_isList ? ">" : "")}, {_variableName}>");
                                    writer.WriteLine("{");
                                    writer.WriteLine("}");
                                }

                                using (StreamWriter writer = new StreamWriter($"{_path}/{_readOnlyReferenceName}.cs"))
                                {
                                    writer.WriteLine("using System;");
                                    writer.WriteLine("using FoldergeistAssets.Variables;");
                                    writer.WriteLine("");
                                    writer.WriteLine("[Serializable]");
                                    writer.WriteLine($"public sealed class {_readOnlyReferenceName} : ReadOnlyVariableReference<" +
                                        $"{(_isList ? "System.Collections.Generic.List<" : "")}" +
                                        $"{Type.GetType(_availableTypes[_typeIndex]).FullName}{(_isList ? ">" : "")}, {_variableName}>");
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
                        _type = Type.GetType(_availableTypes[_typeIndex]);

                        if (_type == null)
                        {
                            _typeIndex = -1;
                        }
                        else
                        {
                            _variableName = $"{_type.Name}{(_isList ? "List" : "")}Variable";
                            _referenceName = $"{_type.Name}{(_isList ? "List" : "")}Reference";
                            _readOnlyReferenceName = $"ReadOnly{_type.Name}{(_isList ? "List" : "")}Reference";
                        }
                    }
                }

                if (GUILayout.Button("Find matching type"))
                {
                    if (_typeString == string.Empty)
                    {
                        Debug.LogError("You need a type to find");
                    }
                    else
                    {
                        _typeIndex = -1;
                        _checkedAvailable = _availableTypesShortened.Where(s => s.ToLower().Contains(_typeString.ToLower())).ToList();
                    }
                }
            }
        }
    }
}