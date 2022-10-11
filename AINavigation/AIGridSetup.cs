using System.Collections.Generic;
using UnityEngine;

public class AIGridSetup : MonoBehaviour
{
    private AIPathfinding aiPathfinding;

    private void Start()
    {
        aiPathfinding = new AIPathfinding(100, 100, 2f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = AIUtils.GetMouseWorldPosition();
            aiPathfinding.GetAIGrid().GetXY(mouseWorldPosition, out int x, out int z);
            List<AIPathNode> path = aiPathfinding.FindPath(0, 0, x, z);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x, 0, path[i].z) * aiPathfinding.CellSize + Vector3.one * 5f, 
                        new Vector3(path[i + 1].x, 0, path[i + 1].z) * aiPathfinding.CellSize + Vector3.one * 5f, Color.blue, 5f);
                }
            }
        }

    }
}
