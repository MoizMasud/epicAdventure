using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D col;
    public float moveMult = 2f;
    public float jumpMult = 5f;
    public float maxSpeed = 8f;
    private bool grounded = true;
    private bool secondJump = true;
    public LayerMask mapLayer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
       
    }

    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        if (hor != 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (hor / Mathf.Abs(hor)), transform.localScale.y, transform.localScale.z);
        }
        if (Physics2D.Raycast(transform.position, Vector2.down, col.bounds.extents.y + 0.1f, mapLayer))
        {
            if (Mathf.Abs(rb.velocity.x) < maxSpeed) {
                rb.AddForce(new Vector3(hor * moveMult, 0, 0));
            }
            if (hor == 0 || (rb.velocity.x != 0 && hor / rb.velocity.x < 0))
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
            secondJump = true;
            grounded = true;
        }
        else
        {
            if (hor != 0 && Mathf.Abs(rb.velocity.x) < maxSpeed) {
                rb.AddForce(new Vector3(hor * moveMult, 0, 0));
            }
            if ((rb.velocity.x != 0 && hor / rb.velocity.x < 0))
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
            grounded = false;
        }
        if ((grounded || secondJump) && Input.GetKeyDown(KeyCode.Space))
        {
            if (!grounded)
            {
                secondJump = false;
                rb.velocity = new Vector3(rb.velocity.x, 0, 0);
            }
            rb.AddForce(Vector2.up * jumpMult, ForceMode2D.Impulse);
        }
        anim.SetBool("Moving", hor != 0);
        anim.SetBool("Grounded", grounded);
        anim.SetFloat("Vertical", rb.velocity.y);
    }

    public void RefreshSecondJump()
    {
        secondJump = true;
    }
}
