using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // игрок
    public float distance = 5f;
    public float height = 2f;

    public float xSpeed = 120f;
    public float ySpeed = 80f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 60f;

    private float x = 0f;
    private float y = 20f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (!target) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        x += mouseX * xSpeed * Time.deltaTime;
        y -= mouseY * ySpeed * Time.deltaTime;
        y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 direction = rotation * new Vector3(0, 0, -distance);
        Vector3 position = target.position + Vector3.up * height + direction;

        transform.position = position;
        transform.rotation = rotation;
    }
}