using UnityEngine;
using Controller;

public class ThirdPersonCamera : PlayerCamera
{
    [SerializeField, Range(0f, 2f)]
    private float m_Offset = 1.5f;
    [SerializeField, Range(0f, 10f)]
    private float m_SmoothTime = 0.1f; // tiempo de suavizado

    private Vector3 m_LookPoint;
    private Vector3 m_TargetPos;

    private Vector3 velocity = Vector3.zero; // para SmoothDamp
    private Quaternion rotVelocity;          // para suavizar rotación

    private void LateUpdate()
    {
        Move();
    }

    public override void SetInput(in Vector2 delta, float scroll)
    {
        base.SetInput(delta, scroll);

        var dir = new Vector3(0, 0, -m_Distance);
        var rot = Quaternion.Euler(m_Angles.x, m_Angles.y, 0f);

        var playerPos = (m_Player == null) ? Vector3.zero : m_Player.position;
        m_LookPoint = playerPos + m_Offset * Vector3.up;
        m_TargetPos = m_LookPoint + rot * dir;
    }

    private void Move()
    {
        // Suaviza la posición
        m_Transform.position = Vector3.SmoothDamp(
            m_Transform.position,
            m_TargetPos,
            ref velocity,
            m_SmoothTime
        );

        // Suaviza la rotación hacia el punto de mira
        Quaternion targetRot = Quaternion.LookRotation(m_LookPoint - m_Transform.position);
        m_Transform.rotation = Quaternion.Slerp(
            m_Transform.rotation,
            targetRot,
            Time.deltaTime * 5f // velocidad de rotación
        );

        // Actualiza el "target" auxiliar
        if (m_Target != null)
        {
            m_Target.position = m_Transform.position + m_Transform.forward * TargetDistance;
        }
    }
}
