using System.Collections.Generic;

namespace AI.Runtime
{

    public enum Mode
    {
        Selector,
        Sequence,
        Inverter,
        Custom, //自定义的Action | Conditional
    }
    
    public class AITreeVar
    {
        public bool isShared;
        public string type;
        public string name;
        public object val;
    }

   
    public class AIVar
    {
        public string type;
        public string name;
        public object val;
       
    }


    public class AITreeSharedVar : AIVar
    {
        //这个只有sharedvar才会有 根据这个值找树维护的变量
        public object bindName;

        public object isShared;

        public string BindName { get { return bindName.ToString(); } }

        public bool IsShared
        {
            get
            {
                if (isShared != null)
                    return bool.Parse(isShared.ToString());
                else
                    return false;
            }
        }
    }

    public class AIRuntimeTaskData
    {
        public Mode mode;
        public string type;
        public List<AIVar> vars;
        public List<AIRuntimeTaskData> children;
    }
    
    public class AIRuntimeTreeData
    {
        /// <summary>
        /// Variables
        /// </summary>
        public List<AITreeVar> vars { get; set; }

        /// <summary>
        /// RootTask
        /// </summary>
        public AIRuntimeTaskData task;


        public void SetVariable(string name, object value)
        {
            if (vars != null)
            {
                for (int i = 0, max = vars.Count; i < max; i++)
                {
                    if ( vars[i].name == name)
                    {
                        vars[i].val = value;
                    }
                }
            }
        }

    }
}
