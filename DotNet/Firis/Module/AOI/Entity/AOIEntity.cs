using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Firis
{
    public class AOIEntity : Entity, IAwake, IAwake<int, Vector3>
    {
        #region 属性
        /// <summary>
        /// 所在位置
        /// </summary>
        public Vector3 Position { get; set; }
        /// <summary>
        /// 所属AOI管理者
        /// </summary>
        public AOIMap Map { get; set; }
        /// <summary>
        /// 观测距离 每个Entity 的 观测距离可以 不一样
        /// </summary>
        public int ViewDistance { get; set; }
        /// <summary>
        /// 当前所在格子
        /// </summary>
        public AOIGrid Grid { get; set; }


        /// <summary>
        /// 观察进入视野的Grid
        /// </summary>
        public HashSet<long> SubEnterGrids { get; set; } = new HashSet<long>();
        /// <summary>
        /// 观察离开视野的Grid
        /// </summary>
        public HashSet<long> SubLeaveGrids { get; set; } = new HashSet<long>();
        /// <summary>
        /// 看到的Grid
        /// </summary>
        public HashSet<long> CanSeeGrids { get; set; } = new HashSet<long>();

        /// <summary>
        /// 我看见的Entity 包括了自己
        /// </summary>
        public Dictionary<long, AOIEntity> ToSeeEntities { get; set; } = new Dictionary<long, AOIEntity>();
        /// <summary>
        /// 看见我的Entity 包括了自己
        /// </summary>
        public Dictionary<long, AOIEntity> BeSeeEntities { get; set; } = new Dictionary<long, AOIEntity>();
        #endregion

        #region 构造
        public void Awake()
        {
            ViewDistance = 1;
            Position = Vector3.Zero;
        }

        public void Awake(int viewDistance, Vector3 position)
        {
            ViewDistance = viewDistance;
            Position = position;
        }
        #endregion

        #region 事件
        // 只可能因为自己的移动 而有Grid 进入或离开 视野
        public event Action<AOIGrid> onGridEnterView;
        public event Action<AOIGrid> onGridLeaveView;
        // 可能因为自身的移动 而有Entity 进入或离开 视野
        // 可能因为其他的移动 而有Entity 进入或离开 视野
        public event Action<AOIEntity> onEntityEnterView;
        public event Action<AOIEntity> onEntityLeaveView;
        #endregion

        public void ReBuild(int viewDistance)
        {
            ViewDistance = viewDistance;
            Map.ReBuild(this);
        }

        public void ReBuild(Vector3 position)
        {
            Position = position;
            Map.ReBuild(this);
        }

        public void ReBuild(int viewDistance, Vector3 position)
        {
            ViewDistance = viewDistance;
            Position = position;
            Map.ReBuild(this);
        }

        /// <summary>
        /// 进入我视野的Grid
        /// </summary>
        /// <param name="grid"></param>
        public void GridEnterView(AOIGrid grid)
        {
            foreach (var entity in grid.ExistEntities)
            {
                this.ToSeeEntities.TryAdd(entity.Key, entity.Value);
                entity.Value.BeSeeEntities.TryAdd(ID, this);
                onEntityEnterView?.Invoke(entity.Value);
            }
            onGridEnterView?.Invoke(grid);
        }

        /// <summary>
        /// 离开我视野的Grid
        /// </summary>
        /// <param name="grid"></param>
        public void GridLeaveView(AOIGrid grid)
        {
            foreach (var entity in grid.ExistEntities)
            {
                this.ToSeeEntities.Remove(entity.Key);
                entity.Value.BeSeeEntities.Remove(ID);
                onEntityLeaveView?.Invoke(entity.Value);
            }
            onGridLeaveView?.Invoke(grid);
        }

        public void EnterEntity(AOIEntity entity)
        {
            this.ToSeeEntities.TryAdd(entity.ID, entity);
            entity.BeSeeEntities.TryAdd(ID, this);
            onEntityEnterView?.Invoke(entity);
        }

        public void LeaveEntity(AOIEntity entity)
        {
            this.ToSeeEntities.Remove(entity.ID);
            entity.BeSeeEntities.Remove(ID);
            onEntityLeaveView?.Invoke(entity);
        }

        /// <summary>
        /// 是否被此ID 实体看见
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool IsBeSee(long ID)
        {
            return BeSeeEntities.ContainsKey(ID);
        }

        public void Destroy()
        {
            // TODO
        }

        public void Clear()
        {
            Grid.Remove(ID);
            Map.Entitys.Remove(ID);
            foreach (var entity in BeSeeEntities)
            {
                entity.Value.ToSeeEntities.Remove(ID);
            }
            foreach (var entity in ToSeeEntities)
            {
                entity.Value.BeSeeEntities.Remove(ID);
            }
            SubEnterGrids.Clear();
            SubLeaveGrids.Clear();
            CanSeeGrids.Clear();
            ToSeeEntities.Clear();
            BeSeeEntities.Clear();
        }


    }

}
