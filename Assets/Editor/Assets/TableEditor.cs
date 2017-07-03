using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

namespace XEditor
{
    public class TableEditor
    {
        [MenuItem(@"Assets/Tool/Table/MakeSelect2Bytes")]
        private static void MakeTableBytes()
        {
            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);
            Table2Bytes(objs);
        }

        [MenuItem(@"Assets/Tool/Table/MakeSelect2Codes")]
        private static void MakeTableCodes()
        {
            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);
            Table2Codes(objs);
        }

        [MenuItem(@"Assets/Tool/Table/MakeAll2Bytes")]
        private static void AllTable2Bytes()
        {
            UnityEngine.Object[] objects = Resources.LoadAll<UnityEngine.Object>("Table");
            Table2Bytes(objects);
            EditorUtility.DisplayDialog("Finish", "All tables processed finish", "OK");
        }

        private static string postCsv = ".csv";
        private delegate bool EnumBytesTableCallback(TextAsset table, string path);

        private static void EnumBytesTable(EnumBytesTableCallback cb, string title)
        {
            UnityEngine.Object[] tables = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);
            if (tables != null)
            {
                for (int i = 0; i < tables.Length; ++i)
                {
                    TextAsset table = tables[i] as TextAsset;
                    string path = "";
                    if (table != null)
                    {
                        {
                            path = AssetDatabase.GetAssetPath(table);
                            if (path.EndsWith(".bytes"))
                            {
                                cb(table, path);
                            }
                        }
                    }
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", title, i, tables.Length), path, (float)i / tables.Length);
                }
            }
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All gameobjects processed finish", "OK");
        }

        public static void PostImportAssets(string[] importedAssets, string[] deletedAssets, bool warning)
        {
            bool deal = false;
            string tables = "";
            if (importedAssets != null)
            {
                for (int i = 0; i < importedAssets.Length; ++i)
                {
                    string path = importedAssets[i];
                    string tableName = GetTableName(path);
                    if (tableName != "")
                    {
                        tables += tableName + " ";
                    }
                }
                if (tables != "")
                {
                    ExeTable2Bytes(tables);
                    deal = true;
                }
            }
            for (int i = 0; i < deletedAssets.Length; ++i)
            {
                string tableName = GetTableName(deletedAssets[i]);
                if (tableName != "")
                {
                    string des = "Assets/Resources/Table/" + tableName + ".bytes";
                    if (File.Exists(des))
                    {
                        File.Delete(des);
                        deal = true;
                    }
                }
            }
            if (deal)
            {
                AssetDatabase.Refresh();
            }
        }

        private static string GetTableName(string tablePath)
        {
            if (tablePath.StartsWith("Assets/Table/") && tablePath.EndsWith(postCsv))
            {
                string tableName = tablePath.Replace("Assets/Table/", "");
                tableName = tableName.Replace(postCsv, "");
                return tableName.Replace("\\", "/");
            }
            return "";
        }

        private static string GetTableName(UnityEngine.Object target)
        {
            return GetTableName(AssetDatabase.GetAssetPath(target));
        }

        private static void ExeTable2Bytes(string tables, string arg0 = "-t ")
        {
#if UNITY_EDITOR_WIN
            System.Diagnostics.Process exep = new System.Diagnostics.Process();
            exep.StartInfo.FileName = Application.dataPath.Replace("Assets", "") 
                + @"tools_proj/XForm/WindowsFormsApplication1/bin/Debug/XForm.exe";
            exep.StartInfo.Arguments = arg0 + tables;
            exep.StartInfo.CreateNoWindow = true;
            exep.StartInfo.UseShellExecute = false;
            exep.StartInfo.RedirectStandardOutput = true;
            exep.StartInfo.StandardOutputEncoding = System.Text.Encoding.Default;
            exep.Start();
            string output = exep.StandardOutput.ReadToEnd();
            exep.WaitForExit();
            if (output != "")Debug.Log(output);
#endif
        }


        public static void Table2Bytes(UnityEngine.Object[] targets)
        {
#if UNITY_EDITOR_WIN
            string tables = MakeTableByObjects(targets);
            if (tables != "")
            {
                ExeTable2Bytes(tables, "-t ");
            }
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Finish", "All tables processed finish", "OK");
#endif
        }

        private static void Table2Codes(UnityEngine.Object[] targets)
        {
#if UNITY_EDITOR_WIN
            string tables = MakeTableByObjects(targets);
            if (tables != "")
            {
                ExeTable2Bytes(tables, "-c ");
            } 
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Finish", "All tables processed finish", "OK");
#endif
        }


        private static string MakeTableByObjects(UnityEngine.Object[] targets)
        {
            string tables = "";
            if (targets != null)
            {

                for (int i = 0; i < targets.Length; ++i)
                {
                    string tableName = AssetDatabase.GetAssetPath(targets[i]);
                    if (tableName != "")
                    {
                        tables += tableName + " ";
                    }
                }
            }
            return tables;
        }

    }

}