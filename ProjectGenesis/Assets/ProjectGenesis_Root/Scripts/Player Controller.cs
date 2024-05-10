using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject[] weapons;
    [SerializeField] private SpriteRenderer sr;
    private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    private SpriteRenderer spriteRenderer;
    [SerializeField] float horizontalInput;
    PlayerInput playerInput;
    Collider2D col;

    [Header("Player Direction")]
    [SerializeField] bool isFacingRight = true;
    [SerializeField] Vector2 moveAxis;

    [Header("Movement")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float groundedDrag;
    [SerializeField] private float midAirDrag;
    private float appliedDrag;
    private bool changingDirection => ((rb.velocity.x > 0f && moveAxis.x < 0f) || (rb.velocity.x < 0f && moveAxis.x > 0f));

    [Header("Physics")]
    [SerializeField] Vector2 gravityDirection;
    [SerializeField] float gravityScale;
    [SerializeField] Vector2 collisionNormal;
    [SerializeField] float collisionAngle;
    [SerializeField] bool trikitakatelas;
    [SerializeField] float groundedAreaLength;
    [SerializeField] float groundedAreaHeight;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    [Header("Jump")]
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowFallMultiplier;
    [SerializeField] bool canGroundJump;
    [SerializeField] private float groundJumpForce;
    [SerializeField] private int wallJumpsMax;
    [SerializeField] private int wallJumpsLeft;
    [SerializeField] bool canWallJump;
    [SerializeField] int wallJumpSide;
    [SerializeField] private float wallJumpForce;

    [Header("Dash")]
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashStrength;
    [SerializeField] private float dashesAvailable;
    [SerializeField] private float maxDashes;
    [SerializeField] private Color dashColor;
    [SerializeField] private float dashColorAmount;
    private Vector2 lastDashAxis;
    private bool isDashing;

    private void Awake()
    {
        //set current health
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        GetInput();
        FacingDirection();

        if (canGroundJump) { dashesAvailable = maxDashes; wallJumpsLeft = wallJumpsMax; }
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        Movement();
        if (!isDashing)
        {
            ApplyLinearDrag();
            FallMultiplier();
            LinearDrag();
        }
        Gravity();
    }

    void Gravity()
    {
        rb.AddForce(-gravityDirection * gravityScale);
    }

    //Vector2 slopeDirc
    void CheckCollisions()
    {
        if (col.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            canWallJump = true;
            ContactPoint2D[] contacts = new ContactPoint2D[10];
            int contactCount = col.GetContacts(contacts);
            for (int i = 0; i < contactCount; i++)
            {
                ContactPoint2D point = contacts[i];
                Debug.DrawRay(point.point, point.normal, Color.red, 2.0f);
                if (point.normal.x < 0)
                {
                    wallJumpSide = -1;
                }
                else if (point.normal.x > 0)
                {
                    wallJumpSide = 1;
                }
                collisionNormal = point.normal;
            }

            collisionAngle = Mathf.Atan2(collisionNormal.y, collisionNormal.x) * 180 / Mathf.PI - 90;
            
            if (collisionAngle >= -45 && collisionAngle <= 45)
            {
                rb.rotation = collisionAngle;
            }
            else
            {

                rb.rotation = 0;
            }
        }
        else
        {
            rb.rotation = 0;

            canWallJump = false;
        }
        

        gravityDirection = Quaternion.Euler(0, 0, rb.rotation) * new Vector2(0, 9.8f);

        //GROUNDCHECK
        canGroundJump = Physics2D.OverlapArea(
                        new Vector2(groundCheck.position.x - (groundedAreaLength / 2),
                                    groundCheck.position.y - groundedAreaHeight),
                        new Vector2(groundCheck.position.x + (groundedAreaLength / 2),
                                    groundCheck.position.y + 0.01f),
                                    groundLayer);
    }

    private void Movement()
    {
        // Rotate the movement axis vector by the z-rotation of the Rigidbody
        Vector2 direction = Quaternion.Euler(0, 0, rb.rotation) * new Vector2(moveAxis.x, 0f);

        float rotatedVelocity = Vector2.Dot(rb.velocity, new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y)));

        //Rotated x velocity
        Vector2.Dot(direction, moveAxis);

        // Calculate the velocity needed to reach the maxMoveSpeed
        float velocityNeeded = maxMoveSpeed - Mathf.Abs(rotatedVelocity);

        // Limit the velocity to not exceed the movementAcceleration
        float velocityToApply = Mathf.Min(velocityNeeded, movementAcceleration);

        if (moveAxis.x != 0 && Mathf.Abs(rotatedVelocity) < maxMoveSpeed)
        {
            rb.velocity += direction * velocityToApply;
        }
    }

    private void LinearDrag()
    {
        float linearDrag;
        if (canGroundJump)
        { linearDrag = groundedDrag; }
        else
        { linearDrag = midAirDrag; }

        float localVelocityX = transform.InverseTransformDirection(rb.velocity).x;

        // Apply drag if the character is changing direction, or if its speed exceeds the maximum speed
        if (Mathf.Abs(moveAxis.x) < 0.4f || changingDirection || Mathf.Abs(localVelocityX) > maxMoveSpeed + .2f)
        {
            appliedDrag = linearDrag;
        }
        else
        {
            appliedDrag = 0f;
        }
    }

    private void ApplyLinearDrag()
    {
        // Convert the velocity to local space
        Vector2 localVelocity = transform.InverseTransformDirection(rb.velocity);

        // Apply drag on the local x-axis
        localVelocity.x /= (1f + appliedDrag / 50);

        // Convert the velocity back to world space
        rb.velocity = transform.TransformDirection(localVelocity);
    }

    private void FallMultiplier()
    {
        if (rb.velocity.y < 1f)
        {
            gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0f && playerInput.actions["Jump"].ReadValue<float>() == 0)
        {
            gravityScale = lowFallMultiplier;
        }
        else
        {
            gravityScale = 2f;
        }
    }

    //INPUTS

    private void GetInput()
    {
        moveAxis = playerInput.actions["Movement"].ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            /*
            Collider2D[] tempCol;
            tempCol = Physics2D.OverlapCircleAll(transform.position, 10f, 1 << 6);

            for (int i = 0; i < tempCol.Length; i++)
            {
                EnemyTest script = tempCol[i].gameObject.GetComponent<EnemyTest>();

                script.Flash(dashColor, .5f, 1);
            }
            */
            if (canGroundJump)
            {
                //anim.SetTrigger("jump");
                //sfx[0].Play();
                isDashing = true;
                gravityScale = 0f;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * groundJumpForce, ForceMode2D.Impulse);
                StartCoroutine(FlashColor(dashColor, dashDuration, dashColorAmount));
                Invoke("ResetGravityScale", .1f);

            }
            else if (wallJumpsLeft > 0 && canWallJump == true)
            {
                wallJumpsLeft--;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(wallJumpSide / 2f, 1) * wallJumpForce, ForceMode2D.Impulse);
            }
        }
    }

    void ResetGravityScale()
    {
        gravityScale = 1f;
        isDashing = false;
    }

    void FinishDash()
    {
        gravityScale = 1f;
        rb.velocity = new Vector2(maxMoveSpeed * lastDashAxis.x, rb.velocity.y * .2f);
        isDashing = false;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && dashesAvailable > 0)
        {
            Debug.Log("dashed");
            dashesAvailable -= 1;
            isDashing = true;
            //sfx[0].Play();
            gravityScale = 0f;
            rb.velocity = Vector2.zero;
            lastDashAxis = moveAxis;
            lastDashAxis.Normalize();
            rb.AddForce(lastDashAxis * dashStrength, ForceMode2D.Impulse);
            StartCoroutine(FlashColor(dashColor, dashDuration, dashColorAmount));
            Invoke("FinishDash", dashDuration);
        }
    }



    private IEnumerator FlashColor(Color color, float duration, float amount)
    {
        sr.material.color = color;
        float elapsedTime = 0f;
        while (elapsedTime < duration + .1f)
        {
            elapsedTime += Time.deltaTime;
            float mult = Mathf.Lerp(amount, 0, elapsedTime / .2f);

            mult = Mathf.Abs(mult);

            // Set the multiplier in the shader
            sr.material.SetFloat("_Multiplier", mult);
            yield return null; // Wait for the next frame
        }
        sr.material.SetFloat("_Multiplier", 0);
        Debug.Log("coloreset");
    }

    //ANIMATIONS


    // -- Animations method --

    private void FacingDirection()
    {
        if (!isFacingRight & rb.velocity.x > 0)
        { Flip(); }
        else if (isFacingRight & rb.velocity.x < 0)
        { Flip(); }
    }

    void Flip()
    {
        Vector2 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

}
