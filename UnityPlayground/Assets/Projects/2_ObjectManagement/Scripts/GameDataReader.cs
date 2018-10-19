using System.IO;
using UnityEngine;

public class GameDataReader
{
    BinaryReader reader;

    // =============================================================

    public GameDataReader(BinaryReader reader)
    {
        this.reader = reader;
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
}
