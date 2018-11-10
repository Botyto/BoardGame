public class Localization
{
    private static Localization s_Instance = null;
    public static Localization instance
    {
        get
        {
            return s_Instance ?? (s_Instance = new Localization());
        }
    }

    public static string T(string text)
    {
        return instance.RemapText(text);
    }

    public string RemapText(string text)
    {
        return text;
    }
}
