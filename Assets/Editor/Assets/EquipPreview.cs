﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using XTable;

namespace XEditor
{
    public class EquipPreview : EditorWindow
    {
        public class EquipPart
        {
            public string[] partPath = new string[8];
            public string mainWeapon;
            public uint hash = 0;
            public List<string> suitName = new List<string>();
        }

        public class TempEquipSuit
        {
            public uint hash = 0;
            public List<TempEquipData> data = new List<TempEquipData>();

        }

        public class TempEquipData
        {
            public FashionList.RowData row;
            public string path;
        }

        public class ThreePart
        {
            public string[] part = new string[3];
            public int id = 0;
        }
        private CombineConfig combineConfig = null;
        private DefaultEquip defaultEquip = new DefaultEquip(true);
        private FashionList fashionList = new FashionList(true);
        private FashionSuit fashionSuit = new FashionSuit(true);
        private EquipSuit equipSuit = new EquipSuit(true);

        private int m_professionIndex = 0;
        private FieldInfo[] fashionTypeField = null;
        private List<EquipPart>[] m_FashionList = null;
        private List<EquipPart>[] m_EquipList = null;
        private List<ThreePart>[] m_ThreePartList = null;
        private Vector2 fashionScrollPos = Vector2.zero;
        private Vector2 equipScrollPos = Vector2.zero;
        private void Hash(ref uint hash, string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                hash = (hash << 5) + hash + str[i];
            }
        }

        private int ConvertPart(int pos)
        {
            switch (pos)
            {
                case 0:
                    return (int)EPartType.EHeadgear;
                case 1:
                    return (int)EPartType.EUpperBody;
                case 2:
                    return (int)EPartType.ELowerBody;
                case 3:
                    return (int)EPartType.EGloves;
                case 4:
                    return (int)EPartType.EBoots;
                case 5:
                    return (int)EPartType.EMainWeapon;
                case 6:
                    return (int)EPartType.ESecondaryWeapon;
                case 7:
                    return (int)EPartType.EWings;
                case 8:
                    return (int)EPartType.ETail;
                case 9:
                    return (int)EPartType.EDecal;
                case 10:
                    return (int)EPartType.EFace;
                case 11:
                    return (int)EPartType.EHair;
            }
            return -1;
        }

        private ThreePart FindThreePart(int id, List<ThreePart> lst)
        {
            for (int i = 0; i < lst.Count; ++i)
            {
                ThreePart tp = lst[i];
                if (tp.id == id)
                {
                    return tp;
                }
            }
            ThreePart newTp = new ThreePart();
            newTp.id = id;
            lst.Add(newTp);
            return newTp;
        }

        private void MakeEquip(string name, int[] fashionIDs, List<EquipPart>[] equipList, TempEquipSuit[] tmpFashionData, int suitID, int prefassion = -1)
        {
            if (fashionIDs != null)
            {
                foreach (TempEquipSuit suit in tmpFashionData)
                {
                    suit.hash = 0;
                    suit.data.Clear();
                }
                bool threePart = false;
                for (int i = 0; i < fashionIDs.Length; ++i)
                {
                    int fashionID = fashionIDs[i];
                    FashionList.RowData row = null;// fashionList.GetByItemID(fashionID);
                    if (row != null)
                    {
                        for (int j = 0; j < fashionTypeField.Length; ++j)
                        {
                            FieldInfo fi = fashionTypeField[j];
                            List<ThreePart> tpLst = m_ThreePartList[j];
                            if (row.EquipPos == 7 || row.EquipPos == 8 || row.EquipPos == 9)
                            {
                                ThreePart tp = FindThreePart(suitID, tpLst);
                                if (row.EquipPos == 9)
                                {
                                    string path = fi.GetValue(row) as string;
                                    tp.part[2] = path;
                                }
                                threePart = true;
                            }
                            else
                            {
                                TempEquipSuit suit = tmpFashionData[j];
                                if (row.ReplaceID != null && j < row.ReplaceID.Length)
                                {
                                    FashionList.RowData replace = null;// fashionList.GetByItemID(row.ReplaceID[j]);
                                    if (replace != null)
                                    {
                                        if (replace.EquipPos == row.EquipPos) row = replace;
                                    }
                                }
                                string path = fi.GetValue(row) as string;
                                if (!string.IsNullOrEmpty(path))
                                {
                                    Hash(ref suit.hash, path);
                                    TempEquipData data = new TempEquipData();
                                    data.row = row;
                                    data.path = path;
                                    suit.data.Add(data);
                                }
                            }
                        }
                    }
                }
                if (threePart) return;
                for (int i = 0; i < equipList.Length; ++i)
                {
                    bool findSame = false;
                    List<EquipPart> equips = equipList[i];
                    TempEquipSuit suit = tmpFashionData[i];
                    if (suit.hash == 0 || prefassion != -1 && prefassion != i) continue;
                    for (int j = 0; j < equips.Count; ++j)
                    {
                        EquipPart part = equips[j];
                        if (part != null && part.hash == suit.hash)
                        {
                            part.suitName.Add(name);
                            findSame = true;
                            break;
                        }
                    }
                    if (!findSame)
                    {
                        EquipPart part = new EquipPart();
                        part.hash = suit.hash;
                        part.suitName.Add(name);
                        for (int j = 0; j < suit.data.Count; ++j)
                        {
                            TempEquipData data = suit.data[j];
                            int partPos = ConvertPart(data.row.EquipPos);
                            if (partPos < part.partPath.Length)
                            {
                                part.partPath[partPos] = data.path;
                            }
                            else if (partPos == part.partPath.Length)
                            {
                                part.mainWeapon = data.path;
                            }
                        }
                        equips.Add(part);
                    }
                }
            }
        }

