using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerManager : MonoBehaviour
{
    CharacterController characterController = null;
    PlayerInputs playerInputs = null;
    Transform cam = null;
    Animator playerAnimator = null;

    float moveSpeed = 5f;
    Vector3 velocity;
    float gravity = -9.81f;
    Vector2 inputVector = Vector2.zero;

    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    bool isAttacking = false;
    EnemyManager lockedOnEnemy = null;

    float attackCounter = 0f;
    float attackTime = 2f;
    float attackMoveSpeed = 0;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInputs = new PlayerInputs();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        playerInputs.Gameplay.Attack.performed += Attack;
        playerAnimator = GetComponentInChildren<Animator>();
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        playerInputs.Gameplay.Attack.performed -= Attack;
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    void Attack(InputAction.CallbackContext cxt)
    {
        if (!isAttacking && lockedOnEnemy != null)
        {
            isAttacking = true;

            float enemyDistance = Vector3.Distance(lockedOnEnemy.transform.position, transform.position) - 1.5f;
            attackMoveSpeed = enemyDistance / attackTime;
            attackMoveSpeed *= 2f;
            attackTime = enemyDistance / attackMoveSpeed;

            Vector3 holderVector = lockedOnEnemy.transform.position - transform.position;
            holderVector.Normalize();
            inputVector.x = holderVector.x;
            inputVector.y = holderVector.z;

            //play animation and sound
            playerAnimator.SetTrigger("Attack");
            FindAnyObjectByType<AudioManager>().PlaySound("Hitting");

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
            //read input
            inputVector = playerInputs.Gameplay.Move.ReadValue<Vector2>().normalized;
            HandleMovement();
        }
        else
        {
            HandleMovement();
            if(attackCounter >= attackTime)
            {
                isAttacking = false;
                attackCounter = 0;
                attackTime = 2f;
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
            if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, 6f))
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
            if (lockedOnEnemy != null && Vector3.Distance(lockedOnEnemy.transform.position, transform.position) > 6.0f)
            {
                lockedOnEnemy.turnOffIndicator();
                lockedOnEnemy = null;
            }
        }
    }

    void HandleMovement()
    {
        //check if movement should be done
        if(inputVector.magnitude >= 0.1f)
        {
            //calculate angle to turn
            float extraAngle = isAttacking ? 0f : cam.eulerAngles.y;
            float targetAngle = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + extraAngle;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //turn to that angle
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //convert angle to move direction
            Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            float speed = isAttacking ? attackMoveSpeed : moveSpeed;
            bool isRunning = playerAnimator.GetBool("isRunning");
            if (!isRunning)
            {
                playerAnimator.SetBool("isRunning", true);
                FindAnyObjectByType<AudioManager>().PlaySound("Footsteps");
            }
            characterController.Move(moveDir * speed * Time.deltaTime);
        }
        else
        {
            bool isRunning = playerAnimator.GetBool("isRunning");
            if (isRunning)
            {
                playerAnimator.SetBool("isRunning", false);
                FindAnyObjectByType<AudioManager>().StopSound("Footsteps");
            }
        }

        //add gravity to stay on ground
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }
    }

    public void KickSummit()
    {
        lockedOnEnemy.TakeDamage(30f);
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        if(newGameState == GameState.Paused)
        {
            playerInputs.Gameplay.Disable();
        }
        else
        {
            playerInputs.Gameplay.Enable();
        }
    }
}
