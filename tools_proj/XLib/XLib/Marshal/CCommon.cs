namespace XTable
{
    using System.Runtime.InteropServices;
    
    public class CSeq<T>
    {
        public T val0, val1;

        public CSeq(ref T[] arr)
        {
            val0 = arr[0];
            val1 = arr[1];
        }

        public void Set(T v1, T v2)
        {
            val0 = v1;
            val1 = v2;
        }

        public T this[int i]
        {
            get { return i == 0 ? val0 : val1; }
        }
    }

}