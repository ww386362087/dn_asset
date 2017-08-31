using UnityEditor;
using UnityEngine;

namespace XEditor
{

    public class XActorClip : XClip
    {
        public XActorClip(float timeline)
            : base(timeline)
        {
            CutSceneClip.Type = XClipType.Actor;
        }

        public XActorClip(XCutSceneClip data)
            : base(data)
        {
        }

        public override void OnTextColor()
        {
            _textStyle.normal.textColor = Color.red;
        }

        private XActorDataClip _data = new XActorDataClip();

        private int _id = 0;
        private GameObject _prefab = null;
        private bool _using_id = false;
        private AnimationClip _clip = null;
        private bool _common = false;
        private bool _appear_fold = false;
        private string _tag = null;

        protected override void OnInnerGUI(XCutSceneData data)
        {
            _using_id = EditorGUILayout.Toggle("Using ID", _using_id);

            if (!_using_id)
            {
                _id = 0;
                _prefab = EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject), true) as GameObject;
            }
            else
            {
                _id = EditorGUILayout.IntField("Statistics ID", _id);
                _prefab = XEditorLibrary.GetDummy((uint)_id);
                if (_prefab != null)
                {
                    EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject), true);
                }
                else
                    _id = 0;
            }

            if (!XEditorLibrary.CheckPrefab(_prefab)) _prefab = null;

            if (_prefab != null)
            {
                _clip = EditorGUILayout.ObjectField("Animation", _clip, typeof(AnimationClip), true) as AnimationClip;
                Vector3 Appear = Vector3FieldEx("Appear At", new Vector3(_data.AppearX, _data.AppearY, _data.AppearZ), ref _appear_fold);
                _data.AppearX = Appear.x; _data.AppearY = Appear.y; _data.AppearZ = Appear.z;
            }

            if (_using_id)
            {
                EditorGUILayout.Space();
                _common = EditorGUILayout.Toggle("Common", _common);
                _tag = EditorGUILayout.TextField("Tag", _tag);
            }
        }

        public override string Name
        {
            get { return _prefab != null ? _prefab.name : "null"; }
        }

        public override void Dump()
        {
            _data.Prefab = _using_id ? null : LocateRes(_prefab);
            _data.Clip = LocateRes(_clip);
            _data.bUsingID = _using_id;
            _data.StatisticsID = _id;
            _data.bToCommonPool = _common;
            _data.Tag = _tag;
        }

        public override void Flush()
        {
            _prefab = _data.bUsingID ? XEditorLibrary.GetDummy((uint)_data.StatisticsID) : Resources.Load(_data.Prefab) as GameObject;
            _clip = Resources.Load(_data.Clip, typeof(AnimationClip)) as AnimationClip;
            _using_id = _data.bUsingID;
            _id = _data.StatisticsID;
            _common = _data.bToCommonPool;
            _tag = _data.Tag;
        }

        public override XCutSceneClip CutSceneClip
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value as XActorDataClip;
            }
        }
    }
}