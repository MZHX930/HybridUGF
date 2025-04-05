using GameFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public static class GMMenuContainer
    {
        [MenuItem("GM/清理存档")]
        private static void ClearArchive()
        {
            if (Application.isPlaying)
                return;

            PlayerPrefs.DeleteAll();

            string filePath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, DefaultSettingHelper.SettingFileName));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        [MenuItem("GM/清理存档", validate = true)]
        private static bool ClearArchiveValidate()
        {
            return !Application.isPlaying;
        }

        [MenuItem("GM/显示or隐藏碰撞范围")]
        private static void ShowColliderRange()
        {
            EditorPrefs.SetBool("ShowColliderRange", !EditorPrefs.GetBool("ShowColliderRange"));
            Debug.Log($"显示or隐藏碰撞范围: {EditorPrefs.GetBool("ShowColliderRange")}");
        }

        [MenuItem("GM/添加所有道具数量一万")]
        private static void AddAllGameProps10000()
        {
            if (Application.isPlaying)
            {
                foreach (var item in GameEntry.DataTable.GetDataTable<DRDefineGameProps>().GetAllDataRows())
                {
                    GameEntry.GameArchive.ModifyGamePropsCount(item.Id, 10000, true);
                }
            }
        }


        [MenuItem("GM/添加战斗经验值")]
        private static void AddBattleExp()
        {
            if (Application.isPlaying)
            {
                GameEntry.BDH.ModifyOptionExp(20);
            }
        }
    }
}