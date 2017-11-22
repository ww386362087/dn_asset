using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace XEditor
{

    public class BytesTableViewEditor : EditorWindow
    {
        private TextAsset table = null;
        private string tableName = "";
        private Type tableType = null;
        private Type tableRowType = null;
        private FieldInfo[] tableRowTypeField = null;
        private CSVReader reader = null;
        private FieldInfo tableListInfo = null;
        private System.Collections.IList tableList = null;
        private List<string> tableData = new List<string>();
        private Vector2 scrollPos;
        private GUILayoutOption scrollOp0 = null;
        private int pageCount = 50;
        private int totalPage = 0;
        private int currentPage = 0;
        private int gotoPage = 0;
        private string searchKey = "";
        private List<int> findLine = new List<int>();
        public void Init(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                table = null;
                tableName = "";
                return;
            }
            string path = AssetDatabase.GetAssetPath(obj);
            if (path.StartsWith("Assets/Resources/Table/") && path.EndsWith(".bytes"))
            {
                table = obj as TextAsset;
                tableName = path.Replace("Assets/Resources/Table/", "");
                tableName = tableName.Replace(".bytes", "");
                Assembly ass = Assembly.Load("Assembly-CSharp");
                if (ass == null)  XDebug.LogError("asse is null");
                tableType = ass.GetType("XTable." + tableName);
                if (tableType != null)
                {
                    tableRowType = tableType.GetNestedType("RowData");
                    if (tableRowType != null)
                    {
                        tableRowTypeField = tableRowType.GetFields();
                        tableListInfo = tableType.GetField("Table");
                        reader = Activator.CreateInstance(tableType) as CSVReader;
                        if (reader != null)
                        {
                            CSVReader.Init();
                            Stream stream = new MemoryStream(table.bytes);
                            reader.ReadFile(stream);
                            tableList = tableListInfo.GetValue(reader) as IList;
                            CSVReader.Uninit();
                            tableData.Clear();
                            if (tableList != null && tableRowTypeField != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                string formatStr = "{{{0}:{1}}},";
                                string formatStrEnd = "{{{0}:{1}}}";
                                string format = "";
                                for (int x = 0; x < tableList.Count; ++x)
                                {
                                    System.Object data = tableList[x];
                                    for (int y = 0; y < tableRowTypeField.Length; ++y)
                                    {
                                        FieldInfo fi = tableRowTypeField[y];
                                        System.Object value = fi.GetValue(data);
                                        string str = "";
                                        if (value is Array)
                                        {
                                            Array arr = value as Array;
                                            IList lst = value as IList;
                                            if (arr != null && lst != null)
                                            {
                                                for (int a = 0; a < arr.Length; ++a)
                                                {
                                                    str += lst[a].ToString();
                                                    if (a != arr.Length - 1)
                                                    {
                                                        str += "|";
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (value != null)
                                            {
                                                str = value.ToString();
                                            }
                                        }
                                        if (y != tableRowTypeField.Length - 1)
                                        {
                                            format = formatStrEnd;
                                        }
                                        else
                                        {
                                            format = formatStr;
                                        }
                                        sb.Append(string.Format(format, y, str));
                                    }
                                    tableData.Add(sb.ToString());
                                    sb.Length = 0;
                                }
                                RefreshPage(tableData.Count);
                            }
                        }
                    }
                }
                else
                {
                     XDebug.LogError("table: " , tableName , " assem is " + Assembly.GetExecutingAssembly().GetName().Name);
                }
            }
        }


        private void RefreshPage(int count)
        {
            currentPage = 0;
            totalPage = count / pageCount;
            if (tableData.Count % pageCount > 0)
            {
                totalPage++;
            }
        }

        protected virtual void OnGUI()
        {
            if (scrollOp0 == null)
            {
                scrollOp0 = GUILayout.ExpandWidth(true);
            }
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Table");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Pre Page", GUILayout.Width(80)))
            {
                currentPage--;
                if (currentPage < 0)
                {
                    currentPage = 0;
                }
            }
            if (GUILayout.Button("Next Page", GUILayout.Width(80)))
            {
                currentPage++;
                if (currentPage >= totalPage)
                {
                    currentPage = totalPage;
                }
            }
            int page = EditorGUILayout.IntField(gotoPage, GUILayout.Width(80));
            if (page != currentPage && page > 0 && page <= totalPage)
            {
                gotoPage = page;
            }
            if (GUILayout.Button("GoTo", GUILayout.Width(80)))
            {
                currentPage = gotoPage - 1;
            }
            GUILayout.Label(string.Format("Page:{0}/{1} Line:{2}", currentPage + 1, totalPage, tableData.Count));


            string key = GUILayout.TextField(searchKey, GUILayout.Width(300));
            if (searchKey != key)
            {
                searchKey = key;
                if (searchKey == "")
                {
                    RefreshPage(tableData.Count);
                }
            }
            if (GUILayout.Button("Search", GUILayout.Width(80)) && searchKey != "")
            {
                findLine.Clear();
                for (int i = 0; i < tableData.Count; ++i)
                {
                    string col = tableData[i];
                    int index = col.IndexOf(searchKey);
                    if (index >= 0)
                    {
                        findLine.Add(i);
                    }
                }
                if (findLine.Count > 0)
                {
                    RefreshPage(findLine.Count);
                }
            }

            GUILayout.EndHorizontal();

            if (tableRowTypeField != null)
            {
                GUILayout.BeginHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                GUILayout.BeginHorizontal();
                for (int i = 0; i < tableRowTypeField.Length; ++i)
                {
                    FieldInfo fi = tableRowTypeField[i];
                    GUILayout.Button(string.Format("{0}:{1}", i, fi.Name));
                }
                GUILayout.EndHorizontal();
                int endCount = (currentPage + 1) * pageCount;
                if (findLine.Count > 0)
                {
                    for (int i = currentPage * pageCount; i < findLine.Count && i < endCount; ++i)
                    {
                        int index = findLine[i];
                        string col = tableData[index];
                        GUILayout.BeginHorizontal();
                        GUILayout.TextField(col);
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    for (int i = currentPage * pageCount; i < tableData.Count && i < endCount; ++i)
                    {
                        string col = tableData[i];
                        GUILayout.BeginHorizontal();
                        GUILayout.TextField(col);
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.EndScrollView();
                GUILayout.EndHorizontal();
            }

        }
    }
}