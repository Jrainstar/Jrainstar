using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    //在由p1和p2两端定义的直线上取一个点 按between
    public static Vector3 LineLerp(Vector3 p1, Vector3 p2, float between)
    {
        float x = Mathf.Lerp(p1.x, p2.x, between);
        float y = Mathf.Lerp(p1.y, p2.y, between);
        float z = Mathf.Lerp(p1.z, p2.z, between);

        return new Vector3(x, y, z);
    }

    // 贝塞尔缓动 between 的一个坐标
    public static Vector3 Ease(List<Transform> points, float t, bool loop = false)
    {
        if (points.Count < 2)
            throw new System.Exception("Bezier曲线至少需要3个点，线性插值至少需要2个点");

        List<Vector3> vs = new List<Vector3>();

        foreach (var pt in points)
        {
            vs.Add(pt.position);
        }

        if (loop) vs.Add(points[0].position);

        return Process(vs, t);
    }

    // 贝塞尔缓动 between 的一个坐标
    public static Vector3 Ease(List<Vector3> points, float t, bool loop = false)
    {
        if (points.Count < 2)
            throw new System.Exception("Bezier曲线至少需要3个点，线性插值至少需要2个点");

        List<Vector3> vs = new List<Vector3>();

        foreach (var v3 in points)
        {
            vs.Add(v3);
        }

        if (loop) vs.Add(points[0]);

        return Process(vs, t);
    }

    private static Vector3 Process(List<Vector3> points, float t)
    {
        if (points.Count == 2)
        {
            return LineLerp(points[0], points[1], t);
        }

        List<Vector3> lines = new List<Vector3>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 line = LineLerp(points[i], points[i + 1], t);
            lines.Add(line);
        }

        return Process(lines, t);
    }
}
