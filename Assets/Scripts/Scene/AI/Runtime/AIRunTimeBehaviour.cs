using System.Collections.Generic;

namespace AI.Runtime
{
    public enum AIRuntimeStatus
    {
        Running,
        Success,
        Failed
    }

    public abstract class AIRunTimeBase
    {
        public abstract void Init(AIRuntimeTaskData data);

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
                    AIRunTimeBase run = AIRuntimeFactory.singleton.MakeRuntime(data.children[i]);
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
                    if (list[i].OnTick(entity) == AIRuntimeStatus.Failed)
                    {
                        return AIRuntimeStatus.Failed;
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
                    AIRunTimeBase run = AIRuntimeFactory.singleton.MakeRuntime(data.children[i]);
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
            return AIRuntimeStatus.Failed;
        }
    }

    public class AIRuntimeInverter : AIRunTimeBase
    {
        private AIRunTimeBase node;

        public override void Init(AIRuntimeTaskData data)
        {
            if (data.children != null && data.children.Count > 0)
            {
                node = AIRuntimeFactory.singleton.MakeRuntime(data.children[0]);
            }
        }

        public override AIRuntimeStatus OnTick(XEntity entity)
        {
            if(node!=null)
            {
                var rst = node.OnTick(entity);
                if(rst== AIRuntimeStatus.Failed)
                {
                    return AIRuntimeStatus.Success;
                }
                else if(rst == AIRuntimeStatus.Success)
                {
                    return AIRuntimeStatus.Failed;
                }
                return rst;
            }
            return AIRuntimeStatus.Success;
        }
    }

    public class AIRuntimeConditional : AIRunTimeBase
    {
        public override void Init(AIRuntimeTaskData data)
        {
        }

        public override AIRuntimeStatus OnTick(XEntity entity)
        {
            return AIRuntimeStatus.Success;
        }
    }


    public class AIRuntimeAction : AIRunTimeBase
    {
        public override void Init(AIRuntimeTaskData data)
        {
        }

        public override AIRuntimeStatus OnTick(XEntity entity)
        {
            return AIRuntimeStatus.Success;
        }
    }

}
