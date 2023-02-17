using Firis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dir
{
    public bool dir;
    public int forword;
}

public class AoiDemo : MonoBehaviour
{
    public AOIComponent aoi;
    public GameObject cube;

    private GameObject me;
    private AOIEntity meEntity;

    private Dictionary<AOIEntity, GameObject> map = new Dictionary<AOIEntity, GameObject>();
    private Dictionary<GameObject, Dir> dirMap = new Dictionary<GameObject, Dir>();

    void Start()
    {
        aoi = APP.Scene.AddComponent<AOIComponent>();

        aoi.CreateMap(0, 10);

        for (int i = 0; i < 200; i++)
        {
            var go = GameObject.Instantiate(cube, new Vector3(Random.Range(-80, 80), 0, Random.Range(-80, 80)), Quaternion.identity);
            go.name = i.ToString();
            go.GetComponent<MeshRenderer>().material.color = Color.white;

            dirMap.Add(go, new Dir() { dir = i % 2 == 0, forword = 1 });
            var aoiEntity = EntityFactory.CreatWithID<AOIEntity, int, Vector3>(i, 10, go.transform.position);

            map.Add(aoiEntity, go);
            aoi.AddMap(0, aoiEntity);
        }

        me = GameObject.Instantiate(cube, new Vector3(0, 0, 0), Quaternion.identity);
        me.name = "player";
        me.GetComponent<MeshRenderer>().material.color = Color.blue;

        meEntity = EntityFactory.CreatWithID<AOIEntity, int, Vector3>(10001, 10, me.transform.position);
        //map.Add(meEntity, me);
        aoi.AddMap(0, meEntity);

        meEntity.onEntityEnterView += (entity) =>
        {
            if (entity.ID == 10001) return;
            map[entity].gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        };
        meEntity.onEntityLeaveView += (entity) =>
        {
            if (entity.ID == 10001) return;
            map[entity].gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        };
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            me.transform.position += new Vector3(0, 0, 1) * Time.deltaTime * 20;
            meEntity.ReBuild(10, new System.Numerics.Vector3(me.transform.position.x, me.transform.position.z, 0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            me.transform.position += new Vector3(0, 0, -1) * Time.deltaTime * 20;
            meEntity.ReBuild(10, new System.Numerics.Vector3(me.transform.position.x, me.transform.position.z, 0));
        }
        if (Input.GetKey(KeyCode.A))
        {
            me.transform.position += new Vector3(-1, 0, 0) * Time.deltaTime * 20;
            meEntity.ReBuild(10, new System.Numerics.Vector3(me.transform.position.x, me.transform.position.z, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            me.transform.position += new Vector3(1, 0, 0) * Time.deltaTime * 20;
            meEntity.ReBuild(10, new System.Numerics.Vector3(me.transform.position.x, me.transform.position.z, 0));
        }

        foreach (var item in map)
        {
            GameObject go = item.Value;
            if (dirMap[go].dir)
            {
                go.transform.position += new Vector3(0, 0, dirMap[item.Value].forword) * Time.deltaTime * 20;
                if (go.transform.position.z >= 100 || go.transform.position.z <= -100)
                {
                    dirMap[go].forword = -dirMap[go].forword;
                }
            }
            else
            {
                go.transform.position += new Vector3(dirMap[item.Value].forword, 0, 0) * Time.deltaTime * 20;
                if (go.transform.position.x >= 100 || go.transform.position.x <= -100)
                {
                    dirMap[go].forword = -dirMap[go].forword;
                }
            }
            item.Key.ReBuild(10, new System.Numerics.Vector3(go.transform.position.x, go.transform.position.z, 0));
        }

        for (int i = -100; i <= 100; i++)
        {
            if (i % aoi.GetMap(0).GridSize == 0)
            {
                float space = aoi.GetMap(0).GridSize / 2;
                Debug.DrawLine(new Vector3(i + space, 0, -100), new Vector3(i + space, 0, 100), Color.blue);
                Debug.DrawLine(new Vector3(-100, 0, i + space), new Vector3(100, 0, i + space), Color.blue);
            }
        }
    }
}
