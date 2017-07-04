using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public abstract class CVSReader
{
    public abstract class ValueParse<T>
    {
        public abstract void Read(BinaryReader stream, ref T t);

        public abstract int ReadBuffer(BinaryReader stream);
        public abstract void SkipBuffer(BinaryReader stream, int count);
    }

    public virtual string bytePath { get { return string.Empty; } }

    
    public void Create()
    {
        string path = Application.dataPath + "/Resources/" + bytePath + ".bytes";
        FileStream fs=new FileStream(path,FileMode.Open);
        ReadFile(fs);
        fs.Close();
    }

    public sealed class UIntParse : ValueParse<uint>
    {
        public override void Read(BinaryReader stream, ref uint t)
        {
            t = stream.ReadUInt32();
        }

        public override int ReadBuffer(BinaryReader stream)
        {
            ReAllocBuff(ref uintBuffer, uintIndex);
            uintBuffer[uintIndex] = stream.ReadUInt32();
            return uintIndex++;
        }
        public override void SkipBuffer(BinaryReader stream, int count)
        {
            stream.BaseStream.Seek(sizeof(uint) * count, SeekOrigin.Current);
        }
    }
    public sealed class IntParse : ValueParse<int>
    {
        public override void Read(BinaryReader stream, ref int t)
        {
            t = stream.ReadInt32();
        }

        public override int ReadBuffer(BinaryReader stream)
        {
            ReAllocBuff(ref intBuffer, intIndex);
            intBuffer[intIndex] = stream.ReadInt32();
            return intIndex++;
        }
        public override void SkipBuffer(BinaryReader stream, int count)
        {
            stream.BaseStream.Seek(sizeof(int) * count, SeekOrigin.Current);
        }
    }
    public sealed class LongParse : ValueParse<long>
    {
        public override void Read(BinaryReader stream, ref long t)
        {
            t = stream.ReadInt64();
        }

        public override int ReadBuffer(BinaryReader stream)
        {
            ReAllocBuff(ref longBuffer, longIndex);
            longBuffer[longIndex] = stream.ReadInt64();
            return longIndex++;
        }
        public override void SkipBuffer(BinaryReader stream, int count)
        {
            stream.BaseStream.Seek(sizeof(long) * count, SeekOrigin.Current);
        }
    }
    public sealed class FloatParse : ValueParse<float>
    {
        public override void Read(BinaryReader stream, ref float t)
        {
            t = stream.ReadSingle();
        }

        public override int ReadBuffer(BinaryReader stream)
        {
            ReAllocBuff(ref floatBuffer, floatIndex);
            float v = stream.ReadSingle();
            floatBuffer[floatIndex] = v;
            return floatIndex++;
        }
        public override void SkipBuffer(BinaryReader stream, int count)
        {
            stream.BaseStream.Seek(sizeof(float) * count, SeekOrigin.Current);
        }
    }

    public sealed class StringParse : ValueParse<string>
    {
        public override void Read(BinaryReader stream, ref string t)
        {
            t = string.Intern(stream.ReadString());
        }

        public override int ReadBuffer(BinaryReader stream)
        {
            ReAllocBuff(ref stringBuffer, stringIndex);
            stringBuffer[stringIndex] = string.Intern(stream.ReadString());
            return stringIndex++;
        }

        public override void SkipBuffer(BinaryReader stream, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                stream.ReadString();
            }
        }
    }

    public sealed class BoolParse : ValueParse<bool>
    {
        public override void Read(BinaryReader stream, ref bool t)
        {
            t = stream.ReadBoolean();
        }

        public override int ReadBuffer(BinaryReader stream)
        {
            return 0;
        }

        public override void SkipBuffer(BinaryReader stream, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                stream.ReadBoolean();
            }
        }
    }
    public struct SeqCache
    {
        public int valueIndex;
        public int indexIndex;
    }

    public static Dictionary<uint, SeqCache> _seqIndex = null;

    public static float[] floatBuffer;
    public static int floatIndex = 0;
    public static uint[] uintBuffer;
    public static int uintIndex = 0;
    public static int[] intBuffer;
    public static int intIndex = 0;
    public static long[] longBuffer;
    public static int longIndex = 0;
    public static double[] doubleBuffer;
    public static int doubleIndex = 0;
    public static string[] stringBuffer;
    public static int stringIndex = 0;

    public static int[] indexBuffer;
    public static int indexIndex = 0;

    protected static FloatParse floatParse = new FloatParse();
    protected static UIntParse uintParse = new UIntParse();
    protected static IntParse intParse = new IntParse();
    protected static LongParse longParse = new LongParse();
    protected static StringParse stringParse = new StringParse();
    protected static BoolParse boolParse = new BoolParse();

    public int lineno = -1;
    public int columnno = -1;
    public string error
    {
        get { return " line: " + lineno.ToString() + " column: " + columnno.ToString(); }
    }

    public static bool IsInited()
    {
        return _seqIndex != null;
    }
    public static void Init()
    {
        Uninit();
        if (_seqIndex == null)
            _seqIndex = new Dictionary<uint, SeqCache>();
        InitBuff<float>(ref floatBuffer, ref floatIndex, 4, 0.0f);
        InitBuff<uint>(ref uintBuffer, ref uintIndex, 16, 0);
        InitBuff<int>(ref intBuffer, ref intIndex, 2, 0);
        InitBuff<long>(ref longBuffer, ref longIndex, 1, 0);
        InitBuff<double>(ref doubleBuffer, ref doubleIndex, 0.1f, 0.0);
        InitBuff<string>(ref stringBuffer, ref stringIndex, 1, "");
        indexIndex = 0;
        indexBuffer = new int[1024 * 512];
        for (int i = 0, imax = indexBuffer.Length; i < imax; ++i)
        {
            indexBuffer[i] = 0;
        }
        indexBuffer[indexIndex++] = 0;
    }

    public static void Uninit()
    {
        if (_seqIndex != null)
        {
            _seqIndex.Clear();
            _seqIndex = null;
            GC.Collect();
        }
    }

    private static void ReAllocBuff<T>(ref T[] buffer, int index)
    {
        if (index >= buffer.Length)
        {
            T[] newBuff = new T[buffer.Length + buffer.Length / 2];
            Array.Copy(buffer, 0, newBuff, 0, buffer.Length);
            buffer = newBuff;
            Seq2Ref<T>.buffRef = buffer;
            Seq2ListRef<T>.buffRef = buffer;
        }
    }

    private static void InitBuff<T>(ref T[] buffer, ref int index, float size, T defalutValue)
    {
        buffer = new T[(int)(1024 * size)];
        for (int i = 0, imax = buffer.Length; i < imax; ++i)
        {
            buffer[i] = defalutValue;
        }
        index = 6;
        Seq2Ref<T>.buffRef = buffer;
        Seq2ListRef<T>.buffRef = buffer;
    }

    private static string LookupInterString(uint hash, string value)
    {
        return string.Intern(value);
    }

    public bool ReadFile(Stream stream)
    {
        lineno = 0;
        columnno = -1;
        BinaryReader reader = new BinaryReader(stream);
        {
            long fileSize = reader.ReadInt64();
            int lineCount = reader.ReadInt32();
            OnClear(lineCount);
            for (int i = 0; i < lineCount; ++i)
            {
                ReadLine(reader);
                ++lineno;
                if (columnno > 0) break;
            }
            long pos = reader.BaseStream.Position;
            if (pos != fileSize)
            {
                Debug.LogError("read table error: " + this.GetType().Name + " size:" + fileSize + " pos:" + pos + " stream: " + stream.Length);
            }
        }
        reader.Close();
        return columnno == -1;
    }


    private int AddIndexBuffer(byte count)
    {
        int currentIndex = indexIndex;
        for (byte i = 0; i < count; ++i)
        {
            indexBuffer[indexIndex++] = 0;
        }
        return currentIndex;
    }

    protected bool Read<T>(BinaryReader stream, ref T v, ValueParse<T> parse)
    {
        parse.Read(stream, ref v);
        return true;
    }

    protected bool ReadArray<T>(BinaryReader stream, ref T[] v, ValueParse<T> parse)
    {
        byte length = stream.ReadByte();
        if (length > 0)
        {
            v = new T[length];
            for (byte i = 0; i < length; ++i)
            {
                parse.Read(stream, ref v[i]);
            }
        }
        else
        {
            v = null;
        }
        return true;
    }

    protected bool Parse<T>(BinaryReader stream, ValueParse<T> parse, byte count, ref int iIndex, ref int vIndex)
    {
        bool newSeq = false;
        uint hash = stream.ReadUInt32();
        if (hash == 0)
        {
            iIndex = 0;
            vIndex = 0;
        }
        else
        {
            SeqCache sc;
            if (_seqIndex.TryGetValue(hash, out sc))
            {
                parse.SkipBuffer(stream, count);
            }
            else
            {
                sc.valueIndex = parse.ReadBuffer(stream);
                for (byte i = 1; i < count; ++i)
                {
                    parse.ReadBuffer(stream);
                }
                sc.indexIndex = iIndex;
                _seqIndex[hash] = sc;
                newSeq = true;
            }
            iIndex = sc.indexIndex;
            vIndex = sc.valueIndex;
        }
        return newSeq;
    }

    protected bool ReadSeq<T>(BinaryReader stream, ref Seq2Ref<T> v, ValueParse<T> parse)
    {
        lock (_seqIndex)
        {
            int iIndex = indexIndex;
            int vIndex = 0;
            bool newSeq = Parse<T>(stream, parse, 2, ref iIndex, ref vIndex);
            v.indexOffset = iIndex;
            if (newSeq)
                indexBuffer[indexIndex++] = vIndex;
        }
        return true;
    }

    protected bool ReadSeqList<T>(BinaryReader stream, ref Seq2ListRef<T> v, ValueParse<T> parse)
    {
        v.count = stream.ReadByte();
        if (v.count > 0)
        {
            lock (_seqIndex)
            {
                v.indexOffset = AddIndexBuffer(v.count);
                for (byte i = 0; i < v.count; ++i)
                {
                    //优化，如果是新分配的seq，可以使用已经分配的index
                    int iIndex = v.indexOffset + i;
                    int vIndex = 0;
                    Parse<T>(stream, parse, 2, ref iIndex, ref vIndex);
                    indexBuffer[v.indexOffset + i] = vIndex;
                }
            }
        }
        else
        {
            v.indexOffset = 0;
        }
        return true;
    }

    
    public abstract void OnClear(int lineCount);
    public virtual void ReadLine(BinaryReader reader) { }
}

    public interface ISeqListRef<T>
    {
        T this[int index, int key] { get; }
        int Count { get; }
        int Dim { get; }
    }

    public interface ISeqRef<T>
    {
        T this[int key] { get; }
        int Dim { get; }
    }

    public struct Seq2Ref<T> : ISeqRef<T>
    {
        public static T[] buffRef;
        public int indexOffset;
        public T this[int key]
        {
            get
            {
                int offset = CVSReader.indexBuffer[indexOffset];
                return buffRef[offset + key];
            }
        }

        public int Dim
        {
            get { return 2; }
        }

        public override string ToString()
        {
            int offset = CVSReader.indexBuffer[indexOffset];
            return string.Format("{0}={1}", buffRef[offset], buffRef[offset + 1]);
        }
    }
    public struct Seq2<T>
    {
        public T value0;
        public T value1;
        public Seq2(T v0, T v1)
        {
            value0 = v0;
            value1 = v1;
        }
    }

