using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[AttributeUsage(AttributeTargets.Property)]
public class ExposePropertyAttribute : Attribute
{
}


public static class ExposeProperties
{
    public static void Expose(PropertyField[] properties)
    {
        GUILayoutOption[] emptyOptions = new GUILayoutOption[0];

        EditorGUILayout.BeginVertical(emptyOptions);

        foreach (PropertyField field in properties)
        {
            EditorGUILayout.BeginHorizontal(emptyOptions);

            switch (field.Type)
            {
                case SerializedPropertyType.Integer:
                    field.SetValue(EditorGUILayout.IntField(field.Name, (int)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.Float:
                    field.SetValue(EditorGUILayout.FloatField(field.Name, (float)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.Boolean:
                    field.SetValue(EditorGUILayout.Toggle(field.Name, (bool)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.String:
                    field.SetValue(EditorGUILayout.TextField(field.Name, (String)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.Vector2:
                    field.SetValue(EditorGUILayout.Vector2Field(field.Name, (Vector2)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.Vector3:
                    field.SetValue(EditorGUILayout.Vector3Field(field.Name, (Vector3)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.Enum:
                    field.SetValue(EditorGUILayout.EnumPopup(field.Name, (Enum)field.GetValue(), emptyOptions));
                    break;

                default:
                    break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    public static PropertyField[] GetProperties(System.Object obj)
    {
        List<PropertyField> fields = new List<PropertyField>();

        PropertyInfo[] infos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo info in infos)
        {

            if (!(info.CanRead && info.CanWrite))
                continue;

            object[] attributes = info.GetCustomAttributes(true);

            bool isExposed = false;

            foreach (object o in attributes)
            {
                if (o.GetType() == typeof(ExposePropertyAttribute))
                {
                    isExposed = true;
                    break;
                }
            }

            if (!isExposed)
                continue;

            SerializedPropertyType type = SerializedPropertyType.Integer;

            if (PropertyField.GetPropertyType(info, out type))
            {
                PropertyField field = new PropertyField(obj, info, type);
                fields.Add(field);
            }

        }

        return fields.ToArray();
    }
}
