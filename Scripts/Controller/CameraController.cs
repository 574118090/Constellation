using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // 相机移动速度
    public float zoomSpeed = 10f; // 相机缩放速度
    public float minSize = 1f; // 最小显示范围
    public float maxSize = 20f; // 最大显示范围

    private Vector3 lastMousePosition; // 上一帧鼠标位置

    void Update()
    {
        // 鼠标中键拖动相机
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.Translate(-delta.x * moveSpeed * Time.deltaTime, -delta.y * moveSpeed * Time.deltaTime, 0);
        }

        // 滑动鼠标滚轮控制显示范围
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera camera = GetComponent<Camera>();
        float newSize = camera.orthographicSize - scroll * zoomSpeed;
        newSize = Mathf.Clamp(newSize, minSize, maxSize);
        camera.orthographicSize = newSize;

        lastMousePosition = Input.mousePosition;
    }

    public void SetCameraPosition(Vector3 worldPosition)
    {
        Camera mainCamera = Camera.main; // 获取主摄像机
        if (mainCamera != null)
        {
            // 设置相机的中心坐标，保持z轴位置不变
            mainCamera.transform.position = new Vector3(worldPosition.x, worldPosition.y, mainCamera.transform.position.z);
        }
    }
}
