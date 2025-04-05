using Cysharp.Threading.Tasks;
using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 场景实体辅助类
    /// </summary>
    public static class SceneEntityHelper
    {
        /// <summary>
        /// 当前激活的英雄单位
        /// </summary>
        public static UnitCharacterCtrl CurActivedHero;
        /// <summary>
        /// 当前激活的载具单位
        /// </summary>
        public static UnitCharacterCtrl CurActivedVehicle;

        private static void CreateShellEntity(
            int entitySerialId,
            string shellAssetPath,
            string entityGroup,
            int assetPriority,
            IUnitCtrlData ctrlData,
            string viewName,
            int viewLoadPriority,
            Action<ShellEntityLogic> onShowShellAction,
            Action<ShellEntityLogic> onShowViewAction
        )
        {
            var data = ShellEntityData.Create();
            data.UnitCtrlData = ctrlData;
            data.ViewName = viewName;
            data.ViewLoadPriority = viewLoadPriority;
            data.OnShowShellAction = onShowShellAction;
            data.OnShowViewAction = onShowViewAction;

            GameEntry.Entity.ShowEntity<ShellEntityLogic>(
                entitySerialId,
                shellAssetPath,
                entityGroup,
                assetPriority,
                data
            );
        }

        /// <summary>
        /// 在当前场景中添加主角
        /// </summary>
        public static void CreateHeroEntity(int entitySerialId, int dtId, Action<ShellEntityLogic> onShowShellAction, Action<ShellEntityLogic> onShowViewAction)
        {
            DRDefineHero configData = GameEntry.DataTable.GetDataTable<DRDefineHero>().GetDataRow(dtId);

            //创建角色运行时数据
            var serverData = UnitCharacterCtrlData.Create();
            serverData.DtId = dtId;
            serverData.GameSide = GameSideEnum.Player;
            serverData.ChaType = CharacterTypeEnum.Hero;
            serverData.RarityType = (CharacterRarityEnum)configData.Rarity;
            serverData.CanMove = configData.CanMove;
            serverData.CanRotate = configData.CanRotate;
            serverData.BornWorldPos = default;
            serverData.Level = 1;
            serverData.InitBaseProp(new ChaProperty()
            {
                HP = configData.HP,
                MP = configData.MP,
                Attack = configData.Attack,
                MoveSpeedFactor = configData.MoveSpeedFactor,
                ActionSpeedFactor = configData.ActionSpeedFactor
            });

            //创建壳数据
            CreateShellEntity(
                entitySerialId,
                AssetPathUtility.GetHeroShellPath(),
                Constant.EntityGroup.Player,
                Constant.AssetPriority.CharacterShellAsset,
                serverData,
                configData.ViewPath,
                Constant.AssetPriority.CharacterViewAsset,
                onShowShellAction,
                onShowViewAction
            );
        }

        /// <summary>
        /// 创建怪物实体
        /// </summary>
        /// <param name="dtId">怪物模板ID</param>
        /// <param name="bornPos">出生位置</param>
        /// <returns>怪物实体序列ID</returns>
        public static void CreateMonsterEntity(int entitySerialId, int dtId, ChaProperty baseChaProperty, Vector3 bornPos, Action<ShellEntityLogic> onShowShellAction, Action<ShellEntityLogic> onShowViewAction)
        {
            DRDefineMonster configData = GameEntry.DataTable.GetDataTable<DRDefineMonster>().GetDataRow(dtId);

            //创建角色运行时数据
            var serverData = UnitCharacterCtrlData.Create();
            serverData.DtId = dtId;
            serverData.GameSide = GameSideEnum.Enemy;
            serverData.ChaType = CharacterTypeEnum.Monster;
            serverData.RarityType = (CharacterRarityEnum)configData.Rarity;
            serverData.CanMove = true;
            serverData.CanRotate = true;
            serverData.BornWorldPos = bornPos;
            serverData.InitBaseProp(baseChaProperty);

            CreateShellEntity(
                entitySerialId,
                AssetPathUtility.GetMonsterShellPath(),
                Constant.EntityGroup.Monster,
                Constant.AssetPriority.CharacterShellAsset,
                serverData,
                configData.ViewPath,
                Constant.AssetPriority.CharacterViewAsset,
                onShowShellAction,
                onShowViewAction
            );
        }

        /// <summary>
        /// 创建士兵实体
        /// </summary>
        public static void CreateSoldierEntity(int entitySerialId, UnitCharacterCtrlData ctrlData, string viewName, Action<ShellEntityLogic> onShowShellAction, Action<ShellEntityLogic> onShowViewAction)
        {
            CreateShellEntity(
                entitySerialId,
                AssetPathUtility.GetSoldierShellPath(),
                Constant.EntityGroup.Player,
                Constant.AssetPriority.CharacterShellAsset,
                ctrlData,
                viewName,
                Constant.AssetPriority.CharacterViewAsset,
                onShowShellAction,
                onShowViewAction);
        }

        /// <summary>
        /// 创建载具
        /// </summary>
        public static void CreateVehicleEntity(int entitySerialId, int drId, Action<ShellEntityLogic> onShowShellAction, Action<ShellEntityLogic> onShowViewAction)
        {
            DRDefineVehicle configData = GameEntry.DataTable.GetDataTable<DRDefineVehicle>().GetDataRow(drId);

            var ctrlData = UnitCharacterCtrlData.Create();
            ctrlData.DtId = drId;
            ctrlData.GameSide = GameSideEnum.Player;
            ctrlData.ChaType = CharacterTypeEnum.Vehicle;
            ctrlData.RarityType = (CharacterRarityEnum)configData.Rarity;

            ctrlData.InitBaseProp(new ChaProperty()
            {
                HP = configData.HP,
                MP = configData.MP,
                Attack = configData.Attack,
                MoveSpeedFactor = configData.MoveSpeedFactor,
                ActionSpeedFactor = configData.ActionSpeedFactor
            });

            CreateShellEntity(
                entitySerialId,
                AssetPathUtility.GetVehicleShellPath(),
                Constant.EntityGroup.Player,
                Constant.AssetPriority.CharacterShellAsset,
                ctrlData,
                configData.ViewPath,
                Constant.AssetPriority.CharacterViewAsset,
                onShowShellAction,
                onShowViewAction
            );
        }


        /// <summary>
        /// 创建子弹实体
        /// </summary>
        public static void CreateBulletShell(int entitySerialId, UnitBulletCtrlData launcherData)
        {
            CreateShellEntity(
                entitySerialId,
                AssetPathUtility.GetBulletShellPath(),
                Constant.EntityGroup.Skill,
                Constant.AssetPriority.SkillShellAsset,
                launcherData,
                launcherData.Model.Prefab,
                Constant.AssetPriority.SkillViewAsset,
                null,
                null
            );
        }

        /// <summary>
        /// 创建Aoe特效实体
        /// </summary>
        public static void CreateAoeShell(int entitySerialId, UnitAoECtrlData LauncherData)
        {
            CreateShellEntity(
                entitySerialId,
                AssetPathUtility.GetAoEShellPath(),
                Constant.EntityGroup.Skill,
                Constant.AssetPriority.SkillShellAsset,
                LauncherData,
                LauncherData.Model.Prefab,
                Constant.AssetPriority.SkillViewAsset,
                null,
                null
            );
        }

        public static void CreateBuildingShell(int entitySerialId, int drId, Vector3 bornWorldPos, Action<ShellEntityLogic> onShowShellAction, Action<ShellEntityLogic> onShowViewAction)
        {
            DRDefineBuilding dtrData = GameEntry.DataTable.GetDataTable<DRDefineBuilding>().GetDataRow(drId);

            var ctrlData = UnitBuildingCtrlData.Create();
            ctrlData.DtId = drId;
            ctrlData.BornWorldPos = bornWorldPos;

            CreateShellEntity(
                entitySerialId,
                AssetPathUtility.GetBuildingShellPath(),
                Constant.EntityGroup.Building,
                Constant.AssetPriority.BuildingShellAsset,
                ctrlData,
                dtrData.ViewName,
                Constant.AssetPriority.BuildingViewAsset,
                onShowShellAction,
                onShowViewAction
            );
        }
    }
}