public struct Seq2ListRef<T> : ISeqListRef<T>
{
    public static T[] buffRef;
    public byte count;
    public int indexOffset;//指向索引数组的起始偏移
    public int Count
    {
        get { return count; }
    }

    public int Dim
    {
        get { return 2; }
    }

    public T this[int index, int key]
    {
        get
        {
            //有多少个count，在索引数组中就有多少项，根据起始便宜和传入的index获取实际value数组中的偏移
            int offset = CVSReader.indexBuffer[indexOffset + index];
            //根据value数组的偏移+key第几项获取实际值
            return buffRef[offset + key];
        }
    }

    public object Get(int key, int index)
    {
        int offset = CVSReader.indexBuffer[indexOffset + index];
        return buffRef[offset + key];
    }

    public override string ToString()
    {
        string str = "";
        for (int i = 0; i < count; ++i)
        {
            int offset = CVSReader.indexBuffer[indexOffset + i];
            if (i == count - 1)
            {
                str += string.Format("{0}={1}", buffRef[offset], buffRef[offset + 1]);
            }
            else
            {
                str += string.Format("{0}={1}|", buffRef[offset], buffRef[offset + 1]);
            }
        }
        return str;
    }
}


public class SeqList<T>
{
    public List<T> buff;
    private short m_dim = 2;
    private short m_count = 1;
    public SeqList()
    {
        buff = new List<T>();
        Reset(2, 1);
    }

    public SeqList(short dim, short count)
    {
        buff = new List<T>();
        Reset(dim, count);
    }

    public short Count
    {
        get { return m_count; }
    }

    public short Dim
    {
        get { return m_dim; }
    }
    public T this[int index, int dim]
    {
        get
        {
            return buff[index * this.m_dim + dim];
        }
        set
        {
            buff[index * this.m_dim + dim] = value;
        }
    }

    public T Get(int key, int dim)
    {
        return this[key, dim];
    }

    public void Reset(short dim, short count)
    {
        m_dim = dim;
        m_count = count;
        buff.Clear();
        buff.Capacity = m_dim * m_count;
        for (int i = 0; i < buff.Capacity; ++i)
        {
            buff.Add(default(T));
        }
    }
}

[Serializable]
public class TableScriptMap
{
    [SerializeField]
    public string table = "";
    [SerializeField]
    public string script = "";
}


[Serializable]
public class TableMap
{
    [SerializeField]
    public List<string> tableDir = new List<string>();
    [SerializeField]
    public List<TableScriptMap> tableScriptMap = new List<TableScriptMap>();
}