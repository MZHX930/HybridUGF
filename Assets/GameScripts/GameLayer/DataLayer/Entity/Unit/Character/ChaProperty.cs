using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    ///<summary>
    ///角色的数值属性部分，比如最大hp、攻击力等等都在这里
    ///这个建一个结构是因为并非只有角色有这些属性，包括装备、buff、aoe、damageInfo等都会用上
    ///</summary>
    public struct ChaProperty
    {
        ///<summary>
        ///最大生命，基本都得有，哪怕角色只有1，装备可以是0
        ///</summary>
        public int HP;

        ///<summary>
        ///最大魔法值
        ///</summary>
        public int MP;

        ///<summary>
        ///攻击力
        ///</summary>
        public int Attack;

        ///<summary>
        ///移动速度，他不是米/秒作为单位的，而是一个可以培养的数值。
        ///具体转化为米/秒，是需要一个规则的，所以是策划脚本 int SpeedToMoveSpeed(int speed)来返回
        ///</summary>
        public int MoveSpeedFactor;

        ///<summary>
        ///行动速度，和移动速度不同，他是增加角色行动速度，也就是变化timeline和动画播放的scale的，比如wow里面开嗜血就是加行动速度
        ///具体多少也不是一个0.2f（我这个游戏中规则设定的最快为正常速度的20%，你的游戏你自己定）到5.0f（我这个游戏设定了最慢是正常速度20%），和移动速度一样需要脚本接口返回策划公式
        ///动画播放速率
        ///</summary>
        public int ActionSpeedFactor;

        // ///<summary>
        // ///体型圆形半径，用于移动碰撞的，单位：米
        // ///这个属性因人而异，但是其实在玩法中几乎不可能经营它，只有buff可能会改变一下，所以直接用游戏中用的数据就行了，不需要转化了
        // ///</summary>
        // public float bodyRadius;

        // ///<summary>
        // ///挨打圆形半径，同体型圆形，只是用途不同，用在判断子弹是否命中的时候
        // ///</summary>
        // public float hitRadius;

        // ///<summary>
        // ///角色移动类型
        // ///</summary>
        // public MoveType moveType;

        public ChaProperty(int moveSpeed, int hp = 0, int mp = 0, int attack = 0, int actionSpeed = 1)
        {
            this.MoveSpeedFactor = moveSpeed;
            this.HP = hp;
            this.MP = mp;
            this.Attack = attack;
            this.ActionSpeedFactor = actionSpeed;
            // this.bodyRadius = bodyRadius;
            // this.hitRadius = hitRadius;
            // this.moveType = moveType;
        }

        public static ChaProperty zero = new ChaProperty(0, 0, 0, 0, 0);

        ///<summary>
        ///将所有值清0
        ///<param name="moveType">移动类型设置为</param>
        ///</summary>
        public void Zero(MoveType moveType = MoveType.ground)
        {
            this.HP = 0;
            this.MoveSpeedFactor = 0;
            this.MP = 0;
            this.Attack = 0;
            this.ActionSpeedFactor = 0;
            // this.bodyRadius = 0;
            // this.hitRadius = 0;
            // this.moveType = moveType;
        }

        //定义加法和乘法的用法，其实这个应该走脚本函数返回，抛给脚本函数多个ChaProperty，由脚本函数运作他们的运算关系，并返回结果
        public static ChaProperty operator +(ChaProperty a, ChaProperty b)
        {
            return new ChaProperty(
                a.MoveSpeedFactor + b.MoveSpeedFactor,
                a.HP + b.HP,
                a.MP + b.MP,
                a.Attack + b.Attack,
                a.ActionSpeedFactor + b.ActionSpeedFactor
            );
        }
        public static ChaProperty operator *(ChaProperty a, ChaProperty b)
        {
            return new ChaProperty(
                Mathf.RoundToInt(a.MoveSpeedFactor * (1.0000f + Mathf.Max(b.MoveSpeedFactor, -0.9999f))),
                Mathf.RoundToInt(a.HP * (1.0000f + Mathf.Max(b.HP, -0.9999f))),
                Mathf.RoundToInt(a.MP * (1.0000f + Mathf.Max(b.MP, -0.9999f))),
                Mathf.RoundToInt(a.Attack * (1.0000f + Mathf.Max(b.Attack, -0.9999f))),
                Mathf.RoundToInt(a.ActionSpeedFactor * (1.0000f + Mathf.Max(b.ActionSpeedFactor, -0.9999f)))
            );
        }
        public static ChaProperty operator *(ChaProperty a, float b)
        {
            return new ChaProperty(
                Mathf.RoundToInt(a.MoveSpeedFactor * b),
                Mathf.RoundToInt(a.HP * b),
                Mathf.RoundToInt(a.MP * b),
                Mathf.RoundToInt(a.Attack * b),
                Mathf.RoundToInt(a.ActionSpeedFactor * b)
            );
        }
    }

    ///<summary>
    ///角色的资源类属性，比如hp，mp等都属于这个
    ///</summary>
    public class ChaResource
    {
        ///<summary>
        ///当前生命
        ///</summary>
        public int HP;

        ///<summary>
        ///魔力值
        ///</summary>
        public int MP;

        ///<summary>
        ///当前耐力，耐力是一个百分比消耗，实时恢复的概念，所以上限按规则就是100了，这里是现有多少
        ///</summary>
        public int Stamina;

        public ChaResource(int hp, int ammo = 0, int stamina = 0)
        {
            this.HP = hp;
            this.MP = ammo;
            this.Stamina = stamina;
        }

        ///<summary>
        ///是否足够
        ///</summary>
        public bool Enough(ChaResource requirement)
        {
            return (
                this.HP >= requirement.HP &&
                this.MP >= requirement.MP &&
                this.Stamina >= requirement.Stamina
            );
        }

        public static ChaResource operator +(ChaResource a, ChaResource b)
        {
            return new ChaResource(
                a.HP + b.HP,
                a.MP + b.MP,
                a.Stamina + b.Stamina
            );
        }
        public static ChaResource operator *(ChaResource a, float b)
        {
            return new ChaResource(
                Mathf.FloorToInt(a.HP * b),
                Mathf.FloorToInt(a.MP * b),
                Mathf.FloorToInt(a.Stamina * b)
            );
        }
        public static ChaResource operator *(float a, ChaResource b)
        {
            return new ChaResource(
                Mathf.FloorToInt(b.HP * a),
                Mathf.FloorToInt(b.MP * a),
                Mathf.FloorToInt(b.Stamina * a)
            );
        }
        public static ChaResource operator -(ChaResource a, ChaResource b)
        {
            return a + b * (-1);
        }

        public static ChaResource Null = new ChaResource(0);
    }
}