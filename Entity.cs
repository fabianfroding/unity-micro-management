using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private Rigidbody rigidBody;
    private Vector3 destination;

    public static event Action<Entity> OnClicked;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (destination != Vector3.zero)
        {
            if (Vector3.Distance(destination, transform.position) > 1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, 1.1f * Time.fixedDeltaTime);
            }
            else
            {
                rigidBody.velocity = Vector3.zero;
                destination = Vector3.zero;
            }
            
        }
    }

    private void OnMouseUp()
    {
        OnClicked?.Invoke(this);
    }

    public void OrderMove(Vector3 destination) => this.destination = destination;
}
