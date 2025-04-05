using Cysharp.Threading.Tasks;
using GameFramework;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 角色控制器
    /// 1个容器entity包含1个模型entity，1对1的关系
    /// 用于管理角色的所有组件
    /// 这个脚本是个数据中控，然后再去控制各个组件（Move、Spine、Buff、Action、Skill等等）
    /// 通过组合各种组件，实现不同角色"性格"
    /// </summary>
    public sealed class UnitCharacterCtrl : MonoBehaviour, IUnitCtrl<UnitCharacterCtrlData>, IShellShowHandler
    {
        #region 组件
        private ViewContainer m_ViewContainer;
        private UnitMove m_UnitMove;
        private UnitSpineAnim m_UnitSpineAnim;
        private UnitBindManager m_BindPoints;
        private UnitCollider m_UnitCollider;
        private UnitCharacterFightAI m_UnitCharacterFightAI;
        #endregion

        public ShellEntityLogic ShellEntityLogic { get; private set; }

        #region public data
        public IUnitCtrlData BaseCtrlData => CtrlData;
        public UnitCharacterCtrlData CtrlData { get; private set; }

        ///<summary>
        ///角色是否已经死了，这不由我这个系统判断，其他系统应该告诉我
        ///</summary>
        public bool IsDead = false;

        /// <summary>
        /// 是否处于战斗中
        /// </summary>
        public bool IsFighting { get; private set; } = false;

        ///<summary>
        ///根据tags可以判断出这是什么样的人
        ///</summary>
        public string[] tags = new string[0];
        #endregion

        private void Awake()
        {
            m_ViewContainer = GetComponentInChildren<ViewContainer>();
            m_UnitMove = gameObject.GetComponent<UnitMove>();
            m_UnitSpineAnim = gameObject.GetComponent<UnitSpineAnim>();
            m_BindPoints = gameObject.GetComponent<UnitBindManager>();
            m_UnitCollider = gameObject.GetComponent<UnitCollider>();
            m_UnitCharacterFightAI = gameObject.GetComponent<UnitCharacterFightAI>();
        }

        public void OnShowShell(ShellEntityLogic shellEntityLogic)
        {
            IsDead = false;

            ShellEntityLogic = GetComponent<ShellEntityLogic>();
            CtrlData = ShellEntityLogic.ShellData.UnitCtrlData as UnitCharacterCtrlData;

            m_RotateToOrder = transform.rotation.eulerAngles.y;
            transform.position = CtrlData.BornWorldPos;
            IsFighting = false;

            CtrlData.AttrRecheck();

            //学习技能
            if (CtrlData.SkillLauncherIds != null)
            {
                foreach (var launcherId in CtrlData.SkillLauncherIds)
                {
                    LearnSkill(launcherId);
                }
            }
        }

        /// <summary>
        /// 当显示视图模型时
        /// </summary>
        public void OnShowView(PrefabViewEntityLogic viewEntityLogic)
        {
            m_BindPoints?.UpdatePoints();
            m_UnitCollider?.ActiveCollision(CtrlData.GameSide == GameSideEnum.Player ? CollisionLayer.PlayerCha : CollisionLayer.EnemyCha);
            if (m_UnitSpineAnim)
            {
                m_UnitSpineAnim.SetAnimator(viewEntityLogic.GetComponentInChildren<SkeletonAnimation>());
                m_UnitSpineAnim.Play(EnumSpineAnimKey.Idle);
            }

            //激活AI
            m_UnitCharacterFightAI?.ActiveAI();
        }

        public void OnHideShell(ShellEntityLogic shellEntityLogic)
        {
            if (shellEntityLogic != null && shellEntityLogic.Entity.Id != 0)
                GameEntry.Event.Fire(this, HideCharacterShellEventArgs.Create(shellEntityLogic.Entity.Id));
            else
                Log.Error("shellEntityLogic is null or shellEntityLogic.Entity.Id is 0");
        }

        public void OnHideView(PrefabViewEntityLogic viewEntityLogic)
        {
            m_UnitCollider?.RemoveCollision();
            m_UnitSpineAnim.ClearAnimator();

            //注销AI
            m_UnitCharacterFightAI?.DeactiveAI();
        }

        private void Update()
        {
            float elapseSeconds = Time.deltaTime;
            // float realElapseSeconds = Time.unscaledDeltaTime;

            if (IsDead || CtrlData == null)
                return;

            //如果角色没死，做以下事情：
            //无敌时间减少
            if (CtrlData.ImmuneTime > 0)
                CtrlData.SetImmuneTime(CtrlData.ImmuneTime - elapseSeconds);

            //技能冷却时间
            for (int i = 0; i < CtrlData.ChaSkillList.Count; i++)
            {
                if (CtrlData.ChaSkillList[i].CoolDown > 0)
                {
                    CtrlData.ChaSkillList[i].CoolDown -= elapseSeconds;
                }
            }

            //对身上的buff进行管理
            List<BuffRTData> toRemove = new List<BuffRTData>();
            for (int i = 0; i < CtrlData.Buffs.Count; i++)
            {
                if (CtrlData.Buffs[i].IsPermanent == false)
                    CtrlData.Buffs[i].Duration -= elapseSeconds;
                CtrlData.Buffs[i].TimeElapsed += elapseSeconds;

                if (CtrlData.Buffs[i].Model.TickTime > 0 && CtrlData.Buffs[i].Model.OnTick != null)
                {
                    //float取模不精准，所以用x1000后的整数来
                    if (Mathf.RoundToInt(CtrlData.Buffs[i].TimeElapsed * 1000) % Mathf.RoundToInt(CtrlData.Buffs[i].Model.TickTime * 1000) == 0)
                    {
                        CtrlData.Buffs[i].Model.OnTick(CtrlData.Buffs[i]);
                        CtrlData.Buffs[i].Ticked += 1;
                    }
                }

                //只要duration <= 0，不管是否是permanent都移除掉
                if (CtrlData.Buffs[i].Duration <= 0 || CtrlData.Buffs[i].Stack <= 0)
                {
                    if (CtrlData.Buffs[i].Model.OnRemoved != null)
                    {
                        CtrlData.Buffs[i].Model.OnRemoved(CtrlData.Buffs[i]);
                    }
                    toRemove.Add(CtrlData.Buffs[i]);
                }
            }
            if (toRemove.Count > 0)
            {
                for (int i = 0; i < toRemove.Count; i++)
                {
                    CtrlData.Buffs.Remove(toRemove[i]);
                }
                CtrlData.AttrRecheck();
            }

            toRemove = null;
        }

        private void FixedUpdate()
        {
            if (IsDead || CtrlData == null)
                return;

            //首先是合并移动信息，发送给UnitMove。因为是2DXY平面移动，角色无转向。
            bool tryRun = CtrlData.ControlState.CanMove == true && m_MoveOrder != Vector3.zero;
            if (m_UnitMove && tryRun)
            {
                if (CtrlData.ControlState.CanMove == false)
                    m_MoveOrder = Vector3.zero;
                int fmIndex = 0;
                while (fmIndex < m_ForceMove.Count)
                {
                    m_MoveOrder += m_ForceMove[fmIndex].VeloInTime(Time.fixedDeltaTime);
                    if (m_ForceMove[fmIndex].duration <= 0)
                    {
                        m_ForceMove.RemoveAt(fmIndex);
                    }
                    else
                    {
                        fmIndex++;
                    }
                }
                m_UnitMove.MoveBy(m_MoveOrder);
                m_MoveOrder = Vector3.zero;
                m_ForceMove.Clear();
            }

            //处理动画
            if (m_UnitSpineAnim)
            {
                //先计算默认（规则下）的动画，并且添加到动画组
                if (tryRun == false)
                {
                    if (m_AnimOrder.Count <= 0)
                        m_AnimOrder.Add(EnumSpineAnimKey.Idle);    //如果没有要求移动，就用站立
                }
                else
                {
                    m_AnimOrder.Add(EnumSpineAnimKey.Walk);
                }
                //送给动画系统处理
                for (int i = 0; i < m_AnimOrder.Count; i++)
                {
                    m_UnitSpineAnim.Play(m_AnimOrder[i]);
                }
                m_AnimOrder.Clear();
            }
        }

        ///<summary>
        ///杀死这个角色
        ///</summary>
        public async UniTaskVoid Kill()
        {
            IsDead = true;
            float waitTime = 0;
            if (m_UnitSpineAnim)
            {
                waitTime = m_UnitSpineAnim.Play(EnumSpineAnimKey.Dead);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
            GameEntry.Entity.HideEntity(ShellEntityLogic.Entity);
        }

        ///<summary>
        ///是否拥有某个tag
        ///</summary>
        public bool HasTag(string tag)
        {
            if (this.tags == null || this.tags.Length <= 0)
                return false;
            for (int i = 0; i < this.tags.Length; i++)
            {
                if (tags[i] == tag)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 切换战斗状态
        /// </summary>
        /// <param name="isFighting">是否处于战斗中</param>
        public void SwitchFightState(bool isFighting)
        {
            if (IsFighting == isFighting)
                return;

            IsFighting = isFighting;
        }

        /// <summary>
        /// 升级合成等级
        /// </summary>
        public void UpgradeMergeLv(int newMergeLv, string modelPath)
        {
            //更新数据
            CtrlData.MergeLv = newMergeLv;
            // ChaLogicData.ModelPath = modelPath;

            var childEntities = GameEntry.Entity.GetChildEntities(ShellEntityLogic.Entity);
            //隐藏旧Model
            foreach (var childEntity in childEntities)
            {
                if (childEntity.Logic is PrefabViewEntityLogic)
                {
                    GameEntry.Entity.HideEntity(childEntity);
                }
            }
        }

        /// <summary>
        /// 获取技能发射口
        /// </summary>
        /// <param name="portKey">发射口key</param>
        /// <returns>发射口位置</returns>
        public Vector3 GetPortPointWorldPos(string portKey)
        {
            var point = m_BindPoints.GetBindPointByKey(portKey);
            if (point == null)
                return transform.position;
            else
                return point.transform.position;
        }

        /// <summary>
        /// 获取绑定点
        /// </summary>
        public UnitBindPoint GetBindPoint(string key)
        {
            return m_BindPoints.GetBindPointByKey(key);
        }


        #region 移动控制
        /// <summary>
        /// 来自操作或者ai的移动请求信息
        /// </summary>
        private Vector3 m_MoveOrder = new Vector3();

        /// <summary>
        /// 来自强制发生的位移信息，通常是技能效果等导致的，比如翻滚、被推开等
        /// </summary>
        private List<MovePreorder> m_ForceMove = new List<MovePreorder>();

        /// <summary>
        /// 收到的来自各方的播放动画的请求
        /// </summary>
        private List<EnumSpineAnimKey> m_AnimOrder = new List<EnumSpineAnimKey>();

        /// <summary>
        /// 来自操作或者ai的旋转角度请求
        /// </summary>
        private float m_RotateToOrder;

        /// <summary>
        /// 来自强制执行的旋转角度
        /// </summary>
        private List<float> m_ForceRotate = new List<float>();

        ///<summary>
        ///角色移动力，单位：米/秒
        ///</summary>
        public float MoveSpeed
        {
            get
            {
                //这个公式也可以通过给策划脚本接口获得，这里就写代码里了，不走策划脚本了
                //设定，值=0.2+5.6*x/(x+100)，初始速度是100，移动力3米/秒，最小值0.2米/秒。
                return CtrlData.CurProperty.MoveSpeedFactor * 5.600f / (CtrlData.CurProperty.MoveSpeedFactor + 100.000f) + 0.200f;
            }
        }

        ///<summary>
        ///角色行动速度，是一个timescale，最小0.1，初始行动速度值也是100。
        ///</summary>
        public float ActionSpeed
        {
            get
            {
                return CtrlData.CurProperty.ActionSpeedFactor * 4.90f / (CtrlData.CurProperty.ActionSpeedFactor + 390.00f) + 0.100f;
            }
        }

        ///<summary>
        ///命令移动
        ///<param name="move">移动力</param>
        ///</summary>
        public void OrderMove(Vector3 move)
        {
            this.m_MoveOrder.x = move.x;
            this.m_MoveOrder.y = move.y;
            this.m_MoveOrder.z = move.z;
        }

        ///<summary>
        ///强制移动
        ///<param name="moveInfo">移动信息</param>
        ///</summary>
        public void AddForceMove(MovePreorder move)
        {
            this.m_ForceMove.Add(move);
        }

        // ///<summary>
        // ///命令旋转到多少度
        // ///<param name="degree">旋转目标</param>
        // ///</summary>
        // public void OrderRotateTo(float degree)
        // {
        //     this.m_RotateToOrder = degree;
        // }

        // ///<summary>
        // ///强制旋转的力量
        // ///<param name="degree">偏移角度</param>
        // ///</summary>
        // public void AddForceRotate(float degree)
        // {
        //     this.m_ForceRotate.Add(degree);
        // }

        ///<summary>
        ///添加角色要做的动作请求
        ///<param name="animName">要做的动作</param>
        ///</summary>
        public void Play(EnumSpineAnimKey animKey)
        {
            m_AnimOrder.Add(animKey);
        }

        ///<summary>
        ///在角色身上放一个特效，其实是挂在一个gameObject而已
        ///<param name="bindPointKey">绑点名称，角色有Muzzle/Head/Body这3个，需要再加</param>
        ///<param name="effect">要播放的特效文件名，统一走Prefabs/下拿</param>
        ///<param name="effectKey">这个特效的key，要删除的时候就有用了</param>
        ///<param name="effect">要播放的特效</param>
        ///</summary>
        public void PlaySightEffect(string bindPointKey, string effect, string effectKey = "", bool loop = false)
        {
        }

        ///<summary>
        ///删除角色身上的一个特效
        ///<param name="bindPointKey">绑点名称，角色有Muzzle/Head/Body这3个，需要再加</param>
        ///<param name="effectKey">这个特效的key，要删除的时候就有用了</param>
        ///</summary>
        public void StopSightEffect(string bindPointKey, string effectKey)
        {
        }

        // public void SetAnimInfo(Dictionary<string, AnimInfo> animInfo)
        // {
        //     if (m_UnitSpineAnim == null)
        //         return;
        //     m_UnitSpineAnim.AnimInfo = animInfo;
        // }
        #endregion

        ///<summary>
        ///增加角色的血量等资源，直接改变数字的，属于最后一步操作了
        ///<param name="value">要改变的量，负数为减少</param>
        ///</summary>
        public void ModifyResource(ChaResource value)
        {
            CtrlData.ModifyResource(value);
            if (CtrlData.CurResource.HP <= 0)
            {
                Kill().Forget();
            }
        }

        #region 技能
        ///<summary>
        ///释放一个技能，释放技能并不总是成功的，如果你一直发释放技能的命令，那失败率应该是骤增的
        ///<param name="id">要释放的技能的id</param>
        ///<return>是否释放成功</return>
        ///</summary>
        public void AutoCastSkill(int id, Vector3 firePos, Vector3 fireDireNorm, Vector3 targetPos, int targetSerialId)
        {
            if (CtrlData.ControlState.CanUseSkill == false)
            {
                Log.EDebug($"技能未激活,id={id}");
                return;
            }
            SkillLauncherShell launcherData = CtrlData.GetSkillById(id);
            if (launcherData == null || launcherData.CoolDown > 0)
            {
                Log.EDebug($"技能冷却中,id={id}");
                return;
            }
            //检测释放条件
            if (CtrlData.CurResource.Enough(launcherData.Model.Condition) == false)
            {
                Log.EDebug($"技能条件不满足,id={id}");
                return;
            }

            LogicTimeLineShell timeline = LogicTimeLineShell.Create(launcherData.Model.Effect, ShellEntityLogic.Entity, launcherData);
            timeline.Values.Add(Constant.GameLogic.Skill_FirePos, firePos);
            timeline.Values.Add(Constant.GameLogic.Skill_FireDire, fireDireNorm);
            timeline.Values.Add(Constant.GameLogic.Skill_TargetPos, targetPos);
            timeline.Values.Add(Constant.GameLogic.Skill_TargetSerialId, targetSerialId);

            for (int i = 0; i < CtrlData.Buffs.Count; i++)
            {
                if (CtrlData.Buffs[i].Model.OnCast != null)
                {
                    timeline = CtrlData.Buffs[i].Model.OnCast(CtrlData.Buffs[i], launcherData, timeline);
                }
            }
            if (timeline != null)
            {
                this.ModifyResource(-1 * launcherData.Model.Cost);
                GameEntry.TimeLine.AddTimeline(timeline);
            }

            //这里是为了避免时间轴的浮点计算冲突，增加一帧延迟。
            launcherData.CoolDown = launcherData.Model.Cooldown + 0.02f;
        }


        /// <summary>
        /// 加载技能发射器
        /// </summary>
        /// <param name="dtId">技能发射器id</param>
        public void LearnSkill(int dtId)
        {
            if (CtrlData.ChaSkillList.Find(skill => skill.Model.DtId == dtId) != null)
            {
                Log.Error($"学习技能，技能发射器已存在,id={dtId}");
                return;
            }

            SkillLauncherShell skillLauncherShell = SkillLauncherShell.Create(dtId);
            CtrlData.ChaSkillList.Add(skillLauncherShell);
            if (skillLauncherShell.Model.Buff != null)
            {
                for (int i = 0; i < skillLauncherShell.Model.Buff.Length; i++)
                {
                    AddBuffInfo abi = skillLauncherShell.Model.Buff[i];
                    abi.Permanent = true;
                    abi.Duration = 10;
                    abi.DurationSetTo = true;
                    AddBuff(abi);
                }
            }
        }

        ///<summary>
        ///判断这个角色是否会被这个damageInfo所杀
        ///<param name="dInfo">要判断的damageInfo</param>
        ///<return>如果是true代表角色可能会被这次伤害所杀</return>
        ///</summary>
        public bool CanBeKilledByDamageInfo(DamageInfo damageInfo)
        {
            if (CtrlData.ImmuneTime > 0 || damageInfo.IsHeal() == true)
                return false;
            int dValue = damageInfo.DamageValue(false);
            return dValue >= CtrlData.CurResource.HP;
        }
        #endregion


        #region  Buff
        ///<summary>
        ///为角色添加buff，当然，删除也是走这个的
        ///</summary>
        public void AddBuff(AddBuffInfo buff)
        {
            List<System.Object> bCaster = new List<System.Object>();
            if (buff.Caster != null)
                bCaster.Add(buff.Caster);
            List<BuffRTData> hasOnes = GetBuffById(buff.BuffModel.BuffKey, bCaster);
            int modStack = Mathf.Min(buff.AddStack, buff.BuffModel.MaxStack);
            bool toRemove = false;
            BuffRTData toAddBuff = null;
            if (hasOnes.Count > 0)
            {
                //已经存在
                hasOnes[0].BuffParam = new Dictionary<string, string>();
                if (buff.BuffParam != null)
                {
                    foreach (KeyValuePair<string, string> kv in buff.BuffParam)
                    {
                        hasOnes[0].BuffParam[kv.Key] = kv.Value;
                    }
                }

                hasOnes[0].Duration = (buff.DurationSetTo == true) ? buff.Duration : (buff.Duration + hasOnes[0].Duration);
                int afterAdd = hasOnes[0].Stack + modStack;
                modStack = afterAdd >= hasOnes[0].Model.MaxStack ? (hasOnes[0].Model.MaxStack - hasOnes[0].Stack) : (afterAdd <= 0 ? (0 - hasOnes[0].Stack) : modStack);
                hasOnes[0].Stack += modStack;
                hasOnes[0].IsPermanent = buff.Permanent;
                toAddBuff = hasOnes[0];
                toRemove = hasOnes[0].Stack <= 0;
            }
            else
            {
                //新建
                toAddBuff = new BuffRTData(
                    buff.BuffModel,
                    buff.Caster,
                    ShellEntityLogic.Entity,
                    buff.Duration,
                    buff.AddStack,
                    buff.Permanent,
                    buff.BuffParam
                );
                CtrlData.Buffs.Add(toAddBuff);
                CtrlData.Buffs.Sort((a, b) =>
                {
                    return a.Model.Priority.CompareTo(b.Model.Priority);
                });
            }
            if (toRemove == false && buff.BuffModel.OnOccurrence != null)
            {
                buff.BuffModel.OnOccurrence(toAddBuff, modStack);
            }
            CtrlData.AttrRecheck();
        }

        ///<summary>
        ///获取角色身上对应的buffObj
        ///<param name="buffKey">buff的model的id</param>
        ///<param name="caster">如果caster不是空，那么就代表只有buffObj.caster在caster里面的才符合条件</param>
        ///<return>符合条件的buffObj数组</return>
        ///</summary>
        public List<BuffRTData> GetBuffById(string buffKey, List<System.Object> caster = null)
        {
            List<BuffRTData> res = new List<BuffRTData>();
            for (int i = 0; i < CtrlData.Buffs.Count; i++)
            {
                if (CtrlData.Buffs[i].Model.BuffKey == buffKey && (caster == null || caster.Count <= 0 || caster.Contains(CtrlData.Buffs[i].Caster) == true))
                {
                    res.Add(CtrlData.Buffs[i]);
                }
            }
            return res;
        }
        #endregion
    }
}