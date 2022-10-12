using System.Collections.Generic;
using UnityEngine;

public class AIMovementHandler : MonoBehaviour
{
    private const float speed = 40f;

    private int currentPathIdx;
    private List<Vector3> pathList;

    private void Update()
    {
        HandleMovement();

        if (Input.GetMouseButtonDown(0))
        {
            SetTargetPosition(AIUtils.GetMouseWorldPosition());
        }
    }

    private void HandleMovement()
    {
        if (pathList != null)
        {
            Vector3 targetPosition = pathList[currentPathIdx];
            if (Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                //float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                //animatedWalker.SetMoveVector(moveDir);
                transform.position = transform.position + speed * Time.deltaTime * moveDir;
            }
            else
            {
                currentPathIdx++;
                if (currentPathIdx >= pathList.Count)
                {
                    pathList = null;
                    //animatedWalker.SetMoveVector(Vector3.zero);
                }
            }
        }
        else
        {
            //animatedWalker.SetMoveVector(Vector3.zero);
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIdx = 0;
        pathList = AIPathfinding.Instance.FindPath(transform.position, targetPosition);
        if (pathList != null && pathList.Count > 1) { pathList.RemoveAt(0); }
    }
}
