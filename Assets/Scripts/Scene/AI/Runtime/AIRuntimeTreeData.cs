using System.Collections.Generic;

namespace AI.Runtime
{
    
    public class AITreeVar
    {
        public bool isShared;
        public string type;
        public string name;
        public object val;
    }

    public struct AIShareVar
    {
        public string type;
        public string name;
        public object val;
    }

    public class AIRuntimeTaskData
    {
        public string name;
        public string type;
        public List<AIShareVar> vars;
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
