using UnityEngine;

namespace GameDevScript
{
    public static partial class Constant
    {
        /// <summary>
        /// 层。
        /// </summary>
        public static class Layer
        {
            public const string DefaultLayerName = "Default";
            public static readonly int DefaultLayerId = LayerMask.NameToLayer(DefaultLayerName);

            public const string UILayerName = "UI";
            public static readonly int UILayerId = LayerMask.NameToLayer(UILayerName);

            public const string TargetableObjectLayerName = "Targetable Object";
            public static readonly int TargetableObjectLayerId = LayerMask.NameToLayer(TargetableObjectLayerName);



            public const string Vehicle = "Vehicle";
            public static readonly int VehicleLayerId = LayerMask.NameToLayer(Vehicle);

            public const string Solider = "Solider";
            public static readonly int SoliderLayerId = LayerMask.NameToLayer(Solider);

            public const string SoliderSeat = "SoliderSeat";
            public static readonly int SoliderSeatLayerId = LayerMask.NameToLayer(SoliderSeat);

            public const string EntityPlayer = "EntityPlayer";
            public static readonly int EntityPlayerLayerId = LayerMask.NameToLayer(EntityPlayer);

            public const string EntityEnemy = "EntityEnemy";
            public static readonly int EntityEnemyLayerId = LayerMask.NameToLayer(EntityEnemy);

            public const string SkillPlayer = "SkillPlayer";
            public static readonly int SkillPlayerLayerId = LayerMask.NameToLayer(SkillPlayer);

            public const string SkillEnemy = "SkillEnemy";
            public static readonly int SkillEnemyLayerId = LayerMask.NameToLayer(SkillEnemy);
        }
    }
}