        private string GetDefaultPath(EPartType part, DefaultEquip.RowData data)
        {
            string partPath = "";
            switch (part)
            {
                case EPartType.EFace:
                    partPath = data.Face;
                    break;
                case EPartType.EHair:
                    partPath = data.Hair;
                    break;
                case EPartType.EUpperBody:
                    partPath = data.Body;
                    break;
                case EPartType.ELowerBody:
                    partPath = data.Leg;
                    break;
                case EPartType.EGloves:
                    partPath = data.Glove;
                    break;
                case EPartType.EBoots:
                    partPath = data.Boots;
                    break;
                case EPartType.ESecondaryWeapon:
                    partPath = data.SecondWeapon;
                    break;
            }
            return partPath;
        }

        private void Preview(EquipPart part)
        {
            List<CombineInstance> ciList = new List<CombineInstance>();
            System.Object[] meshPrefab = new System.Object[8];
            DefaultEquip.RowData data = null;// defaultEquip.GetByProfID(m_professionIndex + 1);
            bool hasOnepart = false;
            bool cutout = true;
            string name = "";
            for (int i = 0; i < part.partPath.Length; ++i)
            {
                string path = part.partPath[i];
                CombineInstance ci = new CombineInstance();
                if (string.IsNullOrEmpty(path))
                {
                    path = GetDefaultPath((EPartType)i, data);
                }
                else if (name == "")
                {
                    int index = path.LastIndexOf("_");
                    if (index >= 0)
                    {
                        name = path.Substring(0, index);
                    }
                }
                if (!string.IsNullOrEmpty(path))
                {
                    path = "Equipments/" + path;
                    XMeshTexData mtd = Resources.Load<XMeshTexData>(path);
                    if (mtd != null && mtd.mesh != null)
                    {
                        ci.mesh = mtd.mesh;
                        meshPrefab[i] = mtd;

                    }
                    else
                    {
                        XMeshMultiTexData mmtd = Resources.Load<XMeshMultiTexData>(path);
                        if (mmtd != null && mmtd.mesh != null)
                        {
                            ci.mesh = mmtd.mesh;
                            meshPrefab[i] = mmtd;
                            hasOnepart = true;
                            if (mmtd.tex1 == null)
                            {
                                cutout = false;
                            }
                        }
                    }
                    if (ci.mesh != null)
                    {
                        ciList.Add(ci);
                    }
                }
            }

            if (ciList.Count > 0)
            {
                Material mat = null;
                if (hasOnepart)
                {
                    if (cutout)
                    {
                        mat = new Material(Shader.Find("Custom/Skin/RimlightBlendCutout"));
                    }
                    else
                    {
                        mat = new Material(Shader.Find("Custom/Skin/RimlightBlendCutout"));
                    }
                    XMeshTexData face = meshPrefab[0] as XMeshTexData;
                    if (face != null)
                        mat.SetTexture("_Face", face.tex);
                    XMeshTexData hair = meshPrefab[1] as XMeshTexData;
                    if (hair != null)
                        mat.SetTexture("_Hair", hair.tex);
                    XMeshMultiTexData mmtd = meshPrefab[2] as XMeshMultiTexData;
                    if (hair != null)
                    {
                        mat.SetTexture("_Body", mmtd.tex0);
                        if (cutout) mat.SetTexture("_Alpha", mmtd.tex1);
                    }
                }
                else
                {
                    mat = new Material(Shader.Find("Custom/Skin/RimlightBlend8"));
                    for (int i = 0; i < meshPrefab.Length; ++i)
                    {
                        System.Object obj = meshPrefab[i];
                        if (obj is XMeshTexData)
                        {
                            XMeshTexData mtd = obj as XMeshTexData;
                            mat.SetTexture("_Tex" + i.ToString(), mtd.tex);
                        }
                    }
                }

                string skinPrfab = "Prefabs/" + combineConfig.PrefabName[m_professionIndex];
                string anim = combineConfig.IdleAnimName[m_professionIndex];
                GameObject newGo = GameObject.Instantiate(Resources.Load<UnityEngine.Object>(skinPrfab)) as GameObject;
                if (name != "") newGo.name = name;
                Animator ator = newGo.GetComponent<Animator>();
                AnimatorOverrideController aoc = new AnimatorOverrideController();
                aoc.runtimeAnimatorController = ator.runtimeAnimatorController;
                ator.runtimeAnimatorController = aoc;
                aoc["Idle"] = Resources.Load<AnimationClip>(anim);
                Transform t = newGo.transform.FindChild("CombinedMesh");
                SkinnedMeshRenderer newSmr = t.GetComponent<SkinnedMeshRenderer>();
                newSmr.sharedMesh = new Mesh();
                newSmr.sharedMesh.CombineMeshes(ciList.ToArray(), true, false);
                newSmr.sharedMaterial = mat;

                if (data.WeaponPoint != null && data.WeaponPoint.Length > 0)
                {
                    string weapon = data.WeaponPoint[0].ToString();
                    Transform trans = newGo.transform.Find(weapon);
                    if (trans != null)
                    {
                        string path = part.mainWeapon;
                        if (string.IsNullOrEmpty(path))
                        {
                            path = data.Weapon;
                        }
                        GameObject mainWeapon = Resources.Load<GameObject>("Equipments/" + path);
                        if (mainWeapon != null)
                        {
                            GameObject instance = GameObject.Instantiate(mainWeapon) as GameObject;
                            instance.transform.parent = trans;
                            instance.transform.localPosition = Vector3.zero;
                            instance.transform.localRotation = Quaternion.identity;
                            instance.transform.localScale = Vector3.one;
                        }
                    }
                }
            }
        }

