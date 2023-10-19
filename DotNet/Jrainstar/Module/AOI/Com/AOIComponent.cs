using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Jrainstar
{
    public class AOIComponent : Component
    {
        private static Dictionary<int, AOIMap> Maps { get; set; } = new Dictionary<int, AOIMap>();

        public static AOIMap GetMap(int mapID)
        {
            if (Maps.ContainsKey(mapID))
                return Maps[mapID];
            else
                return null;
        }

        public static void CreateMap(int mapID, int gridSize)
        {
            if (Maps.ContainsKey(mapID))
            {
                Log.Error("此地图已存在");
                return;
            }
            Maps.Add(mapID, EntityFactory.CreatWithID<AOIMap, int>(mapID, gridSize));
        }

        public static void AddMap(int mapID, AOIEntity entity)
        {
            if (!Maps.ContainsKey(mapID))
            {
                Log.Error("此地图不存在");
                return;
            }
            Maps[mapID].Add(entity);
        }

        public static void AddMap(int mapID, AOIEntity entity, Vector3 position)
        {
            if (!Maps.ContainsKey(mapID))
            {
                Log.Error("此地图不存在");
                return;
            }
            Maps[mapID].Add(entity, position);
        }
    }
}
