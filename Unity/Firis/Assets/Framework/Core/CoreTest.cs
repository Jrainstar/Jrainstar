using Firis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreTest : MonoBehaviour
{
    AE ae1;
    AE ae2;
    AC ac1;
    AC ac2;

    void Start()
    {
        ae1 = APP.Scene.AddChild<AE>();
        ae2 = ae1.AddChild<AE>();
        ac1 = ae1.AddComponent<AC>();
        ac2 = ae2.AddComponent<AC>();
    }

    void Update()
    {
        APP.EventSystem.Update();

        // 测试AddComponent
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("APP.Scene---" + APP.Scene.ChildCount + "---" + APP.Scene.ComponentCount);
            Debug.Log("AE1---" + ae1.ChildCount + "---" + ae1.ComponentCount);
            Debug.Log("AE2---" + ae2.ChildCount + "---" + ae2.ComponentCount);
            APP.Scene.AddComponent(ac2);
            Debug.Log("APP.Scene---" + APP.Scene.ChildCount + "---" + APP.Scene.ComponentCount);
            Debug.Log("AE1---" + ae1.ChildCount + "---" + ae1.ComponentCount);
            Debug.Log("AE2---" + ae2.ChildCount + "---" + ae2.ComponentCount);
        }

        // 测试AddChild
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("APP.Scene---" + APP.Scene.ChildCount + "---" + APP.Scene.ComponentCount);
            Debug.Log("AE1---" + ae1.ChildCount + "---" + ae1.ComponentCount);
            Debug.Log("AE2---" + ae2.ChildCount + "---" + ae2.ComponentCount);
            APP.Scene.AddChild(ae2);
            Debug.Log("APP.Scene---" + APP.Scene.ChildCount + "---" + APP.Scene.ComponentCount);
            Debug.Log("AE1---" + ae1.ChildCount + "---" + ae1.ComponentCount);
            Debug.Log("AE2---" + ae2.ChildCount + "---" + ae2.ComponentCount);
        }

        // 测试RemoveComponent
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("APP.Scene---" + APP.Scene.ChildCount + "---" + APP.Scene.ComponentCount);
            Debug.Log("AE1---" + ae1.ChildCount + "---" + ae1.ComponentCount);
            Debug.Log("AE2---" + ae2.ChildCount + "---" + ae2.ComponentCount);
            ae1.RemoveComponent(ac1);
            Debug.Log("APP.Scene---" + APP.Scene.ChildCount + "---" + APP.Scene.ComponentCount);
            Debug.Log("AE1---" + ae1.ChildCount + "---" + ae1.ComponentCount);
            Debug.Log("AE2---" + ae2.ChildCount + "---" + ae2.ComponentCount);
        }

        // 测试RemoveComponent
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("APP.Scene---" + APP.Scene.ChildCount + "---" + APP.Scene.ComponentCount);
            Debug.Log("AE1---" + ae1.ChildCount + "---" + ae1.ComponentCount);
            Debug.Log("AE2---" + ae2.ChildCount + "---" + ae2.ComponentCount);
            ae1.RemoveChild(ae2);
            Debug.Log("APP.Scene---" + APP.Scene.ChildCount + "---" + APP.Scene.ComponentCount);
            Debug.Log("AE1---" + ae1.ChildCount + "---" + ae1.ComponentCount);
            Debug.Log("AE2---" + ae2.ChildCount + "---" + ae2.ComponentCount);
        }

        // 测试Dispose
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("APP.Scene---" + APP.Scene.ChildCount + "---" + APP.Scene.ComponentCount);
            Debug.Log("AE1---" + ae1.ChildCount + "---" + ae1.ComponentCount);
            Debug.Log("AE2---" + ae2.ChildCount + "---" + ae2.ComponentCount);
            ae1.Dispose();
            Debug.Log("APP.Scene---" + APP.Scene.ChildCount + "---" + APP.Scene.ComponentCount);
            Debug.Log("AE1---" + ae1.ChildCount + "---" + ae1.ComponentCount);
            Debug.Log("AE2---" + ae2.ChildCount + "---" + ae2.ComponentCount);
        }
    }
}

public class AE : Entity, IAwake
{
    public void Awake()
    {
        Debug.Log("E-Awake");
    }

    public override void Dispose()
    {
        base.Dispose();
        Debug.Log("E-Dispose");
    }
}

public class AC : Firis.Component, IAwake
{
    public void Awake()
    {
        Debug.Log("C-Awake");
    }

    public override void Dispose()
    {
        base.Dispose();
        Debug.Log("C-Dispose");
    }
}