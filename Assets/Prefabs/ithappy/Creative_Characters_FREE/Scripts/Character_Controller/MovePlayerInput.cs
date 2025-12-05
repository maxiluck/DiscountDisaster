using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CharacterMover))]
    public class MovePlayerInput : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField]
        private string m_HorizontalAxis = "Horizontal";
        [SerializeField]
        private string m_VerticalAxis = "Vertical";
        [SerializeField]
        private string m_JumpButton = "Jump";
        [SerializeField]
        private KeyCode m_RunKey = KeyCode.LeftShift;

        [Header("Camera")]
        [SerializeField]
        private PlayerCamera m_Camera;
        [SerializeField]
        private string m_MouseX = "Mouse X";
        [SerializeField]
        private string m_MouseY = "Mouse Y";
        [SerializeField]
        private string m_MouseScroll = "Mouse ScrollWheel";

        [SerializeField] UIManager UIManager;

        private CharacterMover m_Mover;

        private Vector2 m_Axis;
        private bool m_IsRun;
        private bool m_IsJump;

        private Vector3 m_Target;
        private Vector2 m_MouseDelta;
        private float m_Scroll;

        private void Awake()
        {
            m_Mover = GetComponent<CharacterMover>();

            if(m_Camera == null ) 
            {
                m_Camera = Camera.main == null ? null : Camera.main.GetComponent<PlayerCamera>();
            }
            if(m_Camera != null) {
                m_Camera.SetPlayer(transform);
            }
        }

        private void Update()
        {
            GatherInput();
            SetInput();
            if (Input.GetKeyDown(KeyCode.Escape))
            UIManager.ShowPauseMenu();
        }

        public void GatherInput()
        {
            m_Axis = new Vector2(Input.GetAxis(m_HorizontalAxis), Input.GetAxis(m_VerticalAxis));
            m_IsRun = Input.GetKey(m_RunKey);
            m_IsJump = Input.GetButton(m_JumpButton);

            // Dirección de referencia: la cámara
            if (m_Camera != null)
            {
                // forward y right de la cámara en el plano XZ
                Vector3 camForward = m_Camera.transform.forward;
                camForward.y = 0;
                camForward.Normalize();

                Vector3 camRight = m_Camera.transform.right;
                camRight.y = 0;
                camRight.Normalize();

                // Convertir input local (WASD) a dirección relativa a la cámara
                Vector3 moveDir = camForward * m_Axis.y + camRight * m_Axis.x;

                m_Target = moveDir; // ahora el jugador avanza hacia donde mira la cámara
            }
            else
            {
                m_Target = Vector3.zero;
            }

            m_MouseDelta = new Vector2(Input.GetAxis(m_MouseX), Input.GetAxis(m_MouseY));
            m_Scroll = Input.GetAxis(m_MouseScroll);
        }


        public void BindMover(CharacterMover mover)
        {
            m_Mover = mover;
        }

        public void SetInput()
        {
            if (m_Mover != null)
            {
                m_Mover.SetInput(in m_Axis, in m_Target, in m_IsRun, m_IsJump);
            }

            if (m_Camera != null)
            {
                m_Camera.SetInput(in m_MouseDelta, m_Scroll);
            }
        }
    }
}