using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 配置表运行时解析工具
    /// </summary>
    public static class DataTableRuntimeParseTools
    {
        public static readonly char[] DataSplitSeparators = new char[] { '\t' };
        public static readonly char[] DataTrimSeparators = new char[] { '\"' };

        public static Color32 ParseColor32(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Color32(byte.Parse(splitedValue[0]), byte.Parse(splitedValue[1]), byte.Parse(splitedValue[2]), byte.Parse(splitedValue[3]));
        }

        public static Color ParseColor(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Color(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static Quaternion ParseQuaternion(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Quaternion(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static Rect ParseRect(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Rect(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static Vector2 ParseVector2(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Vector2(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]));
        }

        public static Vector3 ParseVector3(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Vector3(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]));
        }

        public static Vector4 ParseVector4(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Vector4(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static bool[] ParseBoolArray(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new bool[] { };

            string[] strArray = value.Split(',');
            bool[] boolArray = new bool[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
                boolArray[i] = bool.Parse(strArray[i]);
            return boolArray;
        }

        public static Vector2Int ParseVector2Int(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Vector2Int(int.Parse(splitedValue[0]), int.Parse(splitedValue[1]));
        }

        public static int[] ParseIntArray(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return new int[] { };

                string[] splitedValue = value.Split(',');
                int[] parsedValue = new int[splitedValue.Length];
                for (int i = 0; i < splitedValue.Length; i++)
                    parsedValue[i] = int.Parse(splitedValue[i]);
                return parsedValue;
            }
            catch (Exception e)
            {
                throw new Exception($"解析Int32时出错={value}", e);
            }
        }

        public static long[] ParseLongArray(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new long[] { };

            string[] splitedValue = value.Split(',');
            long[] parsedValue = new long[splitedValue.Length];
            for (int i = 0; i < splitedValue.Length; i++)
                parsedValue[i] = long.Parse(splitedValue[i]);
            return parsedValue;
        }

        public static float[] ParseFloatArray(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new float[] { };

            string[] splitedValue = value.Split(',');
            float[] parsedValue = new float[splitedValue.Length];
            for (int i = 0; i < splitedValue.Length; i++)
                parsedValue[i] = float.Parse(splitedValue[i]);
            return parsedValue;
        }

        public static string[] ParseStringArray(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new string[] { };
            return value.Split(',');
        }

        public static Dictionary<string, string> ParseDicStrStr(string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                return new Dictionary<string, string>();

            string[] splitedValue = value.Split(';');
            Dictionary<string, string> parsedValue = new Dictionary<string, string>();
            foreach (var item in splitedValue)
            {
                string[] keyValue = item.Split(':');
                parsedValue.Add(keyValue[0], keyValue[1]);
            }
            return parsedValue;
        }


        #region 解析枚举

        public static EnumGameFightTag ParseEnumGameFightTag(string value)
        {
            if (string.IsNullOrEmpty(value))
                return EnumGameFightTag.None;

            if (int.TryParse(value, out int enumValue))
            {
                if (Enum.IsDefined(typeof(EnumGameFightTag), enumValue))
                    return (EnumGameFightTag)enumValue;
            }
            else if (Enum.TryParse<EnumGameFightTag>(value, true, out EnumGameFightTag result))
            {
                return result;
            }

            throw new Exception($"GameFightTagEnum枚举转换失败: {value}");
        }

        public static EnumGameFightTag[] ParseEnumGameFightTagArray(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new EnumGameFightTag[0];

            string[] splitedValue = value.Split(',');
            EnumGameFightTag[] resultArray = new EnumGameFightTag[splitedValue.Length];

            for (int i = 0; i < splitedValue.Length; i++)
            {
                resultArray[i] = ParseEnumGameFightTag(splitedValue[i]);
            }
            return resultArray;
        }

        /// <summary>
        /// 解析EnumCharacterTag
        /// </summary>
        public static EnumCharacterTag ParseEnumCharacterTag(string value)
        {
            if (string.IsNullOrEmpty(value))
                return EnumCharacterTag.Normal;

            if (int.TryParse(value, out int enumValue))
            {
                if (Enum.IsDefined(typeof(EnumCharacterTag), enumValue))
                    return (EnumCharacterTag)enumValue;
            }
            else if (Enum.TryParse<EnumCharacterTag>(value, true, out EnumCharacterTag result))
            {
                return result;
            }

            throw new Exception($"CharacterTagEnum枚举转换失败: {value}");
        }

        /// <summary>
        /// 解析EnumCharacterTag[]
        /// </summary>
        public static EnumCharacterTag[] ParseEnumCharacterTagArray(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new EnumCharacterTag[0];

            string[] splitedValue = value.Split(',');
            EnumCharacterTag[] resultArray = new EnumCharacterTag[splitedValue.Length];

            for (int i = 0; i < splitedValue.Length; i++)
            {
                resultArray[i] = ParseEnumCharacterTag(splitedValue[i]);
            }
            return resultArray;
        }


        public static EnumTutorialTriggerType ParseEnumTutorialTriggerType(string value)
        {
            if (int.TryParse(value, out int enumValue))
            {
                if (Enum.IsDefined(typeof(EnumTutorialTriggerType), enumValue))
                    return (EnumTutorialTriggerType)enumValue;
            }
            else if (Enum.TryParse<EnumTutorialTriggerType>(value, true, out EnumTutorialTriggerType result))
            {
                return result;
            }

            throw new Exception($"EnumTutorialTriggerType枚举转换失败: value={value}");
        }
        #endregion


        #region 解析委托事件
        public static LogicTimeLineNodeEvent ParseLogicTimeLineNodeEvent(string value)
        {
            if (LogicTimeLineNodeEventFactory.Functions.TryGetValue(value, out var func))
            {
                return func;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"LogicTimeLineNodeEvent解析失败: {value}");
            return null;
        }

        public static BulletOnCreate ParseBulletOnCreate(string value)
        {
            if (BulletSkillDelegateFactory.OnCreateFuncDic.TryGetValue(value, out var func))
            {
                return func;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"BulletOnCreate解析失败: {value}");
            return null;
        }

        public static BulletOnHit ParseBulletOnHit(string value)
        {
            if (BulletSkillDelegateFactory.OnHitFuncDic.TryGetValue(value, out var func))
            {
                return func;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"BulletOnHit解析失败: {value}");
            return null;
        }

        public static BulletOnRemoved ParseBulletOnRemoved(string value)
        {
            if (BulletSkillDelegateFactory.OnRemovedFuncDic.TryGetValue(value, out var func))
            {
                return func;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"BulletOnRemoved解析失败: {value}");
            return null;
        }

        public static BulletTween ParseBulletTween(string value)
        {
            if (BulletSkillDelegateFactory.BulletTweenDic.TryGetValue(value, out var func))
            {
                return func;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"BulletTween解析失败: {value}");
            return null;
        }

        public static AoeOnCreate ParseAoeOnCreate(string value)
        {
            if (AoeSkillDelegateFactory.OnCreateFuncDic.TryGetValue(value, out var func))
            {
                return func;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"AoeOnCreate解析失败: {value}");
            return null;
        }

        public static AoeOnRemoved ParseAoeOnRemoved(string value)
        {
            if (AoeSkillDelegateFactory.OnRemovedFuncDic.TryGetValue(value, out var func))
            {
                return func;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"AoeOnRemoved解析失败: {value}");
            return null;
        }

        public static AoeOnTick ParseAoeOnTick(string value)
        {
            if (AoeSkillDelegateFactory.OnTickFuncDic.TryGetValue(value, out var func))
            {
                return func;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"AoeOnTick解析失败: {value}");
            return null;
        }

        public static AoeOnCharacterEnter ParseAoeOnCharacterEnter(string value)
        {
            if (AoeSkillDelegateFactory.OnChaEnterFuncDic.TryGetValue(value, out var func))
            {
                return func;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"AoeOnCharacterEnter解析失败: {value}");
            return null;
        }

        public static AoeOnCharacterLeave ParseAoeOnCharacterLeave(string value)
        {
            if (AoeSkillDelegateFactory.OnChaLeaveFuncDic.TryGetValue(value, out var func))
            {
                return func;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"AoeOnCharacterLeave解析失败: {value}");
            return null;
        }

        // public static AoeOnBulletEnter ParseAoeOnBulletEnter(string value)
        // {
        //     return AoeSkillDelegateFactory.OnBulletEnterFuncDic[value];
        // }

        // public static AoeOnBulletLeave ParseAoeOnBulletLeave(string value)
        // {
        //     return AoeSkillDelegateFactory.OnBulletLeaveFuncDic[value];
        // }

        public static AoeMoveTween ParseAoeMoveTween(string value)
        {
            if (AoeSkillDelegateFactory.AoeTweenFuncDic.TryGetValue(value, out var tween))
            {
                return tween;
            }
            if (!string.IsNullOrEmpty(value))
                Debug.LogError($"AoeMoveTween解析失败: {value}");
            return null;
        }

        #endregion


        #region 解析Var变量
        public static VarInt32 ParseVarInt32(string value)
        {
            return int.Parse(value);
        }

        public static VarInt32[] ParseVarInt32Array(string value)
        {
            int[] result = ParseIntArray(value);
            VarInt32[] result2 = new VarInt32[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result2[i] = result[i];
            }
            return result2;
        }
        #endregion

    }
}