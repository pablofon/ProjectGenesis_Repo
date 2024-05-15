using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
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
    [SerializeField] Vector2 mousePos;

    [SerializeField] Vector2 aimDir;

    [Header("Movement")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float groundAccel;
    [SerializeField] private float airAccel;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float groundedDrag;
    [SerializeField] private float midAirDrag;
    private float appliedDrag;
    private bool changingDirection => ((rb.velocity.x > 0f && moveAxis.x < 0f) || (rb.velocity.x < 0f && moveAxis.x > 0f));

    [Header("Physics")]
    [SerializeField] Vector2 gravityDirection;
    [SerializeField] float gravityScale;
    [SerializeField] float groundedAreaLength;
    [SerializeField] float groundedAreaHeight;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    [Header("Jump")]
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowFallMultiplier;
    [SerializeField] private bool gravityOff;
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
    private Vector2 lastMoveAxis;
    private bool isDashing;

    private void Awake()
    {
        //set current health
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        foreach (var rigLayer in rigBuilder.layers)
        {
            armLayer = rigLayer;
        }

        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        GetInput();
        FacingDirection();
        CursorAim();
        ArmTransform();

        if (canGroundJump) { dashesAvailable = maxDashes; wallJumpsLeft = wallJumpsMax; }
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        Gravity();
        Movement();
        HorizontalDrag();
        VerticalDrag();
        FallMultiplier();

        if (facingCursorTimer > 0) facingCursorTimer -= Time.deltaTime;
    }

    void Gravity()
    {
        if (!gravityOff && !isDashing)
        {
            rb.AddForce(-gravityDirection * gravityScale);
        }
    }

    private Vector2 collisionNormal;
    private float collisionAngle;
    void CheckCollisions()
    {
        if (col.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
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
                canWallJump = false;
                rb.rotation = collisionAngle;
            }
            else
            {
                canWallJump = true;
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

        Vector2 localVel = transform.InverseTransformDirection(rb.velocity);

        if (!gravityOff && !isDashing)
        {
            if (moveAxis.x != 0 && Mathf.Sign(localVel.x) == -Mathf.Sign(moveAxis.x) || Mathf.Abs(localVel.x) < maxMoveSpeed)
            {
                // Calculate the velocity needed to reach the maxMoveSpeed
                float velocityNeeded = maxMoveSpeed - Mathf.Abs(localVel.x);

                // Limit the velocity to not exceed the movementAcceleration
                float velocityToApply = Mathf.Min(velocityNeeded, movementAcceleration);

                if (velocityToApply <= 0) { velocityToApply = movementAcceleration; }

                rb.velocity += direction * velocityToApply;
            }
        }
    }

    private void HorizontalDrag()
    {
        Vector2 localVel = transform.InverseTransformDirection(rb.velocity);

        float linearDrag;
        if (canGroundJump)
        {
            linearDrag = groundedDrag;
            movementAcceleration = groundAccel;
        }
        else
        {
            linearDrag = midAirDrag;
            movementAcceleration = airAccel;
        }

        // Apply drag if the character is changing direction, or if its speed exceeds the maximum speed
        if (Mathf.Abs(moveAxis.x) < 0.4f)
        {
            appliedDrag = linearDrag;
        }
        else if (changingDirection && canGroundJump)
        {
            appliedDrag = linearDrag * 2;
        }
        else if (Mathf.Abs(localVel.x) > maxMoveSpeed + .1f)
        {
            appliedDrag = linearDrag * .1f;
        }
        else
        {
            appliedDrag = 0f;
        }

        if (!isDashing && !gravityOff)
        {
            // Apply drag on the local x-axis
            localVel.x /= (1f + appliedDrag / 50);

            // Convert the velocity back to world space
            rb.velocity = transform.TransformDirection(localVel);
        }
    }

    private void VerticalDrag()
    {
        Vector2 localVel = transform.InverseTransformDirection(rb.velocity);

        if (canWallJump && !gravityOff)
        {
            localVel.y *= .9f;
            rb.velocity = transform.TransformDirection(localVel);
        }
    }

    [SerializeField] bool jumped;
    private void FallMultiplier()
    {
        if (rb.velocity.y < 1f)
        {
            jumped = false;
            gravityScale = fallMultiplier;
        }
        else if (!jumped)
        {
            gravityScale = fallMultiplier;
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
        if (moveAxis.x != 0) lastMoveAxis.x = moveAxis.x;

        Vector2 screenPosition = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(screenPosition);

        //Calculate cursor angle relative to the player

        aimDir = (mousePos - new Vector2(shoulder.position.x, shoulder.position.y)).normalized;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canGroundJump)
            {
                //anim.SetTrigger("jump");
                //sfx[0].Play();
                gravityOff = true;
                jumped = true;
                rb.velocity = new Vector2(rb.velocity.x, groundJumpForce);
                StartCoroutine(FlashColor(dashColor, dashDuration, dashColorAmount));
                Invoke("ResetGravityScale", .1f);
            }
            else if (wallJumpsLeft > 0 && canWallJump == true)
            {
                gravityOff = true;
                jumped = true;
                wallJumpsLeft--;
                rb.velocity = new Vector2(rb.velocity.x + wallJumpSide * 10f, wallJumpForce);
                StartCoroutine(FlashColor(dashColor, dashDuration, dashColorAmount));
                Invoke("ResetGravityScale", .1f);
            }
        }
        if (context.canceled && jumped)
        {
            jumped = false;
            rb.velocity *= new Vector2(1, .5f);
        }
    }

    void ResetGravityScale()
    {
        gravityScale = 1f;
        gravityOff = false;
    }

    void FinishDash()
    {
        gravityScale = 1f;
        rb.velocity = new Vector2(maxMoveSpeed * lastMoveAxis.x, rb.velocity.y * .2f);
        isDashing = false;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && dashesAvailable > 0)
        {
            dashesAvailable -= 1;
            isDashing = true;
            //sfx[0].Play();
            rb.velocity = new Vector2(lastMoveAxis.x * dashStrength, 0);
            StartCoroutine(FlashColor(dashColor, dashDuration, dashColorAmount));
            Invoke("FinishDash", dashDuration);
        }
    }

    [SerializeField] private float facingCursorTimer;
    [SerializeField] private float facingCursorDuration;
    public void Attack(InputAction.CallbackContext context)
    {
        //Execute attack code
        facingCursorTimer = facingCursorDuration;
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
    }

    //ANIMATIONS

    //ARM DIRECTION

    //Apply cursor direction or controller joystick direction
    private void CursorAim()
    {
        //MOUSE: Screen to world pos
        Vector2 screenPosition = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(screenPosition);

        //Calculate cursor angle relative to the player
        aimDir = (mousePos - new Vector2(shoulder.position.x, shoulder.position.y)).normalized;
    }

    [Header("Arm Transform")]
    [SerializeField] Transform shoulder;
    [SerializeField] float aimAngle;
    [SerializeField] Transform handController;
    [SerializeField] private float hcDist;
    public float hcKbDuration;
    public float hcKbDist;
    [SerializeField] float hcKbDistOutput = 1f;
    public float hcKbRotation;
    [SerializeField] float hcKbRotationtOutput;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] RigLayer armLayer;
    [SerializeField] GameObject weaponsContainer;

    //Orient the arm
    private void ArmTransform()
    {

        aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) / Mathf.PI * 180;

        //Hand controller ROTATION
        handController.eulerAngles = new Vector3(handController.eulerAngles.x, handController.eulerAngles.y, aimAngle + hcKbRotationtOutput);

        //Hand controller POSITION
        Vector2 hcPos = hcDist * hcKbDistOutput * aimDir;
        handController.position = new Vector3(shoulder.position.x + hcPos.x, shoulder.position.y + hcPos.y);

        if (facingCursorTimer > 0)
        {
            if (!armLayer.active)
            {
                armLayer.active = true;
                weaponsContainer.SetActive(true);
            }
        }
        else if (armLayer.active)
        {
            armLayer.active = false;
            weaponsContainer.SetActive(false);
        }
    }



    //Hand recoil
    public IEnumerator HcDistKb()
    {
        float elapsedTime = 0;
        try
        {
            while (elapsedTime < hcKbDuration)
            {
                elapsedTime += Time.deltaTime;
                float mult = Mathf.Lerp(0, 1, elapsedTime / hcKbDuration);
                hcKbDistOutput = Mathf.Lerp(hcKbDist, 1, mult);
                hcKbRotationtOutput = hcKbRotation * (1 - mult);
                yield return null; // Yield execution to the next frame
            }
        }
        finally
        {
            hcKbDistOutput = 1;
            hcKbRotationtOutput = 0;
        }
    }

    // -- Animations method --

    private void FacingDirection()
    {
        if (facingCursorTimer > 0)
        {
            if (mousePos.x > transform.position.x && !isFacingRight) Flip();
            if (mousePos.x < transform.position.x && isFacingRight) Flip();
        }
        else
        {
            if (!isFacingRight & rb.velocity.x > 0) Flip();
            else if (isFacingRight & rb.velocity.x < 0) Flip();
        }
    }

    void Flip()
    {
        Vector2 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        foreach (GameObject weapon in weapons)
        {
            weapon.transform.localScale = new Vector3(weapon.transform.localScale.x, -1 * weapon.transform.localScale.y, weapon.transform.localScale.z);
        }

        isFacingRight = !isFacingRight;
    }

}
