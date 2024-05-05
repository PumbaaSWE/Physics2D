using UnityEngine;

public readonly struct Face
{
    public readonly Vector2 a;
    public readonly Vector2 b;
    public readonly Vector2 n;

    public Face(Vector2 a, Vector2 b, Vector2 n)
    {
        this.a = a;
        this.b = b;
        this.n = n;
    }

    public Vector2 Normal()
    {
        return new Vector2(-(b.y - a.y), b.x - a.x).normalized;
    }
}
