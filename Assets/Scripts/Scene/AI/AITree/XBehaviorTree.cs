using BehaviorDesigner.Runtime;
using UnityEngine;

namespace AI
{
    public class XBehaviorTree : MonoBehaviour, IXBehaviorTree
    {
        private BehaviorTree _behavior_tree = null;

        void Awake()
        {
            _behavior_tree = gameObject.AddComponent<BehaviorTree>();
        }

        public void SetVariable(string name, object value)
        {
            if (_behavior_tree == null) return;
            SharedVariable sharedvar = _behavior_tree.GetVariable(name);
            if (sharedvar != null) sharedvar.SetValue(value);
        }

        public void SetNavPoint(Transform navpoint)
        {
            if (_behavior_tree == null) return;
            SharedTransform innernav = (SharedTransform)_behavior_tree.GetVariable(AITreeArg.NavTarget);
            if (innernav != null) innernav.SetValue(navpoint);
        }

        public float GetHeartRate()
        {
            if (_behavior_tree == null) return 0;
            SharedFloat innerRate = (SharedFloat)_behavior_tree.GetVariable(AITreeArg.HeartRate);
            return innerRate.Value;
        }

        public void EnableBehaviorTree(bool enable)
        {
            if (_behavior_tree == null) return;
            if (enable)
                _behavior_tree.EnableBehavior();
            else
                _behavior_tree.DisableBehavior();
        }

        public bool SetBehaviorTree(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string location = "Assets/Behavior Designer/AIData/" + name + ".asset";
            ExternalBehaviorTree ebt = UnityEditor.AssetDatabase.LoadAssetAtPath(location, typeof(ExternalBehaviorTree)) as ExternalBehaviorTree;
            _behavior_tree.ExternalBehavior = ebt;
            _behavior_tree.RestartWhenComplete = true;
            return true;
        }

        public void SetManual(bool enable)
        {
            if (enable)
            {
                BehaviorManager.instance.UpdateInterval = UpdateIntervalType.Manual;
            }
        }


        public void TickBehaviorTree()
        {
            if (_behavior_tree != null)
            {
                BehaviorManager.instance.Tick(_behavior_tree);
            }
        }
    }
}