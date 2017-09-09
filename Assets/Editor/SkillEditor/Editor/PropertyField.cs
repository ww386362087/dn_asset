using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class PropertyField
{
    System.Object m_Instance;
    PropertyInfo m_Info;
    SerializedPropertyType m_Type;

    MethodInfo m_Getter;
    MethodInfo m_Setter;

    public SerializedPropertyType Type
    {
        get{ return m_Type; }
    }

    public String Name
    {
        get { return ObjectNames.NicifyVariableName(m_Info.Name); }
    }

    public PropertyField(System.Object instance, PropertyInfo info, SerializedPropertyType type)
    {
        m_Instance = instance;
        m_Info = info;
        m_Type = type;

        m_Getter = m_Info.GetGetMethod();
        m_Setter = m_Info.GetSetMethod();
    }

    public System.Object GetValue()
    {
        return m_Getter.Invoke(m_Instance, null);
    }

    public void SetValue(System.Object value)
    {
        m_Setter.Invoke(m_Instance, new System.Object[] { value });
    }

    public static bool GetPropertyType(PropertyInfo info, out SerializedPropertyType propertyType)
    {
        propertyType = SerializedPropertyType.Generic;

        Type type = info.PropertyType;

        if (type == typeof(int))
        {
            propertyType = SerializedPropertyType.Integer;
            return true;
        }

        if (type == typeof(float))
        {
            propertyType = SerializedPropertyType.Float;
            return true;
        }

        if (type == typeof(bool))
        {
            propertyType = SerializedPropertyType.Boolean;
            return true;
        }

        if (type == typeof(string))
        {
            propertyType = SerializedPropertyType.String;
            return true;
        }

        if (type == typeof(Vector2))
        {
            propertyType = SerializedPropertyType.Vector2;
            return true;
        }

        if (type == typeof(Vector3))
        {
            propertyType = SerializedPropertyType.Vector3;
            return true;
        }

        if (type.IsEnum)
        {
            propertyType = SerializedPropertyType.Enum;
            return true;
        }

        return false;
    }
}
