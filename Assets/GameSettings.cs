using System;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class SettingAttribute : Attribute
{
    public object defaultValue;

    public SettingAttribute(object defaultValue)
    {
        this.defaultValue = defaultValue;
    }
}

[Serializable]
public class SettingsData
{
    [Setting(defaultValue: true)]
    public bool SkipThrow;

    [Setting(defaultValue: true)]
    public bool Shake;
}

public class GameSettings
{
    #region Properties
    
    public SettingsData data = new SettingsData();

    #endregion
    
    #region Operations

    private object GetValue(string name)
    {
        var field = GetFieldInfo(name);
        if (field == null) { return default(object); }

        var settingAttrib = field.GetCustomAttribute<SettingAttribute>();
        if (settingAttrib == null) { return default(object); }

        return field.GetValue(data);
    }

    public T GetValue<T>(string name)
    {
        ValidateFieldType<T>(name, "Getting");
        return (T)GetValue(name);
    }

    public static T Get<T>(string name)
    {
        return instance.GetValue<T>(name);
    }

    private void SetValue(string name, object value)
    {
        var field = GetFieldInfo(name);
        if (field == null) { return; }

        var settingAttrib = field.GetCustomAttribute<SettingAttribute>();
        if (settingAttrib == null) { return; }

        field.SetValue(data, value);

        Save();
    }

    public void SetValue<T>(string name, T value)
    {
        ValidateFieldType<T>(name, "Assigning");
        SetValue(name, (object)value);
    }

    public static void Set<T>(string name, T value)
    {
        instance.SetValue(name, value);
    }

    private object GetDefault(string name)
    {
        var field = GetFieldInfo(name);
        if (field == null) { return default(object); }

        var settingAttrib = field.GetCustomAttribute<SettingAttribute>();
        if (settingAttrib == null) { return default(object); }

        return settingAttrib.defaultValue;
    }

    public T GetDefault<T>(string name)
    {
        ValidateFieldType<T>(name, "Getting default value of ");
        var value = GetDefault(name);
        return (T)value;
    }

    public void Reset(string name)
    {
        Set(name, GetDefault(name));
    }

    public void ResetAll()
    {
        foreach (var field in data.GetType().GetFields())
        {
            var settingAttrib = field.GetCustomAttribute<SettingAttribute>();
            if (settingAttrib == null) { continue; }

            var name = field.Name;
            field.SetValue(data, GetDefault(name));
        }

        PlayerPrefs.DeleteAll();
    }

    #endregion

    #region Storage

    public void Load()
    {
        foreach (var field in data.GetType().GetFields())
        {
            var settingAttrib = field.GetCustomAttribute<SettingAttribute>();
            if (settingAttrib == null) { continue; }

            switch (Type.GetTypeCode(field.FieldType))
            {
                case TypeCode.Boolean: field.SetValue(data, PlayerPrefs.GetInt(field.Name, (bool)settingAttrib.defaultValue ? 1 : 0) != 0); break;
                case TypeCode.Int32: field.SetValue(data, PlayerPrefs.GetInt(field.Name, (int)settingAttrib.defaultValue)); break;
                case TypeCode.Single: field.SetValue(data, PlayerPrefs.GetFloat(field.Name, (float)settingAttrib.defaultValue)); break;
                case TypeCode.String: field.SetValue(data, PlayerPrefs.GetString(field.Name, (string)settingAttrib.defaultValue)); break;

                default: Debug.LogFormat("<color=red>[Warning]</color> Settings field of invalid type '{0}'", field.FieldType.Name); break;
            }
        }
    }

    public void Save()
    {
        foreach (var field in data.GetType().GetFields())
        {
            var settingAttrib = field.GetCustomAttribute<SettingAttribute>();
            if (settingAttrib == null) { continue; }

            var name = field.Name;

            switch (Type.GetTypeCode(field.FieldType))
            {
                case TypeCode.Boolean:
                    bool boolValue = (bool)field.GetValue(data);
                    if ((bool)settingAttrib.defaultValue == boolValue)
                    {
                        PlayerPrefs.DeleteKey(name);
                    }
                    else
                    {
                        PlayerPrefs.SetInt(name, boolValue ? 1 : 0);
                    }
                    break;

                case TypeCode.Int32:
                    int int32Value = (int)field.GetValue(data);
                    if ((int)settingAttrib.defaultValue == int32Value)
                    {
                        PlayerPrefs.DeleteKey(name);
                    }
                    else
                    {
                        PlayerPrefs.SetInt(name, int32Value);
                    }
                    break;

                case TypeCode.Single:
                    float singleValue = (float)field.GetValue(data);
                    if ((float)settingAttrib.defaultValue == singleValue)
                    {
                        PlayerPrefs.DeleteKey(name);
                    }
                    else
                    {
                        PlayerPrefs.SetFloat(name, singleValue);
                    }
                    break;

                case TypeCode.String:
                    string stringValue = (string)field.GetValue(data);
                    if ((string)settingAttrib.defaultValue == stringValue)
                    {
                        PlayerPrefs.DeleteKey(name);
                    }
                    else
                    {
                        PlayerPrefs.SetString(name, stringValue);
                    }
                    break;

                default:
                    Debug.LogFormat("<color=red>[Warning]</color> Settings field of invalid type '{0}'", field.FieldType.Name);
                    break;
            }
        }
    }

    #endregion

    #region Singleton & Internals

    private static GameSettings s_Instance = null;
    public static GameSettings instance
    {
        get
        {
            return s_Instance = s_Instance ?? new GameSettings();
        }
    }

    private GameSettings()
    {
        ValidateSettingTypes();
        Load();
    }

    private FieldInfo GetFieldInfo(string name)
    {
        return data.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public);
    }

    private void ValidateFieldType<T>(string name, string operation)
    {
#if UNITY_EDITOR
        var field = GetFieldInfo(name);
        if (field == null)
        {
            Debug.LogFormat("<color=red>[Warning]</color> Accessing missing setting '{0}'!", name);
            return;
        }

        var settingAttrib = field.GetCustomAttribute<SettingAttribute>();
        if (settingAttrib == null)
        {
            Debug.LogFormat("<color=red>[Warning]</color> Accessing non-setting property '{0}'!", name);
            return;
        }
        
        var ty = field.FieldType;
        var isValid = ty == typeof(T);
        Debug.AssertFormat(isValid, "<color=red>[Warning]</color> {0} '{1}' setting using invalid type '{2}'! Must be bool, int, float or string!", operation, field.Name, ty.Name);
#endif
    }

    private void ValidateSettingTypes()
    {
#if UNITY_EDITOR
        foreach (var field in data.GetType().GetFields())
        {
            var settingAttrib = field.GetCustomAttribute<SettingAttribute>();
            if (settingAttrib == null) { continue; }

            var ty = field.FieldType;
            var isValid = (ty == typeof(bool)) || (ty == typeof(int)) || (ty == typeof(float)) || (ty == typeof(string));
            Debug.AssertFormat(isValid, "<color=red>[Warning]</color> Setting '{0}' has an invalid type '{1}'! Must be bool, int, float or string!", field.Name, ty.Name);
        }
#endif
    }

    #endregion
}
