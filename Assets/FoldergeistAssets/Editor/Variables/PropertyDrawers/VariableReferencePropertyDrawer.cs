using FoldergeistAssets.Variables.Internal;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VariableReference), true)]
public class VariableReferencePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();

        EditorGUI.BeginProperty(position, label, property);
        SerializedProperty useConstant = property.FindPropertyRelative("_useConstant");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

        var rect = new Rect(position.position, Vector2.one * 20);

        if (EditorGUI.DropdownButton(rect, new GUIContent(GetTexture()), FocusType.Keyboard, new GUIStyle() { fixedWidth = 50, border = new RectOffset(1, 1, 1, 1) }))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Constant"), useConstant.boolValue, () => SetProperty(useConstant, true));
            menu.AddItem(new GUIContent("Variable"), !useConstant.boolValue, () => SetProperty(useConstant, false));

            menu.ShowAsContext();
        }

        position.position += Vector2.right * 20;
        position.size -= new Vector2(20, 0);

        if (useConstant.boolValue)
        {
            var constant = property.FindPropertyRelative("_constantValue");
            position.size -= new Vector2(18, 0);
            EditorGUI.PropertyField(position, constant, new GUIContent(""), true);
        }
        else 
        {
            var variable = property.FindPropertyRelative("_variabelValue");
            EditorGUI.PropertyField(position, variable, new GUIContent(""), true);
        }

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    private void SetProperty(SerializedProperty property, bool value)
    {
        if (property.boolValue != value)
        {
            property.boolValue = value;

            property.serializedObject.ApplyModifiedProperties();
        }
    }

    private Texture GetTexture()
    {
        return (Texture)EditorGUIUtility.Load("icons/d__Popup.png");
    }
}