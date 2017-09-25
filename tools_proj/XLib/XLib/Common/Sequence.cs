using System;

/// <summary>
/// 内部实现一个与Vector2 相同的结构
/// 表格配置格式： 2=3
/// </summary>
public class Sequence<T>
{
    public T[] arr = new T[2];

    public void Set(T v1, T v2)
    {
        arr[0] = v1;
        arr[1] = v2;
    }


    public T this[int i]
    {
        get { return arr[i]; }
    }

}
