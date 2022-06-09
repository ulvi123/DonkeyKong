using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{



    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;

    public new Rigidbody2D rigidbody;
    private Vector2 direction;

    private Collider2D[] overlaps = new Collider2D[4];
    private new Collider2D collider;

    private bool grounded;
    public float moveSpeed = 3f;
    public float jumpStrength = 6f;
    private bool climbing;




    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f / 12, 1f / 12);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void Update()
    {
        CheckCollision();
        SetDirection();
    }



    //Functon for checking collision and direction setting

    private void CheckCollision()
    {
        grounded = false;
        climbing = false;

        Vector2 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;
        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0, overlaps);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = overlaps[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f);
                Physics2D.IgnoreCollision(collider, overlaps[i], !grounded);
            }

            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                climbing = true;

            }
        }
    }


    private void SetDirection()
    {
        if (climbing && Input.GetKey(KeyCode.UpArrow))
        {
            direction.y = Input.GetAxis("Vertical");
            rigidbody.velocity = Vector2.up * moveSpeed;

        }

        else if (Input.GetButtonDown("Jump"))
        {
            rigidbody.AddForce(transform.up * jumpStrength, ForceMode2D.Impulse);
            direction += Physics2D.gravity * Time.deltaTime;

        }
        else if (Input.GetKey(KeyCode.D))
        {
            rigidbody.velocity = Vector2.right * moveSpeed;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rigidbody.velocity = Vector2.left * moveSpeed;
        }


        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        direction.x = Input.GetAxis("Horizontal") * moveSpeed;


        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }


    }

    private void AnimateSprite()
    {
        if (climbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)
        {
            spriteIndex++;

            if (spriteIndex >= runSprites.Length)
            {
                spriteIndex = 0;
            }

            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            enabled = false;
            FindObjectOfType<GameManager>().LevelComplete();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            enabled = false;
            FindObjectOfType<GameManager>().LevelFailed();
        }
    }



}
