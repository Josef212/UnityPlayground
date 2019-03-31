using System.Collections.Generic;
using System.Linq;

public class EnumUtils<T>
{
    public static IEnumerable<T> GetAllValues()
    {
        return System.Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static int GetNumberOfValues()
    {
        return System.Enum.GetNames(typeof(T)).Length;
    }

    public static T ParseString(string inString)
    {
        return (T)System.Enum.Parse(typeof(T), inString, true);
    }

    public static string ToString(T type)
    {
        return System.Enum.GetName(typeof(T), type);
    }

    public static void ForEach(System.Action<T> callback)
    {
        foreach(var x in System.Enum.GetValues(typeof(T)))
        {
            callback((T)x);
        }
    }
}
