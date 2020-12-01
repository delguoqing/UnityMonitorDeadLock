using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Threading;

public class Test : MonoBehaviour
{
    static int shouldStopDeadLock = 0;
    long counter = 0;

#if !UNITY_EDITOR && UNITY_IPHONE    
    // C# Main Thread Setup Signal Handler
    void MainThreadSetupSignalHandler() { 
        SignalHandler.RegSigHandler(30, new SignalHandler.handlerDelegate(OnUserSignal));
    }

    [AOT.MonoPInvokeCallback(typeof(SignalHandler.handlerDelegate))]
    static void OnUserSignal() {
        byte[] stackstrace = new byte[8192];
        int stackTraceLen = SignalHandler.ReadStackTrace(stackstrace, stackstrace.Length);
        var stackTraceStr = System.Text.Encoding.UTF8.GetString(stackstrace, 0, stackTraceLen);
        Debug.LogError(stackTraceStr);

        shouldStopDeadLock = 1;
    }

    void Update() {
        Interlocked.Increment(ref counter);
    }

    void MainThreadDeadLock() {
        
        Thread.Sleep(1000 * 1000);

        while (shouldStopDeadLock == 0) {
            // tight loop
        }
    }

    void StartMonitorThread() {
        var t = new Thread(MonitorThreadWork);
        t.Start();
    }

    void MonitorThreadWork() {
        var lastValue = Interlocked.Read(ref counter);
        while (true) {
            Thread.Sleep(5000);
            var nowValue = Interlocked.Read(ref counter);
            if (nowValue == lastValue) {    // dead lock!
                SignalHandler.Signal(30);
                break;
            }
        }
    }
#endif    

    void OnGUI() {
        if ( GUI.Button(new Rect(100, 100, 500, 500), "Enter Dead Loop")) {
#if !UNITY_EDITOR && UNITY_IPHONE
            MainThreadSetupSignalHandler();
            StartMonitorThread();
            MainThreadDeadLock();
#endif
        }
    }
}
