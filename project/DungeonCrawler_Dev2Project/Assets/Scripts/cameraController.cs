using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("----- Mouse Look -----")]
    [SerializeField] private float sens = 100f;
    [SerializeField] private int lockVertMin = -80;
    [SerializeField] private int lockVertMax = 80;
    [SerializeField] private bool invertY = false;
    [SerializeField] private Transform player;           // Drag player root here

    private float camRotX;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (player == null)
            player = transform.root;
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;

        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        if (invertY)
            camRotX += mouseY;
        else
            camRotX -= mouseY;

        camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(camRotX, 0, 0);

        if (player != null)
            player.Rotate(Vector3.up * mouseX);
    }
}