using System;

namespace AI
{
    public class AIRunTimeBehaviorTree : IXBehaviorTree
    {
        XEntity _host = null;

        public XEntity Host { get { return _host; } set { _host = value; } }
        
        public void EnableBehaviorTree(bool enable)
        {
            throw new NotImplementedException();
        }

        public bool SetBehaviorTree(string name)
        {
            throw new NotImplementedException();
        }

        public void SetVariable(string name, object value)
        {
            throw new NotImplementedException();
        }

        public void SetManual(bool enable)
        {

        }

        public void TickBehaviorTree()
        {

        }
    }
}
