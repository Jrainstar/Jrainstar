using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Firis
{
    public class AOIMap : Entity, IAwake, IAwake<int>
    {
        #region 属性
        public int GridSize { get; set; }
        public Dictionary<long, AOIGrid> Grids { get; set; } = new Dictionary<long, AOIGrid>();
        public Dictionary<long, AOIEntity> Entitys { get; set; } = new Dictionary<long, AOIEntity>();
        #endregion

        #region 构造
        //public AOIMap()
        //{
        //    GridSize = 1;
        //}
        //public AOIMap(int gridSize)
        //{
        //    GridSize = gridSize;
        //}

        public void Awake()
        {
            GridSize = 1;
        }

        public void Awake(int gridSize)
        {
            GridSize = gridSize;
        }
        #endregion

        /// <summary>
        /// Entity 加入此地图
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        public void Add(AOIEntity entity, Vector3 position)
        {
            if (Entitys.ContainsKey(entity.ID))
            {
                Log.Warn("不允许有相同ID的Entity加入 或者 重复加入");
                return;
            }

            if (entity.Map != null)
            {
                entity.Clear();
            }

            entity.Map = this;
            entity.Position = position;
            Entitys.Add(entity.ID, entity);

            ReBuild(entity, position);
        }

        public void Add(AOIEntity entity)
        {
            Add(entity, entity.Position);
        }

        public void ReBuild(AOIEntity entity, Vector3 position)
        {
            if (!Entitys.ContainsKey(entity.ID))
            {
                Log.Warn("不存在这个实体");
                return;
            }

            float radio = (float)GridSize / 2;
            int gridX;
            int gridY;

            if (position.X >= 0)
                gridX = (int)(position.X + radio) / GridSize;
            else
                gridX = (int)(position.X - radio) / GridSize;

            if (position.Y >= 0)
                gridY = (int)(position.Y + radio) / GridSize;
            else
                gridY = (int)(position.Y - radio) / GridSize;

            CalcEntityGrid(entity, gridX, gridY);
            CalcEntityView(entity, gridX, gridY);
        }

        public void ReBuild(AOIEntity entity)
        {
            ReBuild(entity, entity.Position);
        }

        /// <summary>
        /// 计算Entity可以看到的Grid范围
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cellX"></param>
        /// <param name="cellY"></param>
        public void CalcEntityGrid(AOIEntity entity, int gridX, int gridY)
        {
            entity.SubLeaveGrids.Clear();

            foreach (var pregrid in entity.CanSeeGrids)
            {
                entity.SubLeaveGrids.Add(pregrid);
            }

            entity.CanSeeGrids.Clear();
            entity.SubEnterGrids.Clear();


            // 求出格子 的可观测 范围 
            // 若 ViewDistance 等于 GridSize 则 range == 1
            int range = (entity.ViewDistance - 1) / GridSize + 1;

            for (int i = gridX - range; i <= gridX + range; ++i)
            {
                for (int j = gridY - range; j <= gridY + range; ++j)
                {
                    long gridId = GetGridID(i, j);
                    GetGrid(gridId);
                    entity.CanSeeGrids.Add(gridId);
                    entity.SubEnterGrids.Add(gridId);
                }
            }

            // 这里的SubLeaveGrids 在方法前 变为之前可见范围
            entity.SubEnterGrids.ExceptWith(entity.SubLeaveGrids);
            entity.SubLeaveGrids.ExceptWith(entity.CanSeeGrids);

            // ====================================================

        }
        /// <summary>
        /// 计算Entity的可视Grid和Entity的各种容器状态
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        public void CalcEntityView(AOIEntity entity, int gridX, int gridY)
        {
            AOIGrid grid;
            entity.Grid = GetGrid(GetGridID(gridX, gridY));
            entity.Grid.Remove(entity);
            entity.Grid.Add(entity);

            // 之前可以看到我的Entity
            // 可能由于我的移动而看不见我
            List<AOIEntity> list = new List<AOIEntity>();
            foreach (var pre in entity.BeSeeEntities)
            {
                if (!pre.Value.CanSeeGrids.Contains(entity.Grid.ID))
                {
                    list.Add(pre.Value);
                }
            }
            foreach (var pre in list)
            {
                pre.LeaveEntity(entity);
            }
            // 对当前Grid 感兴趣的Entity和我产生联系
            foreach (var inter in entity.Grid.InterEntities)
            {
                inter.Value.EnterEntity(entity);
            }
            // 进入我视野的Grid 中 所有Entity和我产生联系
            foreach (var gridId in entity.SubEnterGrids)
            {
                grid = GetGrid(gridId);
                grid.Interest(entity);
                entity.GridEnterView(grid);
            }
            // 离开我视野的Grid 中 所有Entity和我产生联系
            foreach (var gridId in entity.SubLeaveGrids)
            {
                grid = GetGrid(gridId);
                grid.NoInterest(entity);
                entity.GridLeaveView(grid);
            }
        }

        // 将两个int 拼接成long
        public static long GetGridID(int x, int y)
        {
            return (long)((ulong)x << 32) | (uint)y;
        }

        public static (int, int) GetGridXY(long gridID)
        {
            int x = (int)(gridID >> 32);
            int y = (int)((gridID << 32) >> 32);
            return (x, y);
        }

        private AOIGrid GetGrid(long ID)
        {
            if (!Grids.ContainsKey(ID))
            {
                Grids.Add(ID, EntityFactory.CreatWithID<AOIGrid>(ID));
            }
            return Grids[ID];
        }
    }
}

