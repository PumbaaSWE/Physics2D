public readonly struct Query
{
    public readonly float separation;
    public readonly int index;
    public readonly Face face;

    public Query(float bestDistance, int bestIndex, Face face)
    {
        this.separation = -bestDistance;
        this.index = bestIndex;
        this.face = face;
    }
}
