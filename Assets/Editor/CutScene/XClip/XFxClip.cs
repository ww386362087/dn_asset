using UnityEditor;
using UnityEngine;

namespace XEditor
{
    public class XFxClip : XClip
    {
        private XFxDataClip _data = new XFxDataClip();

        private GameObject _fx = null;
        private GameObject _bone = null;

        private int _bind_idx = 0;
        private string _bind_prefab = "None";

        private bool _appear_fold = false;
        private bool _face_fold = false;

        private bool _bone_refresh = false;

        public XFxClip(float timeline)
            : base(timeline)
        {
            CutSceneClip.Type = XClipType.Fx;
        }

        public XFxClip(XCutSceneClip data)
            : base(data)
        {
        }

        public override void OnTextColor()
        {
            _textStyle.normal.textColor = Color.yellow;
        }

        public override string Name
        {
            get { return _fx != null ? _fx.name : "null"; }
        }

        public override XCutSceneClip CutSceneClip
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value as XFxDataClip;
            }
        }

        public override void Flush()
        {
            _fx = Resources.Load(_data.Fx) as GameObject;
            _bone_refresh = false;
            _bind_idx = _data.BindIdx + 1;
            _bind_prefab = CutSceneWindow.ActorList[_bind_idx];
        }

        public override void Dump()
        {
            _data.Fx = XClip.LocateRes(_fx);
            if (_bone != null) _data.Bone = XClip.LocateBone(_bone);
            _data.BindIdx = _bind_idx - 1;
        }

        protected override void OnInnerGUI(XCutSceneData data)
        {
            _bind_idx = CutSceneWindow.ActorList.FindIndex(FindActor);

            _fx = EditorGUILayout.ObjectField("Fx Object", _fx, typeof(GameObject), true) as GameObject;
            _bind_idx = EditorGUILayout.Popup("Bind To", _bind_idx, CutSceneWindow.ActorList.ToArray());
            EditorGUILayout.Space();
            if (_bind_idx > 0)
            {
                EditorGUI.BeginChangeCheck();
                _bone = EditorGUILayout.ObjectField("Bind To", _bone, typeof(GameObject), true) as GameObject;
                if (EditorGUI.EndChangeCheck() || !_bone_refresh)
                {
                    if (_bone != null || !_bone_refresh)
                    {
                        if (_bone != null) _bone_refresh = true;
                        if (_bone_refresh) _data.Bone = XClip.LocateBone(_bone);
                    }
                    else
                    {
                        _data.Bone = null;
                    }
                }
                if (_data.Bone != null && _data.Bone.Length > 0) EditorGUILayout.LabelField("Bone", _data.Bone.Substring(_data.Bone.LastIndexOf("/") + 1));
                _data.Follow = EditorGUILayout.Toggle("Follow", _data.Follow);
                _data.Scale = EditorGUILayout.FloatField("Scale", _data.Scale);
            }
            else
            {
                _bone = null;
                _data.Bone = null;

                Vector3 Appear = Vector3FieldEx("Appear At", new Vector3(_data.AppearX, _data.AppearY, _data.AppearZ), ref _appear_fold, false, false);
                _data.AppearX = Appear.x; _data.AppearY = Appear.y; _data.AppearZ = Appear.z;

                Vector3 Face = XCommon.singleton.FloatToAngle(_data.Face);
                _data.Face = XCommon.singleton.AngleToFloat(Vector3FieldEx("Face To", Face, ref _face_fold, true, false));
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            _data.Destroy_Delay = EditorGUILayout.FloatField("Delay Destroy", _data.Destroy_Delay);
            GUILayout.Label("(s)");
            EditorGUILayout.EndHorizontal();

            _bind_prefab = CutSceneWindow.ActorList[_bind_idx];
        }

        // Explicit predicate delegate. 
        private bool FindActor(string prefab)
        {
            return (prefab == _bind_prefab);
        }
    }
}
