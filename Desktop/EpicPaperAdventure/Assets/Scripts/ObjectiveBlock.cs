using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveBlock : MonoBehaviour
{
    Vector3 initialPos;
    float fallLength;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        fallLength = transform.position.y - 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < fallLength)
        {
            Reset();
        }
    }

    private void Reset()
    {
        transform.position = initialPos;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
