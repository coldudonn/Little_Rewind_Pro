using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    [Header("Action Z")]
    [SerializeField] KeyCode actionKeyZ = KeyCode.Z;
    [SerializeField] string actionTriggerNameZ = "ActionZ";   // Animator Trigger 이름과 동일하게
    [SerializeField] float actionCooldownZ = 0.2f;            // 연타 방지(원하면 0)

    [Header("Action X")]
    [SerializeField] KeyCode actionKeyX = KeyCode.X;
    [SerializeField] string actionTriggerNameX = "ActionX";   // Animator Trigger 이름과 동일하게
    [SerializeField] float actionCooldownX = 0.2f;            // 연타 방지(원하면 0)

    [Header("Action Common")]
    [SerializeField] bool blockActionsWhileInAir = true;      // 공중에서는 액션 금지(원하면 false)

    bool isGrounded;
    bool hasControl = true;

    float ySpeed;
    Quaternion targetRotation;

    float nextActionTimeZ = 0f;
    float nextActionTimeX = 0f;

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));
        var moveInput = (new Vector3(h, 0, v)).normalized;
        var moveDir = cameraController.PlanarRotation * moveInput;

        if (!hasControl) return;

        GroundCheck();

        // ✅ Z 액션
        if (Input.GetKeyDown(actionKeyZ) && Time.time >= nextActionTimeZ)
        {
            if (!blockActionsWhileInAir || isGrounded)
            {
                animator.SetTrigger(actionTriggerNameZ);
                nextActionTimeZ = Time.time + actionCooldownZ;
            }
        }

        // ✅ X 액션
        if (Input.GetKeyDown(actionKeyX) && Time.time >= nextActionTimeX)
        {
            if (!blockActionsWhileInAir || isGrounded)
            {
                animator.SetTrigger(actionTriggerNameX);
                nextActionTimeX = Time.time + actionCooldownX;
            }
        }

        // 점프
        if (isGrounded && Input.GetButtonDown("Fire1"))
        {
            ySpeed = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            animator.SetTrigger("Jump");
        }

        // 지면 보정 / 중력
        if (isGrounded && ySpeed < 0f)
        {
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        var velocity = moveDir * moveSpeed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0f)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        animator.SetFloat("moveAmount", moveAmount, 0.2f, Time.deltaTime);
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(
            transform.TransformPoint(groundCheckOffset),
            groundCheckRadius,
            groundLayer
        );
    }

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if (!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targetRotation = transform.rotation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    public float RotationSpeed => rotationSpeed;
}
