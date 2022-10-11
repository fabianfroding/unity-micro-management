public class AIPathNode
{
    public int x, z;
    public int fCost, gCost, hCost;
    public bool walkable;

    public AIPathNode previousNode;

    public AIPathNode(int x, int z)
    {
        this.x = x;
        this.z = z;
        walkable = true;
    }

    public void CalculateFCost() => fCost = gCost + hCost;

    public override string ToString() => x + ", " + z ;
}
