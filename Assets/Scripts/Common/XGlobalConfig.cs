public class XGlobalConfig : XSingleton<XGlobalConfig>
{

    public static readonly char[] SequenceSeparator = new char[] { '=' };
    public static readonly char[] ListSeparator = new char[] { '|' };
    public static readonly char[] AllSeparators = new char[] { '|', '=' };
    public static readonly char[] SpaceSeparator = new char[] { ' ' };
    public static readonly char[] TabSeparator = new char[] { ' ', '\t' };

}
