//
// TimerManager.cs
// Created by huailiang.peng on 2016/03/14 03:19:41
//


using System.Collections.Generic;
using UnityEngine;

public class TimerManager : XSingleton<TimerManager>
{

    //Timer类型
    private enum enTimerType
    {
        Normal,
        FrameSync,
    };

    //Timer List
    private List<Timer>[] m_timers;
    private int m_timerSequence;

    //----------------------------------------------
    /// 初始化
    //----------------------------------------------
    public override bool Init()
    {
        m_timers = new List<Timer>[System.Enum.GetValues(typeof(enTimerType)).Length];

        for (int i = 0; i < m_timers.Length; i++)
        {
            m_timers[i] = new List<Timer>();
        }
        m_timerSequence = 0;

        return base.Init();
    }

    //----------------------------------------------
    /// Update
    /// @这里只更新Normal类型的Timer
    //----------------------------------------------
    public void Update(float delta)
    {
        AdvanceTimer((int)(delta * 1000), enTimerType.Normal);
    }

    //----------------------------------------------
    /// UpdateLogic
    /// @这里只更新FrameSync类型的Timer
    //----------------------------------------------
    public void UpdateLogic(int delta)
    {
        AdvanceTimer(delta, enTimerType.FrameSync);
    }

    private void AdvanceTimer(int delta, enTimerType timerType)
    {
        List<Timer> timers = m_timers[(int)timerType];

        for (int i = 0; i < timers.Count;)
        {
            if (timers[i].IsFinished())
            {
                timers.RemoveAt(i);
                continue;
            }

            timers[i].Update(delta);
            i++;
        }
    }


    public int AddTimer(float time, Timer.OnTimeUpHandler onTimeUpHandler)
    {
        return AddTimer((int)(time * 1000), onTimeUpHandler);
    }

    public int AddTimer(int time, Timer.OnTimeUpHandler onTimeUpHandler)
    {
        return AddTimer(time, onTimeUpHandler, null);
    }

    public int AddTimer(float time, Timer.OnTimeUpHandler onTimeUpHandler, object param)
    {
        return AddTimer((int)(time * 1000), 1, onTimeUpHandler, param);
    }

    public int AddTimer(int time, Timer.OnTimeUpHandler onTimeUpHandler, object param)
    {
        return AddTimer(time, 1, onTimeUpHandler, param);
    }

    //----------------------------------------------
    /// 添加Timer
    /// @time               : 计时时间(ms)
    /// @loop               : 循环次数
    /// @onTimeUpHandler    : 时间到时的回调函数
    /// @return sequence of timer
    //----------------------------------------------
    public int AddTimer(int time, int loop, Timer.OnTimeUpHandler onTimeUpHandler, object param)
    {
        return AddTimer(time, loop, onTimeUpHandler, param, false);
    }

    //----------------------------------------------
    /// 添加Timer
    /// @time               : 计时时间(ms)
    /// @loop               : 循环次数
    /// @onTimeUpHandler    : 时间到时的回调函数
    /// @useFrameSync       : 是否使用桢同步
    /// @return sequence of timer
    //----------------------------------------------
    public int AddTimer(int time, int loop, Timer.OnTimeUpHandler onTimeUpHandler, object param, bool useFrameSync)
    {
        m_timerSequence++;
        m_timers[(int)(useFrameSync ? enTimerType.FrameSync : enTimerType.Normal)].Add(new Timer(time, loop, onTimeUpHandler, m_timerSequence, param));
        return m_timerSequence;
    }

    //----------------------------------------------
    /// 移除Timer
    /// @sequence
    //----------------------------------------------
    public void RemoveTimer(int sequence)
    {
        for (int i = 0; i < m_timers.Length; i++)
        {
            List<Timer> timers = m_timers[i];

            for (int j = 0; j < timers.Count;)
            {
                if (timers[j].IsSequenceMatched(sequence))
                {
                    timers.RemoveAt(j);
                    return;
                }

                j++;
            }
        }
    }

    //----------------------------------------------
    /// 移除Timer
    /// @sequence: ref，移除后清空
    //----------------------------------------------
    public void RemoveTimerSafely(ref int sequence)
    {
        if (sequence != 0)
        {
            RemoveTimer(sequence);
            sequence = 0;
        }
    }

    //----------------------------------------------
    /// 暂停Timer
    /// @sequence
    //----------------------------------------------
    public void PauseTimer(int sequence)
    {
        Timer timer = GetTimer(sequence);

        if (timer != null)
        {
            timer.Pause();
        }
    }

    //----------------------------------------------
    /// 恢复Timer
    /// @sequence
    //----------------------------------------------
    public void ResumeTimer(int sequence)
    {
        Timer timer = GetTimer(sequence);

        if (timer != null)
        {
            timer.Resume();
        }
    }

    //----------------------------------------------
    /// 重置Timer
    /// @sequence
    //----------------------------------------------
    public void ResetTimer(int sequence)
    {
        Timer timer = GetTimer(sequence);

        if (timer != null)
        {
            timer.Reset();
        }
    }

    //----------------------------------------------
    /// 获取Timer的当前时间
    /// @sequence
    //----------------------------------------------
    public int GetTimerCurrent(int sequence)
    {
        Timer timer = GetTimer(sequence);

        if (timer != null)
        {
            return timer.CurrentTime;
        }

        return -1;
    }

    //----------------------------------------------
    /// 返回指定sequence的Timer
    //----------------------------------------------
    private Timer GetTimer(int sequence)
    {
        for (int i = 0; i < m_timers.Length; i++)
        {
            List<Timer> timers = m_timers[i];

            for (int j = 0; j < timers.Count; j++)
            {
                if (timers[j].IsSequenceMatched(sequence))
                {
                    return timers[j];
                }
            }
        }

        return null;
    }

    //----------------------------------------------
    /// 移除Timer
    /// @onTimeUpHandler
    //----------------------------------------------
    public void RemoveTimer(Timer.OnTimeUpHandler onTimeUpHandler)
    {
        RemoveTimer(onTimeUpHandler, false);
    }

    //----------------------------------------------
    /// 移除Timer
    /// @onTimeUpHandler
    /// @useFrameSync
    //----------------------------------------------
    public void RemoveTimer(Timer.OnTimeUpHandler onTimeUpHandler, bool useFrameSync)
    {
        List<Timer> timers = m_timers[(int)(useFrameSync ? enTimerType.FrameSync : enTimerType.Normal)];

        for (int i = 0; i < timers.Count;)
        {
            if (timers[i].IsDelegateMatched(onTimeUpHandler))
            {
                timers.RemoveAt(i);
                continue;
            }

            i++;
        }
    }

    //----------------------------------------------
    /// 移除所有Timer
    /// @timerType
    //----------------------------------------------
    public void RemoveAllTimer(bool useFrameSync)
    {
        m_timers[(int)(useFrameSync ? enTimerType.FrameSync : enTimerType.Normal)].Clear();
    }

    //----------------------------------------------
    /// 移除所有Timer
    //----------------------------------------------
    public void RemoveAllTimer()
    {
        for (int i = 0; i < m_timers.Length; i++)
        {
            m_timers[i].Clear();
        }
    }


}
