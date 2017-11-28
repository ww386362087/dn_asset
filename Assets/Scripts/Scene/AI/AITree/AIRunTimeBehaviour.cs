using System.Collections.Generic;

namespace AI.Runtime
{
    public enum AIRuntimeStatus
    {
        Inactive = 0,
        Failure = 1,
        Success = 2,
        Running = 3
    }

    public abstract class AIRunTimeBase
    {
        protected AIRunTimeTree _tree;

        public virtual void Init(AIRuntimeTaskData data) { }

        public void SetTree(AIRunTimeTree tree)
        {
            _tree = tree;
        }

        public abstract AIRuntimeStatus OnTick(XEntity entity);
    }

    public class AIRuntimeSequence : AIRunTimeBase
    {
        private List<AIRunTimeBase> list;

        public override void Init(AIRuntimeTaskData data)
        {
            if (data.children != null)
            {
                for (int i = 0, max = data.children.Count; i < max; i++)
                {
                    if (list == null) list = new List<AIRunTimeBase>();
                    AIRunTimeBase run = AIRuntimeFactory.MakeRuntime(data.children[i],_tree);
                    list.Add(run);
                }
            }
        }

        public override AIRuntimeStatus OnTick(XEntity entity)
        {
            if (list != null)
            {
                for (int i = 0, max = list.Count; i < max; i++)
                {
                    if (list[i].OnTick(entity) == AIRuntimeStatus.Failure)
                    {
                        return AIRuntimeStatus.Failure;
                    }
                }
            }
            return AIRuntimeStatus.Success;
        }
    }

    public class AIRuntimeSelector : AIRunTimeBase
    {
        private List<AIRunTimeBase> list;

        public override void Init(AIRuntimeTaskData data)
        {
            if (data.children != null)
            {
                for (int i = 0, max = data.children.Count; i < max; i++)
                {
                    if (list == null) list = new List<AIRunTimeBase>();
                    AIRunTimeBase run = AIRuntimeFactory.MakeRuntime(data.children[i],_tree);
                    list.Add(run);
                }
            }
        }

        public override AIRuntimeStatus OnTick(XEntity entity)
        {
            if (list != null)
            {
                for (int i = 0, max = list.Count; i < max; i++)
                {
                    if(list[i].OnTick(entity)== AIRuntimeStatus.Success)
                    {
                        return AIRuntimeStatus.Success;
                    }
                }
            }
            return AIRuntimeStatus.Failure;
        }
    }

    public class AIRuntimeInverter : AIRunTimeBase
    {
        private AIRunTimeBase node;

        public override void Init(AIRuntimeTaskData data)
        {
            if (data.children != null && data.children.Count > 0)
            {
                node = AIRuntimeFactory.MakeRuntime(data.children[0], _tree);
            }
        }

        public override AIRuntimeStatus OnTick(XEntity entity)
        {
            if(node!=null)
            {
                var rst = node.OnTick(entity);
                if(rst== AIRuntimeStatus.Failure)
                {
                    return AIRuntimeStatus.Success;
                }
                else if(rst == AIRuntimeStatus.Success)
                {
                    return AIRuntimeStatus.Failure;
                }
                return rst;
            }
            return AIRuntimeStatus.Success;
        }
    }

}
