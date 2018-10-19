using System.IO;
using UnityEngine;

public class GameDataReader
{
    BinaryReader reader;

    public int Version { get; private set; }

    // =============================================================

    public GameDataReader(BinaryReader reader, int version)
    {
        this.reader = reader;
        this.Version = version;
    }

    public float ReadFloat()
    {
        return reader.ReadSingle();
    }

    public int ReadInt()
    {
        return reader.ReadInt32();
    }

    public Vector3 ReadVector3()
    {
        Vector3 ret;

        ret.x = reader.ReadSingle();
        ret.y = reader.ReadSingle();
        ret.z = reader.ReadSingle();

        return ret;
    }

    public Quaternion ReadQuaternion()
    {
        Quaternion ret;

        ret.x = reader.ReadSingle();
        ret.y = reader.ReadSingle();
        ret.z = reader.ReadSingle();
        ret.w = reader.ReadSingle();

        return ret;
    }

    public Color ReadColor()
    {
        Color c;

        c.r = reader.ReadSingle();
        c.g = reader.ReadSingle();
        c.b = reader.ReadSingle();
        c.a = reader.ReadSingle();

        return c;
    }
}
