using UnityEditor;
using UnityEngine;

namespace XEditor
{
    public class XAudioClip : XClip
    {
        private XAudioDataClip _data = new XAudioDataClip();

        private int _bind_idx = 0;
        private string _bind_prefab = "None";

        public XAudioClip(float timeline)
            : base(timeline)
        {
            CutSceneClip.Type = XClipType.Audio;
        }

        public XAudioClip(XCutSceneClip data)
            : base(data)
        {
        }

        public override void OnTextColor()
        {
            _textStyle.normal.textColor = Color.cyan;
        }

        public override string Name
        {
            get { return _data.Clip != null ? _data.Clip : "null"; }
        }

        public override XCutSceneClip CutSceneClip
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value as XAudioDataClip;
            }
        }

        public override void Flush()
        {
            _bind_idx = _data.BindIdx + 1;
            _bind_prefab = _bind_idx <= 0 ? "None" : CutSceneWindow.ActorList[_bind_idx];
        }

        public override void Dump()
        {
            _data.BindIdx = _bind_idx <= 0 ? -1 : _bind_idx - 1;
        }

        protected override void OnInnerGUI(XCutSceneData data)
        {
            _bind_idx = CutSceneWindow.ActorList.FindIndex(FindActor);
            _data.Clip = EditorGUILayout.TextField("Clip Name", _data.Clip);
            _bind_idx = EditorGUILayout.Popup("Bind To", _bind_idx, CutSceneWindow.ActorList.ToArray());
            EditorGUILayout.Space();
            _data.Channel = (AudioChannel)EditorGUILayout.EnumPopup("Channel", _data.Channel);
            _bind_prefab = _bind_idx > 0 ? CutSceneWindow.ActorList[_bind_idx] : "None";
            EditorGUILayout.Space();
        }

        private bool FindActor(string prefab)
        {
            return prefab == _bind_prefab;
        }

    }

}