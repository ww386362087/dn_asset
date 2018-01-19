#if TEST

using System;
using UnityEngine;

public class TestNativeAI : ITest
{

    bool m_tick = true;
    float m_tick_time = 0f;
    float spf = 0.033f;//30fps

    public void LateUpdate()
    {
    }

    public void OnGUI()
    {
    }

    public void OnQuit()
    {
        m_tick = false;
        NativeInterface.iQuitCore();
    }

    public void Start()
    {
        m_tick = true;
        NativeInterface.iStartCore();
    }

    public void Update()
    {
        if (m_tick)
        {
            m_tick_time += Time.deltaTime;
            if (m_tick_time > spf)
            {
                NativeInterface.iTickCore(m_tick_time);
                m_tick_time = 0;
            }
        }
    }
}
#endif