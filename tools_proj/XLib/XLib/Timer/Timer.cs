//
// Timer.cs
// Created by huailiang.peng on 2016/03/14 03:19:41
//



public class Timer
{
    
    private XTimerMgr.OnTimeUpHandler m_timeUpHandler;

    private object m_param;

    //循环次数( < 0 表示无限循环)
    private int m_loop = 1;

    //计时(ms)
    private int m_totalTime;
    private int m_currentTime;

    //是否完成
    private bool m_isFinished;

    //是否处于运行状态
    private bool m_isRunning;

    //序列号
    private uint m_sequence;


    //--------------------------------------
    /// 构造函数
    //--------------------------------------
    public Timer(int time, int loop, XTimerMgr.OnTimeUpHandler timeUpHandler, uint sequence,object param)
    {
        if (loop == 0)
        {
            loop = -1;
        }

        m_totalTime = time;
        m_loop = loop;
        m_param = param;
        m_timeUpHandler = timeUpHandler;
        m_sequence = sequence;

        m_currentTime = 0;
        m_isRunning = true;
        m_isFinished = false;
    }


    //--------------------------------------
    /// Update
    /// @deltaTime
    //--------------------------------------
    public void Update(int deltaTime)
    {
        if (m_isFinished || !m_isRunning)
        {
            return;
        }

        if (m_loop == 0)
        {
            m_isFinished = true;
        }
        else
        {
            m_currentTime += deltaTime;

            if (m_currentTime >= m_totalTime)
            {
                if (m_timeUpHandler != null)
                {
                    m_timeUpHandler(m_param);
                }

                m_currentTime = 0;
                m_loop--;
            }
        }
    }

    //--------------------------------------
    /// 是否完成
    //--------------------------------------
    public bool IsFinished()
    {
        return m_isFinished;
    }

    public int CurrentTime
    {
        get { return m_currentTime; }
    }

    //--------------------------------------
    /// 暂停
    //--------------------------------------
    public void Pause()
    {
        m_isRunning = false;
    }

    //--------------------------------------
    /// 恢复
    //--------------------------------------
    public void Resume()
    {
        m_isRunning = true;
    }

    //--------------------------------------
    /// 重置
    //--------------------------------------
    public void Reset()
    {
        m_currentTime = 0;
    }

    //--------------------------------------
    /// 检查sequence是否匹配
    //--------------------------------------
    public bool IsSequenceMatched(uint sequence)
    {
        return (m_sequence == sequence);
    }

    //--------------------------------------
    /// 检查delegate是否匹配
    //--------------------------------------
    public bool IsDelegateMatched(XTimerMgr.OnTimeUpHandler timeUpHandler)
    {
        return (m_timeUpHandler == timeUpHandler);
    }

}



