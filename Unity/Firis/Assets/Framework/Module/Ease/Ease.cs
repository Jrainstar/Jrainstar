using UnityEngine;

public enum EaseType { Linear, QuadIn, QuadOut, QuadInOut, CubeIn, CubeOut, CubeInOut, BackIn, BackOut, BackInOut, ExpoIn, ExpoOut, ExpoInOut, SineIn, SineOut, SineInOut, ElasticIn, ElasticOut, ElasticInOut }

public delegate float Easer(float t);

public static class Ease
{
    public static readonly Easer Linear = (t) => { return t; };
    public static readonly Easer QuadIn = (t) => { return t * t; };
    public static readonly Easer QuadOut = (t) => { return 1 - QuadIn(1 - t); };
    public static readonly Easer QuadInOut = (t) => { return (t <= 0.5f) ? QuadIn(t * 2) / 2 : QuadOut(t * 2 - 1) / 2 + 0.5f; };
    public static readonly Easer CubeIn = (t) => { return t * t * t; };
    public static readonly Easer CubeOut = (t) => { return 1 - CubeIn(1 - t); };
    public static readonly Easer CubeInOut = (t) => { return (t <= 0.5f) ? CubeIn(t * 2) / 2 : CubeOut(t * 2 - 1) / 2 + 0.5f; };
    public static readonly Easer BackIn = (t) => { return t * t * (2.70158f * t - 1.70158f); };
    public static readonly Easer BackOut = (t) => { return 1 - BackIn(1 - t); };
    public static readonly Easer BackInOut = (t) => { return (t <= 0.5f) ? BackIn(t * 2) / 2 : BackOut(t * 2 - 1) / 2 + 0.5f; };
    public static readonly Easer ExpoIn = (t) => { return (float)Mathf.Pow(2, 10 * (t - 1)); };
    public static readonly Easer ExpoOut = (t) => { return 1 - ExpoIn(t); };
    public static readonly Easer ExpoInOut = (t) => { return t < .5f ? ExpoIn(t * 2) / 2 : ExpoOut(t * 2) / 2; };
    public static readonly Easer SineIn = (t) => { return -Mathf.Cos(Mathf.PI / 2 * t) + 1; };
    public static readonly Easer SineOut = (t) => { return Mathf.Sin(Mathf.PI / 2 * t); };
    public static readonly Easer SineInOut = (t) => { return -Mathf.Cos(Mathf.PI * t) / 2f + .5f; };
    public static readonly Easer ElasticIn = (t) => { return 1 - ElasticOut(1 - t); };
    public static readonly Easer ElasticOut = (t) => { return Mathf.Pow(2, -10 * t) * Mathf.Sin((t - 0.075f) * (2 * Mathf.PI) / 0.3f) + 1; };
    public static readonly Easer ElasticInOut = (t) => { return (t <= 0.5f) ? ElasticIn(t * 2) / 2 : ElasticOut(t * 2 - 1) / 2 + 0.5f; };

    public static Easer FromType(EaseType type)
    {
        switch (type)
        {
            case EaseType.Linear: return Linear;
            case EaseType.QuadIn: return QuadIn;
            case EaseType.QuadOut: return QuadOut;
            case EaseType.QuadInOut: return QuadInOut;
            case EaseType.CubeIn: return CubeIn;
            case EaseType.CubeOut: return CubeOut;
            case EaseType.CubeInOut: return CubeInOut;
            case EaseType.BackIn: return BackIn;
            case EaseType.BackOut: return BackOut;
            case EaseType.BackInOut: return BackInOut;
            case EaseType.ExpoIn: return ExpoIn;
            case EaseType.ExpoOut: return ExpoOut;
            case EaseType.ExpoInOut: return ExpoInOut;
            case EaseType.SineIn: return SineIn;
            case EaseType.SineOut: return SineOut;
            case EaseType.SineInOut: return SineInOut;
            case EaseType.ElasticIn: return ElasticIn;
            case EaseType.ElasticOut: return ElasticOut;
            case EaseType.ElasticInOut: return ElasticInOut;
        }
        return Linear;
    }

    public static float Lerp(float t, EaseType type)
    {
        return FromType(type)(t);
    }
}
