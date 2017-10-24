
namespace AI
{
    public interface IXBehaviorTree
    {
        void SetVariable(string name, object value);

        void EnableBehaviorTree(bool enable);

        bool SetBehaviorTree(string name);

        void SetManual(bool enable);

        void TickBehaviorTree();
    }

}
