using UnityEditor;
using UnityEngine;

namespace XEditor
{
    public class XPlayerClip : XClip
    {
        public XPlayerClip(float timeline)
            : base(timeline)
        {
            CutSceneClip.Type = XClipType.Player;
            _prefab = AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorResources/Prefabs/ZJ_zhanshi.prefab", typeof(GameObject)) as GameObject;
        }

        public XPlayerClip(XCutSceneClip data)
            : base(data)
        {
            _prefab = AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorResources/Prefabs/ZJ_zhanshi.prefab", typeof(GameObject)) as GameObject;
        }

        public override void OnTextColor()
        {
            _textStyle.normal.textColor = Color.red;
        }

        private XPlayerDataClip _data = new XPlayerDataClip();

        private GameObject _prefab = null;

        private AnimationClip _clip1 = null;
        private AnimationClip _clip2 = null;
        private AnimationClip _clip3 = null;
        private AnimationClip _clip4 = null;
        private AnimationClip _clip5 = null;
        private AnimationClip _clip6 = null;

        private bool _appear_fold = false;

        protected override void OnInnerGUI(XCutSceneData data)
        {
            _clip1 = EditorGUILayout.ObjectField("Warrior Animation", _clip1, typeof(AnimationClip), true) as AnimationClip;
            _clip2 = EditorGUILayout.ObjectField("Archer Animation", _clip2, typeof(AnimationClip), true) as AnimationClip;
            _clip3 = EditorGUILayout.ObjectField("Sorceress Animation", _clip3, typeof(AnimationClip), true) as AnimationClip;
            _clip4 = EditorGUILayout.ObjectField("Cleric Animation", _clip4, typeof(AnimationClip), true) as AnimationClip;
            _clip5 = EditorGUILayout.ObjectField("Academic Animation", _clip5, typeof(AnimationClip), true) as AnimationClip;
            _clip6 = EditorGUILayout.ObjectField("Assassin Animation", _clip6, typeof(AnimationClip), true) as AnimationClip;

            Vector3 Appear = Vector3FieldEx("Appear At", new Vector3(_data.AppearX, _data.AppearY, _data.AppearZ), ref _appear_fold);
            _data.AppearX = Appear.x; _data.AppearY = Appear.y; _data.AppearZ = Appear.z;
        }

        public override string Name
        {
            get { return _prefab != null ? _prefab.name : "null"; }
        }

        public override void Dump()
        {
            _data.Clip1 = LocateRes(_clip1);
            _data.Clip2 = LocateRes(_clip2);
            _data.Clip3 = LocateRes(_clip3);
            _data.Clip4 = LocateRes(_clip4);
            _data.Clip5 = LocateRes(_clip5);
            _data.Clip6 = LocateRes(_clip6);
        }

        public override void Flush()
        {
            _clip1 = Resources.Load(_data.Clip1, typeof(AnimationClip)) as AnimationClip;
            _clip2 = Resources.Load(_data.Clip2, typeof(AnimationClip)) as AnimationClip;
            _clip3 = Resources.Load(_data.Clip3, typeof(AnimationClip)) as AnimationClip;
            _clip4 = Resources.Load(_data.Clip4, typeof(AnimationClip)) as AnimationClip;
            _clip5 = Resources.Load(_data.Clip5, typeof(AnimationClip)) as AnimationClip;
            _clip6 = Resources.Load(_data.Clip6, typeof(AnimationClip)) as AnimationClip;
        }

        public override XCutSceneClip CutSceneClip
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value as XPlayerDataClip;
            }
        }
    }
}
