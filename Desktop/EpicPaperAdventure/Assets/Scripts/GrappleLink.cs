using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleLink : MonoBehaviour
{
    private LineRenderer lineRend;
    private DistanceJoint2D distJoint;
    private HingeJoint2D hinJoint;
    private Rigidbody2D rb;
    private bool isEndLink = false;
    private bool startInitialized = false;
    private bool isStatic = false;
    public Material normalMaterial;
    public Material highlightedMaterial;
    // Start is called before the first frame update
    void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        distJoint = GetComponent<DistanceJoint2D>();
        hinJoint = GetComponent<HingeJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        startInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (distJoint != null && distJoint.enabled && distJoint.connectedAnchor != null)
        {
            lineRend.SetPosition(0, transform.position);
            lineRend.SetPosition(1, distJoint.connectedBody.transform.position);
        }
        else {
            lineRend.enabled = false;
        }
    }

    public void setPosition(Vector3 newPos)
    {
        if (!startInitialized)
        {
            Start();
        }
        transform.position = newPos;
    }

    public void setConnectedObj(Rigidbody2D conRb)
    {
        if (!startInitialized)
        {
            Start();
        }
        distJoint.connectedBody = conRb;
    }

    public void setHingeConnectedObj(Rigidbody2D conRb)
    {
        if (!startInitialized)
        {
            Start();
        }
        hinJoint.connectedBody = conRb;
    }

    public void setMaxDistance(float dist)
    {
        if (!startInitialized)
        {
            Start();
        }
        distJoint.maxDistanceOnly = true;
        distJoint.distance = dist;
    }

    public void setDynamicEnd()
    {
        if (!startInitialized)
        {
            Start();
        }
        isStatic = false;
        distJoint.enabled = false;
        hinJoint.enabled = true;
    }

    public void setStaticEnd()
    {
        if (!startInitialized)
        {
            Start();
        }
        isStatic = true;
        distJoint.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
    }

    public void setDynamicStart()
    {
        if (!startInitialized)
        {
            Start();
        }
        isStatic = false;
        hinJoint.enabled = true;
    }

    public void setStaticStart()
    {
        if (!startInitialized)
        {
            Start();
        }
        isStatic = true;
        hinJoint.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
    }

    public void setHighlight(bool isFocused)
    {
        if (!startInitialized)
        {
            Start();
        }
        if (isFocused) {
            lineRend.material = highlightedMaterial;
        }
        else
        {
            lineRend.material = normalMaterial;
        }
    }

    public bool StaticStatus()
    {
        return isStatic;
    }

    public Rigidbody2D getHingeBody()
    {
        return hinJoint.connectedBody;
    } 

    public void setHingeConnectedAnchor(Vector3 point, Vector3 objTransform)
    {
        Debug.Log(point);
        Debug.Log(objTransform);
        Debug.Log(new Vector2(point.x - objTransform.x, point.y - objTransform.y));
        hinJoint.connectedAnchor = new Vector2((point.x - objTransform.x)/4, (point.y - objTransform.y)/3);
    }
}
