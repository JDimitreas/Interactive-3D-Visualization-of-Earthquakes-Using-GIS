using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDragOrbitCamera : MonoBehaviour
{
    public float moveSpeed = 1000f;
    public float rotateSpeed = 2f;
    public float heightSpeed = 1000f;
    public float zoomSpeed = 100f;
    public float minZoomDistance = 100f;
    public float maxZoomDistance = 10000f;

    private Vector3 lastMousePosition;

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject()) 
        {
            // Drag to pan (left-click)
            if (Input.GetMouseButton(0))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;

                // Convert screen delta to movement in world XZ plane
                Vector3 right = Camera.main.transform.right;
                Vector3 forward = Camera.main.transform.forward;
                right.y = 0;
                forward.y = 0;
                right.Normalize();
                forward.Normalize();

                Vector3 move = (-delta.x * right + -delta.y * forward) * moveSpeed * Time.deltaTime;

                transform.Translate(move, Space.World);
            }

            // Right-click to rotate
            if (Input.GetMouseButton(1))
            {
                float rotX = Input.GetAxis("Mouse X") * rotateSpeed;
                float rotY = -Input.GetAxis("Mouse Y") * rotateSpeed;
                transform.Rotate(Vector3.up, rotX, Space.World);
                transform.Rotate(Vector3.right, rotY, Space.Self);
            }

            // Adjust height with middle mouse
            if (Input.GetMouseButton(2))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;

                float heightChange = -delta.y * heightSpeed * Time.deltaTime;
                transform.Translate(new Vector3(0, heightChange, 0), Space.World);
            }

            // Scroll wheel to zoom (in the direction the camera is facing)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                Vector3 zoomDirection = transform.forward;
                Vector3 newPosition = transform.position + zoomDirection * scroll * zoomSpeed;

                float distance = Vector3.Distance(newPosition, Vector3.zero); // you can change this reference point
                if (distance >= minZoomDistance && distance <= maxZoomDistance)
                {
                    transform.position = newPosition;
                }
            }

            lastMousePosition = Input.mousePosition;
        }
    }
}
