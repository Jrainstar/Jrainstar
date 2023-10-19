using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrainstar
{
    public class AOIGrid : Entity
    {
        #region 属性
        // 处在这个Grid的单位
        public Dictionary<long, AOIEntity> ExistEntities { get; set; } = new Dictionary<long, AOIEntity>();
        // 对这个Grid感兴趣的单位 包含ExistEntities
        public Dictionary<long, AOIEntity> InterEntities { get; set; } = new Dictionary<long, AOIEntity>();

        #endregion

        /// <summary>
        /// 接入当前Grid
        /// </summary>
        /// <param name="entity"></param>
        public void Add(AOIEntity entity)
        {
            ExistEntities.TryAdd(entity.ID, entity);
        }

        /// <summary>
        /// 移除当前Grid
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(AOIEntity entity)
        {
            if (ExistEntities.ContainsKey(entity.ID))
                ExistEntities.Remove(entity.ID);
        }

        public void Remove(long id)
        {
            if (ExistEntities.ContainsKey(id))
                ExistEntities.Remove(id);
        }

        /// <summary>
        /// 被Entity关注
        /// </summary>
        /// <param name="entity"></param>
        public void Interest(AOIEntity entity)
        {
            InterEntities.TryAdd(entity.ID, entity);
        }

        /// <summary>
        /// 不再被Entity关注
        /// </summary>
        /// <param name="entity"></param>
        public void NoInterest(AOIEntity entity)
        {
            if (InterEntities.ContainsKey(entity.ID))
                InterEntities.Remove(entity.ID);
        }

        public void NoInterest(long id)
        {
            if (InterEntities.ContainsKey(id))
                InterEntities.Remove(id);
        }


    }
}
