using UnityEditor;
using UnityEngine;
using XTable;

namespace XEditor
{
    public class XSlashClip : XClip
    {
        private XSlashDataClip _data = new XSlashDataClip();

        private int _id = 0;
        private string _name = null;
        private string _discription = null;
        private float _duration = 1;
        private float _x = 0;
        private float _y = 0;

        public XSlashClip(float timeline)
            : base(timeline)
        {
            CutSceneClip.Type = XClipType.Slash;
        }

        public XSlashClip(XCutSceneClip data)
            : base(data)
        {

        }

        public override string Name
        {
            get { return string.IsNullOrEmpty(_data.Name) ? "None" : _data.Name; }
        }

        public override void OnTextColor()
        {
            _textStyle.normal.textColor = Color.white;
        }

        public override XCutSceneClip CutSceneClip
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value as XSlashDataClip;
            }
        }

        public override void Flush()
        {
            _name = _data.Name;
            _discription = _data.Discription;
            _duration = _data.Duration;

            _x = _data.AnchorX;
            _y = _data.AnchorY;
        }

        public override void Dump()
        {
            _data.Name = _name;
            _data.Discription = _discription;
            _data.Duration = _duration;

            _data.AnchorX = _x;
            _data.AnchorY = _y;
        }

        protected override void OnInnerGUI(XCutSceneData data)
        {
            EditorGUILayout.Space();

            if (!data.GeneralShow)
            {
                EditorGUI.BeginChangeCheck();
                _id = EditorGUILayout.IntField("ID", _id);
                if (EditorGUI.EndChangeCheck())
                {
                    XEntityStatistics.RowData row = XEntityStatistics.sington.GetByID(_id);
                    if (row != null)
                    {
                        _name = row.Name;
                    }
                }
                _name = EditorGUILayout.TextField("Name", _name);
            }

            _discription = EditorGUILayout.TextArea(_discription, GUILayout.Height(80));
            EditorGUILayout.BeginHorizontal();
            _duration = EditorGUILayout.FloatField("Length", _duration);
            GUILayout.Label("(s)");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            _x = EditorGUILayout.FloatField("X", _x);
            _y = EditorGUILayout.FloatField("Y", _y);
        }
    }

}