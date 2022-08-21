using UnityEngine;

public class PlayerHealthSlider : MonoBehaviour
{
    void LateUpdate()
    {
        transform.LookAt(new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z));
    }
}
