using System.Collections.Generic;
using UnityEngine;

public class AIGridSetup : MonoBehaviour
{
    private AIPathfinding aiPathfinding;

    private void Start()
    {
        aiPathfinding = new AIPathfinding(10, 10, 4f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = AIUtils.GetMouseWorldPosition();
            aiPathfinding.GetAIGrid().GetXZ(mouseWorldPosition, out int x, out int z);
            List<AIPathNode> path = aiPathfinding.FindPath(0, 0, x, z);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x, 0, path[i].z) * AIPathfinding.CellSize + new Vector3(1, 0.2f, 1) * (AIPathfinding.CellSize * 0.5f), 
                        new Vector3(path[i + 1].x, 0, path[i + 1].z) * AIPathfinding.CellSize + new Vector3(1, 0.2f, 1) * (AIPathfinding.CellSize * 0.5f), Color.blue, 5f);
                }
            }
        }

    }
}
