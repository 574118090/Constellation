using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // ����ƶ��ٶ�
    public float zoomSpeed = 10f; // ��������ٶ�
    public float minSize = 1f; // ��С��ʾ��Χ
    public float maxSize = 20f; // �����ʾ��Χ

    private Vector3 lastMousePosition; // ��һ֡���λ��

    void Update()
    {
        // ����м��϶����
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.Translate(-delta.x * moveSpeed * Time.deltaTime, -delta.y * moveSpeed * Time.deltaTime, 0);
        }

        // ���������ֿ�����ʾ��Χ
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera camera = GetComponent<Camera>();
        float newSize = camera.orthographicSize - scroll * zoomSpeed;
        newSize = Mathf.Clamp(newSize, minSize, maxSize);
        camera.orthographicSize = newSize;

        lastMousePosition = Input.mousePosition;
    }

    public void SetCameraPosition(Vector3 worldPosition)
    {
        Camera mainCamera = Camera.main; // ��ȡ�������
        if (mainCamera != null)
        {
            // ����������������꣬����z��λ�ò���
            mainCamera.transform.position = new Vector3(worldPosition.x, worldPosition.y, mainCamera.transform.position.z);
        }
    }
}
