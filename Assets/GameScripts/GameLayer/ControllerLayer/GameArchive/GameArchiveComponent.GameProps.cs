using UnityEngine;

namespace GameDevScript
{
    /*
    游戏道具存档
    */
    public partial class GameArchiveComponent
    {
        /// <summary>
        /// 获取游戏道具数量
        /// </summary>
        /// <param name="dtId">DefineGameProps表ID</param>
        /// <returns>游戏道具数量</returns>
        public long GetGamePropsCount(int dtId)
        {
            if (Data.GamePropsArchiveData.ContainsKey(dtId))
                return Data.GamePropsArchiveData[dtId];

            return 0;
        }

        /// <summary>
        /// 修改游戏道具数量
        /// </summary>
        /// <param name="dtId">DefineGameProps表ID</param>
        /// <param name="count">数量</param>
        public void ModifyGamePropsCount(int dtId, long count, bool needFire = true)
        {
            long oldCount = GetGamePropsCount(dtId);

            if (Data.GamePropsArchiveData.ContainsKey(dtId))
                Data.GamePropsArchiveData[dtId] += count;
            else
                Data.GamePropsArchiveData[dtId] = count;

            if (needFire)
            {
                GameEntry.Event.Fire(this, GamePropsCountChangeEventArgs.Create(dtId, oldCount, Data.GamePropsArchiveData[dtId]));
            }
        }

        /// <summary>
        /// 检查游戏道具是否足够
        /// </summary>
        /// <param name="dtId">DefineGameProps表ID</param>
        /// <param name="count">数量</param>
        /// <returns>是否足够</returns>
        public bool CheckGamePropsIsEnough(int dtId, long count)
        {
            return GetGamePropsCount(dtId) >= count;
        }
    }
}
