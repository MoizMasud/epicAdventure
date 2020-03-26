using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCreator : MonoBehaviour
{
    public GameObject grappleLink;
    public Vector3 startPos;
    public Vector3 endPos;
    public float desiredLinkDist = 0.5f;
    public float maximumLinkDist = 2f;
    
    public void createTether()
    {
        float distance = (endPos - startPos).magnitude;
        float numLinks = Mathf.Round(distance / desiredLinkDist) + 2;
        Vector3 distVector = (startPos - endPos).normalized * desiredLinkDist;
        List<GameObject> links = new List<GameObject>();
        for (var i = 0; i < numLinks; i++)
        {
            if (i == 0)
            {
                GameObject endLink = Instantiate(grappleLink);
                GrappleLink linkScript = endLink.GetComponent<GrappleLink>();
                //linkScript.setEndStatus(true);
                linkScript.setPosition(endPos);
                links.Add(endLink);
            }
            else if (i == numLinks - 1)
            {
                Vector3 newSpawn = links[i-1].transform.position + distVector;
                GameObject newLink = Instantiate(grappleLink);
                GrappleLink linkScript = newLink.GetComponent<GrappleLink>();
                linkScript.setPosition(newSpawn);
                linkScript.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                linkScript.setConnectedObj(links[i - 1].GetComponent<Rigidbody2D>());
                linkScript.setMaxDistance(desiredLinkDist + 0.02f);
            }
            else
            {
                Vector3 newSpawn = links[i-1].transform.position + distVector;
                GameObject newLink = Instantiate(grappleLink);
                GrappleLink linkScript = newLink.GetComponent<GrappleLink>();
                linkScript.setPosition(newSpawn);
                linkScript.setConnectedObj(links[i-1].GetComponent<Rigidbody2D>());
                linkScript.setMaxDistance(desiredLinkDist + 0.02f);
                links.Add(newLink);
            }
        }
    }
}
