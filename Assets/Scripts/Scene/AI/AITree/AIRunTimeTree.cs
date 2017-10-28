using System;

namespace AI.Runtime
{
    public class AIRunTimeTree : IXBehaviorTree
    {
        XEntity _host = null;
        bool _enbale = false;
        AIRuntimeTreeData _tree_data;

        private XEntity Host { get { return _host; } set { _host = value; } }
        
        public void Initial(XEntity e)
        {
            Host = e;
        }

        public void EnableBehaviorTree(bool enable)
        {
            _enbale = enable;
        }

        public bool SetBehaviorTree(string name)
        {
            _tree_data = AIRuntimeUtil.Load(name);
            return true;
        }

        public void SetVariable(string name, object value)
        {
            if (_enbale && _tree_data != null)
            {
                _tree_data.SetVariable(name, value);
            }
        }

        public void SetManual(bool enable)
        {
            //do nothing here 
        }

        public void TickBehaviorTree()
        {

        }
    }
}
