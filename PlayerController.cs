using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private Entity selectedEntity;

    NavMeshAgent agent;
    public float rotateSpeedMovement = 0.1f;
    float rotateVelocity;

    private void Awake()
    {
        Entity.OnClicked += Test;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedEntity != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity/*, LayerMask.NameToLayer("Ground")*/))
                {
                    //selectedEntity.OrderMove(raycastHit.point);
                    agent.SetDestination(raycastHit.point);

                    Quaternion rotationLookAt = Quaternion.LookRotation(raycastHit.point - transform.position);
                    float rotationY =  Mathf.SmoothDampAngle(transform.eulerAngles.y,
                        rotationLookAt.eulerAngles.y,
                        ref rotateVelocity,
                        rotateSpeedMovement * (Time.deltaTime * 5));

                    transform.eulerAngles = new Vector3(0, rotationY, 0);
                }
            }
        }
    }

    private void Test(Entity e)
    {
        selectedEntity = e;
        Debug.Log("Clicked " + e.gameObject.name);
    }
}
