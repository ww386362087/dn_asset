using UnityEngine;
using System.Collections;
//using UnityEditor;

    /// <summary>
    /// 由此脚本生成配置 保存在prefab中
    /// </summary>
public class CombineConfig : MonoBehaviour
{
    public string BodyString;//_body
    public string LegString;//_leg
    public string GloveString;//_glove
    public string BootString;//_boots
    public string HeadString;//_head
    public string FaceString;//_face
    public string HairString;//_hair
    public string HelmetString;//_helmet

    public int professionCount;

    public string[] EquipFolderName;//Warrior,Sorcer,Archer,Cleric,Academic,Assassin
    public string[] SecondaryWeapon;// _gauntlet,_book,_quiver,_shield,_gauntlet,_scimitar
    public string[] BandposeName;//player_warrior_bandpose,player_archer_bandpose,player_sorceress_bandpose,player_cleric_bandpose,player_academic_bandpose,player_assassin_bandpose
    public string[] PrefabName;//ZJ_zhanshi_SkinnedMesh,Player_archer_SkinnedMesh,Player_sorceress_SkinnedMesh,Player_cleric_SkinnedMesh,Player_academic_SkinnedMesh,Player_assassin_SkinnedMesh
    public string[] SkillFolderName;//Player_warrior,Player_archer,Player_sorceress,Player_cleric,Player_academic,Player_assassin
    public string[] IdleAnimName;
    public string[] FashionListColumn;


    public CombineConfig()
    {
        this.BodyString = "_body";
        this.LegString = "_leg";
        this.GloveString = "_glove";
        this.BootString = "_boots";
        this.HeadString = "_head";
        this.FaceString = "_face";
        this.HairString = "_hair";
        this.HelmetString = "_helmet";
        this.professionCount = 6;

        this.EquipFolderName = new string[] {
            "Warrior",
            "Archer", 
            "Sorcer", 
            "Cleric", 
            "Academic", 
            "Assassin" };

        this.SecondaryWeapon = new string[] {
            "_gauntlet", 
            "_quiver", 
            "_book", 
            "_shield", 
            "_gauntlet", 
            "_scimitar" };

        this.BandposeName = new string[] { 
            "player_warrior_bandpose",
            "player_archer_bandpose",
            "player_sorceress_bandpose",
            "player_cleric_bandpose",
            "player_academic_bandpose",
            "player_assassin_bandpose" };

        this.PrefabName = new string[] { 
            "ZJ_zhanshi_SkinnedMesh",
            "Player_archer_SkinnedMesh_createchar",
            "Player_sorceress_SkinnedMesh",
            "Player_cleric_SkinnedMesh",
            "Player_academic_SkinnedMesh",
            "Player_assassin_SkinnedMesh" };

        this.SkillFolderName = new string[] { "Player_warrior", 
            "Player_archer", 
            "Player_sorceress",
            "Player_cleric", 
            "Player_academic",
            "Player_assassin" };

        this.IdleAnimName = new string[] { 
            "Animation/Player_warrior/Player_warrior_idle_normal", 
            "Animation/Player_archer/Player_archer_idle_normal",
            "Animation/Player_sorceress/Player_sorceress_stand_normal",
            "Animation/Player_cleric/Player_cleric_idle_normal", 
            "Animation/Player_academic/Player_academic_idle_normal",
            "Animation/Player_assassin/Player_assassin_idle_normal" };

        this.FashionListColumn = new string[] { 
            "ModelPrefabWarrior",
            "ModelPrefabArcher",
            "ModelPrefabSorcer", 
            "ModelPrefabCleric", 
            "ModelPrefab5", 
            "ModelPrefab6" };
        
    }
}