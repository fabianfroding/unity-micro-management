using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public float scrollSpeed = 20f;
    public float maxY = 20f;
    public float minY = 120f;

    private void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.UpArrow) || 
            (Input.mousePosition.y >= Screen.height - panBorderThickness && Input.mousePosition.y < Screen.height + panBorderThickness))
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow) || 
            (Input.mousePosition.y <= panBorderThickness && Input.mousePosition.y > -panBorderThickness))
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || 
            (Input.mousePosition.x <= panBorderThickness && Input.mousePosition.x > -panBorderThickness))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow) || 
            (Input.mousePosition.x >= Screen.width - panBorderThickness && Input.mousePosition.x < Screen.width + panBorderThickness))
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis(EditorConstants.INPUT_MOUSE_SCROLL_WHEEL);
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        //pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

        transform.position = pos;
    }
}
