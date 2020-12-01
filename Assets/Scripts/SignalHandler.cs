using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SignalHandler
{
	public delegate void handlerDelegate();

#if !UNITY_EDITOR && UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern void RegSigHandler(int s, handlerDelegate handler);

    [DllImport ("__Internal")]
    public static extern int ReadStackTrace(byte[] buf, int bufLen);

    [DllImport ("__Internal")]
    public static extern void Signal(int s);
#endif
}
