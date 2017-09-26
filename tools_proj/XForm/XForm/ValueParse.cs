using System;
using System.IO;

namespace XForm
{

    public enum ValueType
    {
        Atom, //粒子的
        Array, //1|2|3
        Sequence //2=3
    }

    public abstract class ValueParse
    {
        protected string exMsg { get { return string.Format("解析{0}类型{1}错误,配值为", title, GetArgType().Name); } }

        public string title { get; set; }

        public abstract ValueType type { get; }

        public abstract void Write(BinaryWriter stream, string data);

        public abstract Type GetArgType();

        protected bool CheckArray(object arr,BinaryWriter writer)
        {
            if (arr == null)
            {
                byte length=0;
                writer.Write(length);
                return false;
            }
            return true;
        }
    }

    public class IntParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Atom; } }

        public int GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            int val = 0;
            if (!int.TryParse(value, out val))
            {
                throw new Exception(exMsg+value);
            }
            return val;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            int v = GetValue(data);
            stream.Write(v);
        }

        public override Type GetArgType()
        {
            return typeof(int);
        }
    }

    public class UintParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Atom; } }

        public uint GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            uint val = 0;
            if (!uint.TryParse(value, out val))
            {
                throw new Exception(exMsg + value);
            }
            return val;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            uint v = GetValue(data);
            stream.Write(v);
        }

        public override Type GetArgType()
        {
            return typeof(uint);
        }
    }

    public class BoolParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Atom; } }

        public bool GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return true;
            bool val = true;
            if (!bool.TryParse(value, out val))
            {
                throw new Exception(exMsg + value);
            }
            return val;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            bool v = GetValue(data);
            stream.Write(v);
        }

        public override Type GetArgType()
        {
            return typeof(bool);
        }
    }

    public class FloatParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Atom; } }

        public float GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            float val = 0;
            if (!float.TryParse(value, out val))
            {
                throw new Exception(exMsg + value);
            }
            return val;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            float v = GetValue(data);
            stream.Write(v);
        }

        public override Type GetArgType()
        {
            return typeof(float);
        }
    }

    public class StringParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Atom; } }

        public string GetValue(string value)
        {
            return string.Intern(value);
        }

        public override void Write(BinaryWriter stream, string data)
        {
            string v = GetValue(data);
            stream.Write(v);
        }

        public override Type GetArgType()
        {
            return typeof(string);
        }
    }

    public class ArrIntParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Array; } }

        public int[] GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            string[] s = value.Split(CSVUtil.sington.ListSeparator);
            int[] array = new int[s.Length];
            for (int i = 0, max = s.Length; i < max; i++)
            {
                array[i] = 0;
                if (!int.TryParse(s[i], out array[i]))
                {
                    throw new Exception(exMsg + value);
                }
            }
            return array;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            int[] v = GetValue(data);
            if (CheckArray(v, stream))
            {
                byte length = (byte)v.Length;
                stream.Write(length);
                for (int i = 0, max = v.Length; i < max; i++)
                {
                    stream.Write(v[i]);
                }
            }
        }

        public override Type GetArgType()
        {
            return typeof(int[]);
        }
    }

    public class ArrUintParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Array; } }

        public uint[] GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            string[] s = value.Split(CSVUtil.sington.ListSeparator);
            uint[] array = new uint[s.Length];
            for (int i = 0, max = s.Length; i < max; i++)
            {
                array[i] = 0;
                if (!uint.TryParse(s[i], out array[i]))
                {
                    throw new Exception(exMsg + value);
                }
            }
            return array;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            uint[] v = GetValue(data);
            if (CheckArray(v,stream))
            {
                stream.Write((byte)v.Length);
                for (int i = 0, max = v.Length; i < max; i++)
                {
                    stream.Write(v[i]);
                }
            }
        }

        public override Type GetArgType()
        {
            return typeof(uint[]);
        }
    }

    public class ArrFloatParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Array; } }

        public float[] GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            string[] s = value.Split(CSVUtil.sington.ListSeparator);
            float[] array = new float[s.Length];
            for (int i = 0, max = s.Length; i < max; i++)
            {
                array[i] = 0;
                if (!float.TryParse(s[i], out array[i]))
                {
                    throw new Exception(exMsg + value);
                }
            }
            return array;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            float[] v = GetValue(data);
            if (CheckArray(v, stream))
            {
                stream.Write((byte)v.Length);
                for (int i = 0, max = v.Length; i < max; i++)
                {
                    stream.Write(v[i]);
                }
            }
        }

        public override Type GetArgType()
        {
            return typeof(float[]);
        }
    }

    public class ArrBoolParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Array; } }

        public bool[] GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            string[] s = value.Split(CSVUtil.sington.ListSeparator);
            bool[] array = new bool[s.Length];
            for (int i = 0, max = s.Length; i < max; i++)
            {
                array[i] = true;
                if (!bool.TryParse(s[i], out array[i]))
                {
                    throw new Exception(exMsg + value);
                }
            }
            return array;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            bool[] v = GetValue(data);
            if (CheckArray(v, stream))
            {
                stream.Write((byte)v.Length);
                for (int i = 0, max = v.Length; i < max; i++)
                {
                    stream.Write(v[i]);
                }
            }
        }

        public override Type GetArgType()
        {
            return typeof(bool[]);
        }
    }

    public class ArrStringParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Array; } }

        public string[] GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            string[] s = value.Split(CSVUtil.sington.ListSeparator);
            return s;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            string[] v = GetValue(data);
            if (CheckArray(v, stream))
            {
                stream.Write((byte)v.Length);
                for (int i = 0, max = v.Length; i < max; i++)
                {
                    stream.Write(v[i]);
                }
            }
        }
        public override Type GetArgType()
        {
            return typeof(string[]);
        }
    }

    public class SequenceUintParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Sequence; } }

        public override Type GetArgType()
        {
            return typeof(SequenceUintParse);
        }

        public uint[] GetValue(string value)
        {
            Exception ex = new Exception("uint sequence parse error");
           // if (string.IsNullOrEmpty(value)) return null;
            string[] s = value.Split(CSVUtil.sington.SequenceSeparator);
            uint[] u = new uint[2];
            if (s == null || string.IsNullOrEmpty(s[0])) s = new string[] { "0", "0" };
            if (!uint.TryParse(s[0], out u[0])) throw ex;
            if (!uint.TryParse(s[1], out u[1])) throw ex;
            return u;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            uint[] v = GetValue(data);
            stream.Write(v[0]);
            stream.Write(v[1]);
        }
    }


    public class SequenceIntParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Sequence; } }

        public override Type GetArgType()
        {
            return typeof(SequenceIntParse);
        }

        public int[] GetValue(string value)
        {
            Exception ex = new Exception("int sequence parse error");
            if (string.IsNullOrEmpty(value)) return null;
            string[] s = value.Split(CSVUtil.sington.SequenceSeparator);
            int[] u = new int[2];
            if (s == null) s = new string[] { "0", "0" };
            if (!int.TryParse(s[0], out u[0])) throw ex;
            if (!int.TryParse(s[1], out u[1])) throw ex;
            return u;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            int[] v = GetValue(data);
            stream.Write(v[0]);
            stream.Write(v[1]);
        }
    }


    public class SequenceFloatParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Sequence; } }

        public override Type GetArgType()
        {
            return typeof(SequenceFloatParse);
        }

        public float[] GetValue(string value)
        {
            Exception ex = new Exception("float sequence parse error");
            if (string.IsNullOrEmpty(value)) return null;
            string[] s = value.Split(CSVUtil.sington.SequenceSeparator);
            if (s == null) s = new string[] { "0", "0" };
            float[] u = new float[2];
            if (!float.TryParse(s[0], out u[0])) throw ex;
            if (!float.TryParse(s[1], out u[1])) throw ex;
            return u;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            float[] v = GetValue(data);
            stream.Write(v[0]);
            stream.Write(v[1]);
        }
    }

    public class SequenceBoolParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Sequence; } }

        public override Type GetArgType()
        {
            return typeof(SequenceBoolParse);
        }

        public bool[] GetValue(string value)
        {
            Exception ex = new Exception("bool sequence parse error");
            if (string.IsNullOrEmpty(value)) return null;
            string[] s = value.Split(CSVUtil.sington.SequenceSeparator);
            if (s == null) s = new string[] { "true", "true" };
            bool[] u = new bool[2];
            if (!bool.TryParse(s[0], out u[0])) throw ex;
            if (!bool.TryParse(s[1], out u[1])) throw ex;
            return u;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            bool[] v = GetValue(data);
            stream.Write(v[0]);
            stream.Write(v[1]);
        }
    }


    public class SequenceStringParse : ValueParse
    {
        public override ValueType type { get { return ValueType.Sequence; } }

        public override Type GetArgType()
        {
            return typeof(SequenceStringParse);
        }

        public string[] GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            string[] s = value.Split(CSVUtil.sington.SequenceSeparator);
            string[] u = new string[2];
            u[0] = (s == null || s.Length < 1) ? "" : s[0];
            u[1] = (s == null || s.Length < 2) ? "" : s[1];
            return u;
        }

        public override void Write(BinaryWriter stream, string data)
        {
            string[] v = GetValue(data);
            stream.Write(v[0]);
            stream.Write(v[1]);
        }
    }
}
