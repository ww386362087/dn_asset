namespace AI.Runtime
{
    public class AIRunTimeTree : IXBehaviorTree
    {
        XEntity _entity = null;
        bool _enbale = false;

        // 数据
        AIRuntimeTreeData _tree_data;
      
        // 表现 root_tree
        AIRunTimeBase _tree_behaviour;

        public static string[] composites= { "Sequence", "Selector", "Inverter" };

        private XEntity Host { get { return _entity; } set { _entity = value; } }
        
        public void Initial(XEntity e)
        {
            _entity = e;
        }

        public void EnableBehaviorTree(bool enable)
        {
            _enbale = enable;
        }

        public bool SetBehaviorTree(string name)
        {
            _tree_data = AIRuntimeUtil.Load(name);
            _tree_behaviour = AIRuntimeFactory.MakeRuntime(_tree_data.task);
            return true;
        }

        public void SetVariable(string name, object value)
        {
            if (_enbale && _tree_data != null)
            {
                _tree_data.SetVariable(name, value);
            }
        }

        public object GetVariable(string name)
        {
            if (_enbale && _tree_data != null && _tree_data.vars != null)
            {
                for (int i = 0, max = _tree_data.vars.Count; i < max; i++)
                {
                    if (_tree_data.vars[i].name == name)
                        return _tree_data.vars[i].val;
                }
            }
            return null;
        }

        public void SetManual(bool enable)
        {
            //do nothing here 
        }

        public void TickBehaviorTree()
        {
            if (_enbale && _tree_behaviour != null)
            {
                _tree_behaviour.OnTick(_entity);
            }
        }
        

    }
}
