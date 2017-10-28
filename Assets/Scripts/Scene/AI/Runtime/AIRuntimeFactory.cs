namespace AI.Runtime
{
    public class AIRuntimeFactory:XSingleton<AIRuntimeFactory>
    {

        public AIRunTimeBase MakeRuntime(AIRuntimeTaskData data)
        {
            AIRunTimeBase rst = null;
            switch (data.type)
            {
                case "Sequence":
                    rst = new AIRuntimeSequence();
                    break;
                case "Selector":
                    rst = new AIRuntimeSelector();
                    break;
                case "Inverter":
                    rst = new AIRuntimeInverter();
                    break;
                default:
                  //  rst = new 
                    break;
            }
            if (rst != null)
            {
                rst.Init(data);
            }
            return rst;
        }

    }
}
