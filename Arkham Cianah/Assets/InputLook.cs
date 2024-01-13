using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputLook : MonoBehaviour
{
    PlayerInputs playerInputs = null;
    CinemachineFreeLook cineCam = null;
    bool invertY = false;
    float lookSpeed = 1;
    float mouseXSense;
    float mouseYSense;

    public void ChangeMouseSense(float senseX, float senseY)
    {
        mouseXSense = senseX;
        mouseYSense = senseY;

        cineCam.m_XAxis.m_MaxSpeed = mouseXSense;
        cineCam.m_YAxis.m_MaxSpeed = mouseYSense;
    }

    void HandleLook()
    {
        Vector2 look = playerInputs.Gameplay.Look.ReadValue<Vector2>().normalized;
        look.y = invertY ? -look.y : look.y;
        look.x = look.x * 180f;

        cineCam.m_XAxis.Value += look.x * lookSpeed * Time.deltaTime;
        cineCam.m_YAxis.Value += look.y * lookSpeed * Time.deltaTime;
    }

    private void Awake()
    {
        cineCam = GetComponent<CinemachineFreeLook>();
        playerInputs = new PlayerInputs();
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
        HandleLook();
    }
}
