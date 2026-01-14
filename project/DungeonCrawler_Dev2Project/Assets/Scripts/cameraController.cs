using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;
    [SerializeField] Transform player;

    float camRotx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;
        if (invertY)
        {
            camRotx += mouseY;
        }
        else
        {
            camRotx -= mouseY;
        }

        camRotx = Mathf.Clamp(camRotx, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(camRotx, 0, 0);

        player.Rotate(Vector3.up * mouseX);

    }
}
