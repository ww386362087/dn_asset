using System.IO;

public abstract class CSVReader
{
    public abstract class ValueParse<T>
    {
        public abstract void Read(BinaryReader stream, ref T t);
        public abstract int ReadBuffer(BinaryReader stream);
        public abstract void SkipBuffer(BinaryReader stream, int count);
    }

    public abstract int length { get; }

    public virtual string bytePath { get { return string.Empty; } }

    public bool isDone = false;
    public void Create()
    {
        string path = XConfig.stream_path + @"/" + bytePath + ".bytes";
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        ReadFile(stream);
        stream.Close();
        isDone = true;
    }
    
    public BaseRow BinarySearch(BaseRow[] table, int low, int high, int key)
    {
        if (low > high) return null;
        else
        {
            int mid = (low + high) / 2;
            if (table[mid].sortID == key)
                return table[mid];
            else if (table[mid].sortID > key)
                return BinarySearch(table, low, mid - 1, key);
            else
                return BinarySearch(table, mid + 1, high, key);
        }
    }

    public sealed class UIntParse : ValueParse<uint>
    {
        public override void Read(BinaryReader stream, ref uint t)
        {
            t = stream.ReadUInt32();
        }

        public override int ReadBuffer(BinaryReader stream)
        {
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

    public static void Init()
    {
        Uninit();
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
    }


    private static string LookupInterString(uint hash, string value)
    {
        return string.Intern(value);
    }

    public bool ReadFile(Stream stream)
    {
        lineno = 0;
        columnno = -1;
        BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.UTF8);
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
                XDebug.LogError("read table error: " + this.GetType().Name, " size:" + fileSize, " pos:" + pos, " stream: " + stream.Length);
            }
        }
        reader.Close();
        return columnno == -1;
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

    protected bool ReadSequence<T>(BinaryReader stream, ref Sequence<T> v, ValueParse<T> parse)
    {
        v = new Sequence<T>();
        T v1 = v[0]; T v2 = v[1];
        parse.Read(stream, ref v1);
        parse.Read(stream, ref v2);
        v.Set(v1, v2);
        return true;
    }


    public abstract void OnClear(int lineCount);

    public virtual void ReadLine(BinaryReader reader) { }
}

 