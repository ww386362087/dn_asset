using UnityEditor;
using UnityEngine;

namespace XEditor
{
    public class XSubTitleClip : XClip
    {
        public XSubTitleClip(float timeline)
            : base(timeline)
        {
            CutSceneClip.Type = XClipType.SubTitle;
        }

        public XSubTitleClip(XCutSceneClip data)
            : base(data)
        {

        }

        private XSubTitleDataClip _data = new XSubTitleDataClip();

        public override string Name
        {
            get
            {
                if (_data.Context != null)
                {
                    return _data.Context.Length < 3 ? _data.Context : _data.Context.Substring(0, 3) + "...";
                }
                else
                    return "null";
            }
        }

        public override XCutSceneClip CutSceneClip
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value as XSubTitleDataClip;
            }
        }

        public override void Flush()
        {

        }

        public override void Dump()
        {

        }

        protected override void OnInnerGUI(XCutSceneData data)
        {
            _data.Context = EditorGUILayout.TextArea(_data.Context, GUILayout.Height(80));
            EditorGUILayout.BeginHorizontal();
            _data.Duration = EditorGUILayout.FloatField("Duration", _data.Duration);
            GUILayout.Label("(frame)");
            EditorGUILayout.EndHorizontal();
        }

        public override void OnTextColor()
        {
            _textStyle.normal.textColor = Color.green;
        }
    }
}