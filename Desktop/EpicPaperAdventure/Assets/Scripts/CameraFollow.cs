using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float standardZoom = 9;
    public float maxZoom = 14;
    public float zoomSpeed = 1;
    public float maxOffset = 10f;
    private float xOffset = 0;
    private float yOffset = 0;
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.orthographicSize = standardZoom;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey(KeyCode.C) && Camera.main.orthographicSize < maxZoom)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, Camera.main.orthographicSize + 1, Time.deltaTime * zoomSpeed);
        }
        else if (!Input.GetKey(KeyCode.C) && Camera.main.orthographicSize > standardZoom)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, Camera.main.orthographicSize - 1, Time.deltaTime * zoomSpeed);
            if (Mathf.Abs(xOffset) > 0)
            {
                xOffset = Mathf.Lerp(xOffset, -0.1f * (Mathf.Abs(xOffset) / xOffset), Time.deltaTime * zoomSpeed);
            }
            if (Mathf.Abs(yOffset) > 0)
            {
                yOffset = Mathf.Lerp(yOffset, -0.1f * (Mathf.Abs(yOffset) / yOffset), Time.deltaTime * zoomSpeed);
            }
        }
        else if (Input.GetKey(KeyCode.C))
        {
            xOffset += Input.GetAxis("Mouse X")/3;
            yOffset += Input.GetAxis("Mouse Y")/3;
            if (Mathf.Abs(xOffset) > maxOffset)
            {
                xOffset = maxOffset * (Mathf.Abs(xOffset) / xOffset);
            }
            if (Mathf.Abs(yOffset) > maxOffset)
            {
                yOffset = maxOffset * (Mathf.Abs(yOffset) / yOffset);
            }
            
        }
        else
        {
            if (Mathf.Abs(xOffset) > 0.1)
            {
                xOffset = Mathf.Lerp(xOffset, -0.1f * (Mathf.Abs(xOffset) / xOffset), Time.deltaTime * zoomSpeed / 2);
            }
            if (Mathf.Abs(yOffset) > 0.1)
            {
                yOffset = Mathf.Lerp(yOffset, -0.1f * (Mathf.Abs(yOffset) / yOffset), Time.deltaTime * zoomSpeed / 2);
            }
        }
        transform.position = new Vector3(target.transform.position.x + xOffset, target.position.y + yOffset, transform.position.z);
    }
}
