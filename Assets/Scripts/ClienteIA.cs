using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ClienteIA : MonoBehaviour
{
    // --- Navegación y animación ---
    private NavMeshAgent agent;
    private Rigidbody rootRb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pelvis;

    private AnimationHandler animationHandler;
    public Vector3 destinoGuardado;
    private bool haComenzado = false;
    private bool finalizado = false;

    // --- Ragdoll ---
    private Rigidbody[] ragdollBodies;
    private bool ragdollActivo = false;

    // --- Paciencia ---
    private float tiempoEsperando = 0f;
    public float maxPaciencia = 5f;

    // --- Tiempo máximo de vida ---
    private float tiempoActivo = 0f;
    public float maxTiempoActivo = 180;

    // --- IK ---
    public Transform salidaSuper;
    public Transform producto;
    public Transform lookTarget;
    [Header("IK Settings")]
    public LookWeight lookWeight = new LookWeight(1f, 0.3f, 0.7f, 1f);

    private bool expulsado = false;
    private bool enSalida = false;

    // --- UI Cliente ---
    [Header("UI Cliente")]
    [SerializeField] private Image emojiImage;   // referencia al Image del Canvas
    [SerializeField] private Sprite felizSprite;
    [SerializeField] private Sprite medioSprite;
    [SerializeField] private Sprite frustradoSprite;
    [SerializeField] private CanvasGroup barraGroup;
    [SerializeField] private Transform jugador;
    public float distanciaVisible = 20f;

    // --- Manager ---
    private RoundManager roundManager;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rootRb = GetComponent<Rigidbody>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();

        if (rootRb != null) rootRb.isKinematic = true;

        animationHandler = new AnimationHandler(animator, "Hor", "Vert", "State", "IsJump");
        SetRagdoll(false);

        roundManager = FindObjectOfType<RoundManager>();
    }

    void Start()
    {
        // Buscar jugador automáticamente si no está asignado
        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) jugador = playerObj.transform;
        }
    }

    // --- Inicialización ---
    public void InitCliente(float pacienciaMin, float pacienciaMax)
    {
        maxPaciencia = Random.Range(pacienciaMin, pacienciaMax);
        tiempoEsperando = 0f;
        tiempoActivo = 0f;
        finalizado = false;
        haComenzado = false;
        destinoGuardado = Vector3.zero;
        expulsado = false;
        enSalida = false;
        jugador = Camera.main.transform;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
        }
    }

    // --- Destino ---
    public void SetDestino(Transform target)
    {
        if (agent != null && agent.enabled)
        {
            agent.isStopped = false;
            destinoGuardado = target.position;
            agent.SetDestination(destinoGuardado);
        }
    }

    void Update()
{
    // --- Rotación del emoji hacia la cámara ---
    if (jugador != null && barraGroup != null)
    {
        Vector3 dir = jugador.position - barraGroup.transform.position;
        dir.y = 0;
        barraGroup.transform.rotation = Quaternion.LookRotation(-dir);
    }

    // --- Paciencia y frustración SOLO si no finalizó ---
    if (!finalizado)
    {
        bool quiereMoverse = agent != null && agent.enabled && agent.hasPath && agent.desiredVelocity.magnitude > 0.1f;
        bool casiNoAvanza = agent.velocity.magnitude < 0.3f;
        bool lejosDelDestino = agent.remainingDistance > agent.stoppingDistance + 0.5f;
        bool pathParcial = agent.pathStatus == NavMeshPathStatus.PathPartial;

        bool estaBloqueado = (quiereMoverse && casiNoAvanza && lejosDelDestino) || pathParcial;

        if (estaBloqueado)
        {
            tiempoEsperando += Time.deltaTime;
            float ratio = Mathf.Clamp01(tiempoEsperando / maxPaciencia);

            if (emojiImage != null)
            {
                if (ratio < 0.33f) emojiImage.sprite = felizSprite;
                else if (ratio < 0.66f) emojiImage.sprite = medioSprite;
                else emojiImage.sprite = frustradoSprite;
            }

            if (tiempoEsperando >= maxPaciencia)
            {
                ClienteFrustrado();
            }
        }
        else if (agent.desiredVelocity.magnitude > 1f && agent.velocity.magnitude > 1f)
        {
            //tiempoEsperando = 0f;
        }
    }

    // --- Fade de visibilidad (siempre activo) ---
    if (jugador != null && barraGroup != null)
    {
        float dist = Vector3.Distance(transform.position, jugador.position);
        float targetAlpha = dist <= distanciaVisible ? 1f : 0f;
        barraGroup.alpha = Mathf.Lerp(barraGroup.alpha, targetAlpha, Time.deltaTime * 5f);
    }

    // --- Animación y llegada al destino (siempre activo) ---
    if (agent != null && agent.enabled && agent.isOnNavMesh)
    {
        Vector3 localVel = transform.InverseTransformDirection(agent.velocity);
        Vector2 axis = new Vector2(localVel.x, localVel.z);
        float state = agent.velocity.magnitude > 0.1f ? 1f : 0f;
        animationHandler.Animate(in axis, state, false, Time.deltaTime);

        if (agent.velocity.magnitude > 0.1f) haComenzado = true;

        bool pathListo = !agent.pathPending && agent.hasPath;
        bool llegoAlDestino = haComenzado && pathListo && agent.remainingDistance <= agent.stoppingDistance + 0.05f;

        if (llegoAlDestino)
        {
            bool juntoAlProducto = (producto != null) &&
                Vector3.Distance(transform.position, producto.position) <= agent.stoppingDistance + 0.05f;

            if (!expulsado && juntoAlProducto && !finalizado)
            {
                ClienteSatisfecho();
            }
            else if (salidaSuper != null && Vector3.Distance(agent.destination, salidaSuper.position) <= 0.05f)
            {
                if (Vector3.Distance(transform.position, salidaSuper.position) <= agent.stoppingDistance + 0.1f)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                agent.SetDestination(destinoGuardado);
            }
        }
    }
}




    // --- Estados cliente ---
    void ClienteSatisfecho()
    {
        emojiImage.sprite = felizSprite;
        finalizado = true;
        Debug.Log("Cliente satisfecho");
        if (roundManager != null) roundManager.RegistrarClienteSatisfecho();

        destinoGuardado = salidaSuper.position;
        agent.SetDestination(destinoGuardado);
    }

    void ClienteFrustrado()
    {
        // 👇 fijar cara enojada y no volver a cambiarla
        if (emojiImage != null) 
        {emojiImage.sprite = frustradoSprite;}
        finalizado = true;
        enSalida = true;
        Debug.Log("Cliente frustrado");
        if (roundManager != null) roundManager.RegistrarClienteFrustrado();

        destinoGuardado = salidaSuper.position;
        agent.SetDestination(destinoGuardado);
    }

    // --- Impacto de ítems ---
    public void ImpactadoPorItem(float fuerza, float duracionRagdoll, Vector3 direccion)
    {
        ActivarRagdoll(fuerza, duracionRagdoll, direccion);
        expulsado = true;
    }

    // --- RAGDOLL ---
    public void ActivarRagdoll(float fuerza, float duracion, Vector3 direccion)
    {
        SetRagdoll(true);

        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb == rootRb) continue;
            rb.AddForce(direccion * fuerza, ForceMode.Impulse);
        }

        StartCoroutine(DesactivarDespues(duracion));
    }

    void SetRagdoll(bool enabled)
    {
        ragdollActivo = enabled;
        bool isKinematic = !enabled;

        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb == rootRb) continue;
            rb.isKinematic = isKinematic;
            if (!enabled) rb.velocity = Vector3.zero;
        }

        animator.enabled = !enabled;
        if (agent != null) agent.enabled = !enabled;

        if (!enabled && pelvis != null)
        {
            transform.position = pelvis.position;
            transform.rotation = Quaternion.Euler(0, pelvis.rotation.eulerAngles.y, 0);

            if (agent != null)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
                    agent.Warp(hit.position);

                agent.isStopped = false;
                agent.ResetPath();
                if (destinoGuardado != Vector3.zero)
                    agent.SetDestination(destinoGuardado);

                expulsado = false; // resetear expulsado
            }
        }
    }

    private IEnumerator DesactivarDespues(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);

        SetRagdoll(false);

        if (animator != null)
        {
            yield return null;
            animator.SetTrigger("GetUp");
        }

        if (agent != null && destinoGuardado != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
            agent.SetDestination(destinoGuardado);
        }
    }

    // --- ANIMATION HANDLER ---
    private class AnimationHandler
    {
        private readonly Animator m_Animator;
        private readonly string m_HorizontalID, m_VerticalID, m_StateID, m_JumpID;
        private readonly float k_InputFlow = 4.5f;
        private float m_FlowState;
        private Vector2 m_FlowAxis;

        public AnimationHandler(Animator animator, string horizontalID, string verticalID, string stateID, string jumpID)
        {
            m_Animator = animator;
            m_HorizontalID = horizontalID;
            m_VerticalID = verticalID;
            m_StateID = stateID;
            m_JumpID = jumpID;
        }

        public void Animate(in Vector2 axis, float state, bool isJump, float deltaTime)
        {
            m_Animator.SetFloat(m_HorizontalID, m_FlowAxis.x);
            m_Animator.SetFloat(m_VerticalID, m_FlowAxis.y);
            m_Animator.SetFloat(m_StateID, Mathf.Clamp01(m_FlowState));
            m_Animator.SetBool(m_JumpID, isJump);

            m_FlowAxis = Vector2.ClampMagnitude(
                m_FlowAxis + k_InputFlow * deltaTime * (axis - m_FlowAxis).normalized, 1f
            );
            m_FlowState = Mathf.Clamp01(
                m_FlowState + k_InputFlow * deltaTime * Mathf.Sign(state - m_FlowState)
            );
        }

        public void AnimateIK(in Vector3 target, in LookWeight lookWeight)
        {
            m_Animator.SetLookAtPosition(target);
            m_Animator.SetLookAtWeight(lookWeight.weight, lookWeight.body, lookWeight.head, lookWeight.eyes);
        }
    }

    [System.Serializable]
    public struct LookWeight
    {
        public float weight, body, head, eyes;
        public LookWeight(float weight, float body, float head, float eyes)
        {
            this.weight = weight; this.body = body; this.head = head; this.eyes = eyes;
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animationHandler != null && lookTarget != null)
            animationHandler.AnimateIK(lookTarget.position, lookWeight);
    }
}










