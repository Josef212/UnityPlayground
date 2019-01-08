
public class Flag
{
    public Flag()
    {
        m_flag = 0;
    }

    public Flag(int initialFlag)
    {
        m_flag = initialFlag;
    }

    public void AddFlag(int flag)
    {
        m_flag |= flag;
    }

    public void RemoveFlag(int flag)
    {
        m_flag &= ~flag;
    }

    public bool HasFlag(int flag)
    {
        return (m_flag & flag) == flag;
    }

    public void Reset()
    {
        m_flag = 0;
    }

    private int m_flag;
}
