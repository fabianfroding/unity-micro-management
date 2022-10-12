using UnityEngine;

public class AIPathNode
{
    private const float MAX_HEIGHT_FOR_UNWALKABLE_ENV = 7.5f;

    public int x, z;
    public int fCost, gCost, hCost;
    public bool walkable;

    public AIPathNode previousNode;

    public AIPathNode(int x, int z, Vector3 loc)
    {
        this.x = x;
        this.z = z;
        walkable = true;

        if (walkable && 
            Physics.Raycast(new Vector3(loc.x, -0.2f, loc.z), Vector3.up, 
            MAX_HEIGHT_FOR_UNWALKABLE_ENV + 2f, 1 << LayerMask.NameToLayer(EditorConstants.LAYER_ENV)))
        {
            walkable = false;
            //Debug.DrawRay(new Vector3(loc.x, -0.2f, loc.z), Vector3.up * MAX_HEIGHT_FOR_UNWALKABLE_ENV * 2f, Color.red, 30f);
        }
    }

    public void CalculateFCost() => fCost = gCost + hCost;

    public override string ToString() => x + ", " + z ;
}
