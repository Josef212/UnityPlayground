using System.Collections.Generic;

public class DataMap
{
    private Dictionary<string, object> m_data = new Dictionary<string, object>();

    // =================================

    public object GetObject(string name)
    {
        return m_data.ContainsKey(name) ? m_data[name] : null;
    }

    public int? GetInt(string name)
    {
        return (int?)GetObject(name);
    }

    public float? GetFloat(string name)
    {
        return (float?)GetObject(name);
    }

    public string GetString(string name)
    {
        var val = (string)GetObject(name);
        return val != null ? val : "";
    }

    public double? GetDouble(string name)
    {
        return (double?)GetObject(name);
    }

    public long? GetLong(string name)
    {
        return (long?)GetObject(name);
    }

    public DataMap GetDataMap(string name)
    {
        return (DataMap)GetObject(name);
    }

    // =================================

    public void SetObject(string name, object data)
    {
        m_data[name] = data;
    }

    public void SetInt(string name, int data)
    {
        m_data[name] = data;
    }

    public void SetFloat(string name, float data)
    {
        m_data[name] = data;
    }

    public void SetString(string name, string data)
    {
        m_data[name] = data;
    }

    public void SetDouble(string name, double data)
    {
        m_data[name] = data;
    }

    public void SetLong(string name, long data)
    {
        m_data[name] = data;
    }

    public void SetDataMap(string name, DataMap data)
    {
        m_data[name] = data;
    }
}
