using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BaseFramework
{
#if UNITY_EDITOR
    /// <summary>
    /// Code Originally From ChemiKhazi:
    /// https://gist.github.com/ChemiKhazi/11395776
    /// Code Forked From FFouetil
    /// https://gist.github.com/FFouetil/dd081256da0e3475d524d88b414076e3
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
            Enum targetEnum = (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue);

            string propName = flagSettings.enumName;
            if (string.IsNullOrEmpty(propName))
                propName = ObjectNames.NicifyVariableName(property.name);

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(position, label, property);
#if UNITY_2017_3_OR_NEWER
            Enum enumNew = EditorGUI.EnumFlagsField(position, propName, targetEnum);
#else
		Enum enumNew = EditorGUI.EnumMaskField(position, propName, targetEnum);
#endif
            if (!property.hasMultipleDifferentValues || EditorGUI.EndChangeCheck())
                property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());

            EditorGUI.EndProperty();
        }
    }
#endif
}
