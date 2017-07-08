using UnityEngine;
using System;

public class XException : ApplicationException
{
    public XException()
    { }

    public XException(string message)
        : base(message)
    { }

    public XException(string message, Exception inner)
        : base(message, inner)
    { } 
   
}

[Serializable]
public class XComponentException : XException
{
    public XComponentException(string message)
        : base(message)
    { }

    public XComponentException(string message, Exception inner)
        : base(message, inner)
    { }
}


[Serializable]
public class XDocumentException : XException
{
    public XDocumentException(string message)
        : base(message)
    { }

    public XDocumentException(string message, Exception inner)
        : base(message, inner)
    { }
}