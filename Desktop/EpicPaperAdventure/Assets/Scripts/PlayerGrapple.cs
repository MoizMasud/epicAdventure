using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerGrapple : MonoBehaviour
{
    public float maxDist = 10f;
    public float grappleRate = 0.02f;
    public float minLinkDist = 0.4f;
    public float desiredLinkDist = 0.5f;
    public float maxLinkDist = 0.6f;
    public GameObject grappleLink;
    private LineRenderer lineRend;
    public LayerMask grapLayer;
    private bool grappleActive = false;
    private List<List<GameObject>> grappleList = new List<List<GameObject>>();
    private List<List<GrappleLink>> linkScriptList = new List<List<GrappleLink>>();
    private List<List<DistanceJoint2D>> linkJointList = new List<List<DistanceJoint2D>>();
    private List<int> highlightedIndices = new List<int>();
    private Stack<int> nextAddedIndices = new Stack<int>();
    private int grappleCount = 0;
    private Rigidbody2D rb;
    public Color traceFail;
    public Color traceSuccess;
    private Color regularColor;
    private bool reattachAvailable = false;
    private RaycastHit2D reattachHit;
    public Slider resetSlider;
    private float resetTimer = 0;
    private float totalResetTime = 0.5f;
    public List<GameObject> grappleIconList = new List<GameObject>();
    public Color regularColorUI;
    public Color highlightColorUI;
    // Start is called before the first frame update
    void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        nextAddedIndices = new Stack<int>(new int[] { 2, 1, 0 });
        InitializeGrappleList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && nextAddedIndices.Count > 0) //Check to see if list is full
        {
            StartCoroutine(GrappleFire(true));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            DestroyGrapple();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && !nextAddedIndices.Contains(0))
        {
            UpdateGrappleSelect(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !nextAddedIndices.Contains(1))
        {
            UpdateGrappleSelect(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && !nextAddedIndices.Contains(2))
        {
            UpdateGrappleSelect(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && !nextAddedIndices.Contains(3))
        {
            UpdateGrappleSelect(3);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(IncreaseGrapple());
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(DecreaseGrapple());
        }
        if (Input.GetMouseButtonDown(1) && highlightedIndices.Count == 1)
        {
            regularColor = lineRend.material.color;
            lineRend.material.color = traceFail;
        }
        if (Input.GetMouseButton(1) && highlightedIndices.Count == 1)
        {
            lineRend.enabled = true;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRend.SetPosition(0, grappleList[highlightedIndices[0]][0].transform.position);
            Vector3 dir = (new Vector3(mousePos.x, mousePos.y, 0) - grappleList[highlightedIndices[0]][1].transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(grappleList[highlightedIndices[0]][1].transform.position + dir, dir, (new Vector3(mousePos.x, mousePos.y, 0) - grappleList[highlightedIndices[0]][1].transform.position).magnitude, grapLayer);
            if (hit.collider != null)
            {
                reattachAvailable = true;
                reattachHit = hit;
                lineRend.SetPosition(1, new Vector2(hit.point.x, hit.point.y));
                lineRend.material.color = traceSuccess;
            }
            else
            {
                reattachAvailable = false;
                lineRend.SetPosition(1, new Vector3(mousePos.x, mousePos.y, 0));
                lineRend.material.color = traceFail;
            }
        }
        if (Input.GetMouseButtonUp(1) && highlightedIndices.Count == 1)
        {
            if (reattachAvailable)
            {
                ReattachGrapple(reattachHit);
                reattachAvailable = false;
            }
            lineRend.enabled = false;
            lineRend.material.color = regularColor;
        }
        if (Input.GetKey(KeyCode.L))
        {
            resetSlider.value += 0.2f;
            if (resetSlider.value == resetSlider.maxValue)
            {
                resetTimer += Time.deltaTime;
                if (resetTimer >= totalResetTime)
                {
                    LevelManager.Instance.Reset();
                    resetTimer = 0;
                }
            }
        }
        else if (resetSlider.value > 0)
        {
            resetTimer = 0;
            resetSlider.value = 0;
        }
    }

    private void InitializeGrappleList()
    {
        for (int i = 0; i < 4; i++)
        {
            grappleList.Add(new List<GameObject>());
            linkScriptList.Add(new List<GrappleLink>());
            linkJointList.Add(new List<DistanceJoint2D>());
        }
    }

    private void DestroyGrapple()
    {
        for (int i = 0; i < highlightedIndices.Count; i++)
        {
            for (int j = grappleList[highlightedIndices[i]].Count - 1; j >= 0; j--)
            {
                Destroy(grappleList[highlightedIndices[i]][j]);
            }
            grappleList[highlightedIndices[i]] = new List<GameObject>();
            linkScriptList[highlightedIndices[i]] = new List<GrappleLink>();
            linkJointList[highlightedIndices[i]] = new List<DistanceJoint2D>();
            grappleIconList[highlightedIndices[i]].GetComponent<Image>().color = regularColorUI;
            grappleIconList[highlightedIndices[i]].SetActive(false);
            nextAddedIndices.Push(highlightedIndices[i]);
        }
        highlightedIndices.Clear();
    }

    private void UpdateGrappleSelect(int newIndex)
    {
        if (highlightedIndices.Contains(newIndex))
        {
            highlightedIndices.Remove(newIndex);
            for (int i = 0; i < linkScriptList[newIndex].Count; i++)
            {
                linkScriptList[newIndex][i].setHighlight(false);
            }
            grappleIconList[newIndex].GetComponent<Image>().color = regularColorUI;
        }
        else
        {
            highlightedIndices.Add(newIndex);
            for (int i = 0; i < linkScriptList[newIndex].Count; i++)
            {
                linkScriptList[newIndex][i].setHighlight(true);
            }
            grappleIconList[newIndex].GetComponent<Image>().color = highlightColorUI;
        }
    }

    IEnumerator GrappleFire(bool isPlayer)
    {
        lineRend.enabled = true;
        lineRend.SetPosition(0, transform.position);
        float currDist = 0f;
        Vector3 newPos = transform.position;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetVect = (new Vector3(mousePos.x, mousePos.y, 0) - transform.position) / ((new Vector3(mousePos.x, mousePos.y, 0) - transform.position).magnitude);
        while (currDist < maxDist)
        {
            newPos = newPos + (targetVect * grappleRate);
            currDist += grappleRate;
            lineRend.SetPosition(0, transform.position);
            lineRend.SetPosition(1, newPos);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (newPos - transform.position).normalized, currDist, grapLayer);
            if (hit.collider != null)
            {
                if (isPlayer) {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Map"))
                    {
                        InstantiateGrapple(transform.position, hit.point, false, true, GetComponent<Rigidbody2D>(), null);
                    }
                    else
                    {
                        InstantiateGrapple(transform.position, newPos, false, false, GetComponent<Rigidbody2D>(), hit.rigidbody);
                    }
                    break;
                }
            }
            yield return null;
        }
        lineRend.enabled = false;
    }

    IEnumerator GrappleTether(Vector3 currEnd)
    {
        lineRend.enabled = true;
        lineRend.SetPosition(0, currEnd);

        yield return null;
    }

    private void InstantiateGrapple(Vector3 startPos, Vector3 endPos, bool staticStart, bool staticEnd, Rigidbody2D fromBody, Rigidbody2D hitBody)
    {
        int currentListIndex = nextAddedIndices.Pop();
        grappleIconList[currentListIndex].SetActive(true);
        float distance = (endPos - startPos).magnitude;
        float numLinks = Mathf.Round(distance / desiredLinkDist) + 2;
        Vector3 distVector = (startPos - endPos).normalized * desiredLinkDist;
        for (var i = 0; i < numLinks; i++)
        {
            if (i == 0)
            {
                GameObject endLink = Instantiate(grappleLink);
                GrappleLink linkScript = endLink.GetComponent<GrappleLink>();
                if (staticEnd)
                {
                    linkScript.setStaticEnd();
                }
                else
                {
                    Debug.Log(hitBody);
                    linkScript.setDynamicEnd();
                    linkScript.setHingeConnectedObj(hitBody);
                    linkScript.setHingeConnectedAnchor(endPos, hitBody.transform.position);
                }
                linkScript.setPosition(endPos);
                grappleList[currentListIndex].Add(endLink);
                linkScriptList[currentListIndex].Add(linkScript);
                linkJointList[currentListIndex].Add(endLink.GetComponent<DistanceJoint2D>());
            }
            else if (i == numLinks - 1)
            {
                Vector3 newSpawn = grappleList[currentListIndex][i - 1].transform.position + distVector;
                GameObject newLink = Instantiate(grappleLink);
                GrappleLink linkScript = newLink.GetComponent<GrappleLink>();
                linkScript.setPosition(newSpawn);
                linkScript.setConnectedObj(grappleList[currentListIndex][i - 1].GetComponent<Rigidbody2D>());
                linkScript.setMaxDistance(desiredLinkDist + 0.02f);
                if (staticStart) {
                    linkScript.setStaticStart();
                }
                else
                {
                    linkScript.setDynamicStart();
                    linkScript.setHingeConnectedObj(fromBody);
                }
                GetComponent<PlayerMovement>().RefreshSecondJump();
                grappleList[currentListIndex].Add(newLink);
                linkScriptList[currentListIndex].Add(linkScript);
                linkJointList[currentListIndex].Add(newLink.GetComponent<DistanceJoint2D>());
            }
            else
            {
                Vector3 newSpawn = grappleList[currentListIndex][i - 1].transform.position + distVector;
                GameObject newLink = Instantiate(grappleLink);
                GrappleLink linkScript = newLink.GetComponent<GrappleLink>();
                linkScript.setPosition(newSpawn);
                linkScript.setConnectedObj(grappleList[currentListIndex][i - 1].GetComponent<Rigidbody2D>());
                linkScript.setMaxDistance(desiredLinkDist + 0.02f);
                grappleList[currentListIndex].Add(newLink);
                linkScriptList[currentListIndex].Add(linkScript);
                linkJointList[currentListIndex].Add(newLink.GetComponent<DistanceJoint2D>());
            }
        }
        if (nextAddedIndices.Count == 3)
        {
            UpdateGrappleSelect(currentListIndex);
        }
    }

    IEnumerator IncreaseGrapple()
    {
        while (Input.GetKey(KeyCode.E))
        {
            for (int i = 0; i < highlightedIndices.Count; i++) {
                if (linkJointList[highlightedIndices[i]][3].distance > ((desiredLinkDist + 0.02f) * (linkJointList[highlightedIndices[i]].Count + 1) / (linkJointList[highlightedIndices[i]].Count)))
                {
                    for (var j = 0; j < linkJointList[highlightedIndices[i]].Count; j++)
                    {
                        linkJointList[highlightedIndices[i]][j].distance = 0.52f;
                    }
                    int midIndex = Mathf.RoundToInt(linkJointList[highlightedIndices[i]].Count / 2);
                    Vector3 distVector = (grappleList[highlightedIndices[i]][midIndex].transform.position - grappleList[highlightedIndices[i]][midIndex].transform.position).normalized;
                    Vector3 newSpawn = grappleList[highlightedIndices[i]][midIndex - 1].transform.position + distVector;
                    GameObject newLink = Instantiate(grappleLink);
                    GrappleLink linkScript = newLink.GetComponent<GrappleLink>();
                    linkScript.setHighlight(true);
                    linkScript.setPosition(newSpawn);
                    linkScript.setConnectedObj(grappleList[highlightedIndices[i]][midIndex - 1].GetComponent<Rigidbody2D>());
                    grappleList[highlightedIndices[i]][midIndex].GetComponent<GrappleLink>().setConnectedObj(newLink.GetComponent<Rigidbody2D>());
                    linkScript.setMaxDistance(desiredLinkDist + 0.02f);
                    grappleList[highlightedIndices[i]].Insert(midIndex, newLink);
                    linkScriptList[highlightedIndices[i]].Insert(midIndex, linkScript);
                    linkJointList[highlightedIndices[i]].Insert(midIndex, newLink.GetComponent<DistanceJoint2D>());
                }
                else {
                    for (var j = 0; j < linkJointList[highlightedIndices[i]].Count; j++)
                    {
                        linkJointList[highlightedIndices[i]][j].distance += 0.008f;
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator DecreaseGrapple()
    {
        while (Input.GetKey(KeyCode.Q) && LowestGrappleCount() > 4)
        {
            for (int i = 0; i < highlightedIndices.Count; i++)
            {
                if (grappleList[highlightedIndices[i]][grappleList[highlightedIndices[i]].Count - 1].transform.position.y > transform.position.y && !linkScriptList[highlightedIndices[i]][linkScriptList[highlightedIndices[i]].Count - 1].StaticStatus() && linkScriptList[highlightedIndices[i]][linkScriptList[highlightedIndices[i]].Count - 1].getHingeBody() == rb)
                {
                    rb.velocity = new Vector2(rb.velocity.x, (grappleList[highlightedIndices[i]][0].transform.position - transform.position).normalized.y * 2.75f);
                }
                if (linkJointList[highlightedIndices[i]][3].distance < ((desiredLinkDist - 0.02f) * (linkJointList[highlightedIndices[i]].Count - 1) / (linkJointList[highlightedIndices[i]].Count)))
                {
                    for (int j = 0; j < linkJointList[highlightedIndices[i]].Count; j++)
                    {
                        linkJointList[highlightedIndices[i]][j].distance = 0.52f;
                    }
                    int midIndex = Mathf.RoundToInt(linkJointList[highlightedIndices[i]].Count / 2);
                    linkScriptList[highlightedIndices[i]][midIndex + 1].setConnectedObj(grappleList[highlightedIndices[i]][midIndex - 1].GetComponent<Rigidbody2D>());
                    Destroy(grappleList[highlightedIndices[i]][midIndex]);
                    grappleList[highlightedIndices[i]].RemoveAt(midIndex);
                    linkScriptList[highlightedIndices[i]].RemoveAt(midIndex);
                    linkJointList[highlightedIndices[i]].RemoveAt(midIndex);

                }
                else
                {
                    for (int j = 0; j < linkJointList[highlightedIndices[i]].Count; j++)
                    {
                        linkJointList[highlightedIndices[i]][j].distance -= 0.008f;
                    }
                }
            }
            yield return null;
        }
    }

    private void ReattachGrapple(RaycastHit2D hit)
    {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Map") && linkScriptList[highlightedIndices[0]][0].StaticStatus())
        {
            InstantiateGrapple(hit.point, grappleList[highlightedIndices[0]][0].transform.position, true, true, null, null);
        }
        else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Map") && !linkScriptList[highlightedIndices[0]][0].StaticStatus())
        {
            InstantiateGrapple(hit.point, grappleList[highlightedIndices[0]][0].transform.position, true, false, null, linkScriptList[highlightedIndices[0]][0].getHingeBody());
        }
        else if (linkScriptList[highlightedIndices[0]][0].StaticStatus())
        {
            InstantiateGrapple(hit.point, grappleList[highlightedIndices[0]][0].transform.position, false, true, hit.rigidbody, null);
        }
        else
        {
            InstantiateGrapple(hit.point, grappleList[highlightedIndices[0]][0].transform.position, false, false, hit.rigidbody, linkScriptList[highlightedIndices[0]][0].getHingeBody());
        }
        DestroyGrapple();
    }

    private int LowestGrappleCount()
    {
        int min = 1000;
        for (int i = 0; i < highlightedIndices.Count; i++)
        {
            if (grappleList[highlightedIndices[i]].Count < min)
            {
                min = grappleList[highlightedIndices[i]].Count;
            }
        }

        return min;
    }
}
