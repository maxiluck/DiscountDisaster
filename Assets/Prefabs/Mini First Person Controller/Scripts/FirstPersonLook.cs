using UnityEngine;
using Controller;

public class FirstPersonLook : PlayerCamera
{
    [SerializeField] private Transform character;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float smoothing = 1.5f;

    private Vector2 velocity;
    private Vector2 frameVelocity;

    protected override void Awake()
    {
        base.Awake();
        if (character == null)
        {
            character = GetComponentInParent<FirstPersonMovement>()?.transform;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void SetInput(in Vector2 delta, float scroll)
    {
        base.SetInput(delta, scroll);

        // Suavizado del mouse
        Vector2 rawFrameVelocity = delta * sensitivity;
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1f / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90f, 90f);
    }

    private void LateUpdate()
    {
        // Rotación vertical (cámara)
        m_Transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);

        // Rotación horizontal (personaje)
        if (character != null)
        {
            character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
        }

        // Actualiza el target auxiliar para IK / mover
        if (m_Target != null)
        {
            // Usar forward de la cámara pero solo en el plano XZ
            Vector3 flatForward = m_Transform.forward;
            flatForward.y = 0f;              // ignorar inclinación vertical
            flatForward.Normalize();

            m_Target.position = m_Transform.position + flatForward * TargetDistance;
        }
    }
}

