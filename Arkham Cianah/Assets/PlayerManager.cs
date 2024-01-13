using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    CharacterController characterController = null;
    PlayerInputs playerInputs = null;
    Transform cam = null;

    float moveSpeed = 5f;
    Vector3 velocity;
    float gravity = -9.81f;

    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    bool isAttacking = false;
    EnemyManager lockedOnEnemy = null;
    float attackCounter = 0f;
    float attackTime = 2f;
    float attackMoveSpeed = 0;
    Vector3 attackDir = Vector3.zero;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInputs = new PlayerInputs();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        playerInputs.Gameplay.Attack.performed += Attack;
    }

    void Attack(InputAction.CallbackContext cxt)
    {
        if(!isAttacking && lockedOnEnemy != null)
        {
            isAttacking = true;

            float enemyDistance = Vector3.Distance(lockedOnEnemy.transform.position, transform.position) - 1;
            attackMoveSpeed = enemyDistance / attackTime;

            attackDir = lockedOnEnemy.transform.position - transform.position;
            attackDir.Normalize();
            attackDir.y = 0f;

            //if near punch distance, start punch animation
            //else start the flying kick animation
            //all animation will take similar time to execute
        }
    }

    private void OnEnable()
    {
        playerInputs.Gameplay.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Gameplay.Disable();
    }

    private void FixedUpdate()
    {
        if (playerInputs != null && !isAttacking)
        {
            HandleMovement();
        }
        else
        {
            HandleAttackMovement();
            if(attackCounter >= attackTime)
            {
                isAttacking = false;
                attackCounter = 0;
            }
            else
            {
                attackCounter += Time.deltaTime;
            }
        }
    }

    private void Update()
    {
        if(!isAttacking)
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, 4f))
            {
                if (hit.transform.tag == "Enemy" && !GameObject.ReferenceEquals(lockedOnEnemy, hit.collider.gameObject))
                {
                    if (lockedOnEnemy != null)
                    {
                        lockedOnEnemy.turnOffIndicator();
                    }
                    lockedOnEnemy = hit.transform.GetComponent<EnemyManager>();
                    lockedOnEnemy.turnOnIndicator();
                }
            }
            if (lockedOnEnemy != null && Vector3.Distance(lockedOnEnemy.transform.position, transform.position) > 4.0f)
            {
                lockedOnEnemy.turnOffIndicator();
                lockedOnEnemy = null;
            }
        }
    }

    void HandleMovement()
    {
        //read input
        Vector2 inputVector = playerInputs.Gameplay.Move.ReadValue<Vector2>().normalized;

        //check if the player moved the person
        if(inputVector.magnitude >= 0.1f && !isAttacking)
        {
            //calculate angle to turn
            float targetAngle = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //turn to that angle
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //convert angle to move direction
            Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            characterController.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        //add gravity to stay on ground
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }
    }

    void HandleAttackMovement()
    {
        //add gravity to stay on ground
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }

        //move based on the speed it is provided 
    }
}