        public void Init()
        {
            combineConfig = FbxEditor.GetConfig();
            Type t = typeof(FashionList.RowData);
            FieldInfo[] fields = t.GetFields();
            fashionTypeField = new FieldInfo[combineConfig.FashionListColumn.Length];
            TempEquipSuit[] fashions = new TempEquipSuit[combineConfig.FashionListColumn.Length];
            m_FashionList = new List<EquipPart>[combineConfig.FashionListColumn.Length];
            m_EquipList = new List<EquipPart>[combineConfig.FashionListColumn.Length];
            m_ThreePartList = new List<ThreePart>[combineConfig.FashionListColumn.Length];
            for (int i = 0; i < combineConfig.FashionListColumn.Length; ++i)
            {
                fashions[i] = new TempEquipSuit();
                m_FashionList[i] = new List<EquipPart>();
                m_EquipList[i] = new List<EquipPart>();
                m_ThreePartList[i] = new List<ThreePart>();
            }
            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo fi = fields[i];
                for (int j = 0; j < combineConfig.FashionListColumn.Length; ++j)
                {
                    string equipPathColumn = combineConfig.FashionListColumn[j];
                    if (equipPathColumn == fi.Name)
                    {
                        fashionTypeField[j] = fi;
                        break;
                    }
                }
            }
        }

        protected virtual void OnGUI()
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < combineConfig.EquipFolderName.Length; ++i)
            {
                if (GUILayout.Button(combineConfig.EquipFolderName[i], GUILayout.MaxWidth(100)))
                {
                    m_professionIndex = i;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            //时装
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("时装", GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
            fashionScrollPos = GUILayout.BeginScrollView(fashionScrollPos, false, false);
            List<EquipPart> currentPrefession = m_FashionList[m_professionIndex];
            for (int i = 0; i < currentPrefession.Count; ++i)
            {
                EquipPart part = currentPrefession[i];
                for (int j = 0; j < part.suitName.Count; ++j)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(part.suitName[j], GUILayout.MaxWidth(150));
                    if (j == 0)
                    {
                        if (GUILayout.Button("Preview", GUILayout.MaxWidth(100)))
                        {
                            Preview(part);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            //装备
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("装备", GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
            equipScrollPos = GUILayout.BeginScrollView(equipScrollPos, false, false);
            List<EquipPart> currentEquipPrefession = m_EquipList[m_professionIndex];
            for (int i = 0; i < currentEquipPrefession.Count; ++i)
            {
                EquipPart part = currentEquipPrefession[i];
                for (int j = 0; j < part.suitName.Count; ++j)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(part.suitName[j], GUILayout.MaxWidth(200));
                    if (j == 0)
                    {
                        if (GUILayout.Button("Preview", GUILayout.MaxWidth(100)))
                        {
                            Preview(part);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

    }
}