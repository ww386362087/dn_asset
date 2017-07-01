using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

namespace XEditor
{
    public class TableEditor
    {

        private delegate bool EnumTableCallback(TextAsset table, string path, Type readerType);
        
        private static void EnumTable(EnumTableCallback cb, string title, Dictionary<string, Type> tableScriptMap)
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
                            if (path.EndsWith(".txt"))
                            {
                                string name = path.Replace("Assets/Resources/Table/", "");
                                name = name.Replace(".txt", "");
                                if (tableScriptMap != null)
                                {
                                    Type readerType = null;
                                    if (tableScriptMap.TryGetValue(name, out readerType))
                                    {
                                        cb(table, path, readerType);
                                    }
                                }
                                else
                                {
                                    cb(table, path, null);
                                }
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
            if (tablePath.StartsWith("Assets/Table/") && tablePath.EndsWith(".txt"))
            {
                string tableName = tablePath.Replace("Assets/Table/", "");
                tableName = tableName.Replace(".txt", "");
                return tableName.Replace("\\", "/");
            }
            return "";
        }

        private static string GetTableName(UnityEngine.Object target)
        {
            return GetTableName(AssetDatabase.GetAssetPath(target));
        }

        private static void ExeTable2Bytes(string tables, string arg0 = "-q -tables ")
        {
#if UNITY_EDITOR_WIN
            System.Diagnostics.Process exep = new System.Diagnostics.Process();
            exep.StartInfo.FileName = @"..\XProject\Shell\Table2Bytes.exe";
            exep.StartInfo.Arguments = arg0 + tables;
            exep.StartInfo.CreateNoWindow = true;
            exep.StartInfo.UseShellExecute = false;
            exep.StartInfo.RedirectStandardOutput = true;
            exep.StartInfo.StandardOutputEncoding = System.Text.Encoding.Default;
            exep.Start();
            string output = exep.StandardOutput.ReadToEnd();
            exep.WaitForExit();
            if (output != "")
            {
                Debug.LogError(output);
            }
            AssetDatabase.Refresh();
#endif
        }

        public static void Table2Bytes(UnityEngine.Object target)
        {
#if UNITY_EDITOR_WIN
            string tableName = GetTableName(target);
            if (tableName != "")
            {
                ExeTable2Bytes(tableName);
            }
            EditorUtility.DisplayDialog("Finish", "All tables processed finish", "OK");
#endif
        }

        public static void Table2Bytes(UnityEngine.Object[] targets)
        {
#if UNITY_EDITOR_WIN
            if (targets != null)
            {
                string tables = "";
                for (int i = 0; i < targets.Length; ++i)
                {
                    string tableName = AssetDatabase.GetAssetPath(targets[i]);
                    if (tableName != "")
                    {
                        tables += tableName + " ";
                    }
                }
                if (tables != "")
                {
                    ExeTable2Bytes(tables);
                }
            }
            EditorUtility.DisplayDialog("Finish", "All tables processed finish", "OK");
#endif
        }

        public static void AllTable2Bytes()
        {
            ExeTable2Bytes("");
            EditorUtility.DisplayDialog("Finish", "All tables processed finish", "OK");
        }

    }

}