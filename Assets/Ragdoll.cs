using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ragdoll : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pelvis;

    private Rigidbody rootRb;                 // 👈 Rigidbody del objeto raíz (donde está el Agent)
    private Rigidbody[] ragdollBodies;
    private Vector3 destinoGuardado;          // opcional: último destino
    private bool ragdollActivo = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rootRb = GetComponent<Rigidbody>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();

        // El root debe ser kinematic para no pelear con el Agent
        if (rootRb != null) rootRb.isKinematic = true;

        SetEnabled(false);
    }

    public void GuardarDestino(Vector3 destino)
    {
        destinoGuardado = destino;
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.SetDestination(destinoGuardado);
    }

    void SetEnabled(bool enabled)
    {
        ragdollActivo = enabled;
        bool isKinematic = !enabled;

        // Huesos: togglear físicas
        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb == rootRb) continue; // no tocar el root
            rb.isKinematic = isKinematic;
            if (!enabled) rb.velocity = Vector3.zero;
        }

        // Animator
        animator.enabled = !enabled;

        // Agent: apagar durante ragdoll, prender al salir
        if (agent != null)
            agent.enabled = !enabled;

        // Al salir del ragdoll: recolocar y preparar navegación
        if (!enabled && pelvis != null)
        {
            // Mover el root a la pelvis (posición/rotación)
            transform.position = pelvis.position;
            transform.rotation = Quaternion.Euler(0, pelvis.rotation.eulerAngles.y, 0);

            // Asegurar NavMesh y warp seguro
            if (agent != null)
            {
                NavMeshHit hit;
                // Samplear cerca por si quedó fuera de la malla
                if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
                {
                    // Warp para alinear el agente con la nueva posición
                    agent.Warp(hit.position);
                }

                // Desbloquear y recomputar
                agent.isStopped = false;
                agent.ResetPath();

                // Si tenías un destino guardado, volver a setearlo
                if (destinoGuardado != Vector3.zero)
                    agent.SetDestination(destinoGuardado);
            }
        }
    }

    void Update()
    {
        
    }

    public void ActivarRagdoll(float fuerza, float duracion, Vector3 direccion)
    {
        SetEnabled(true);

        // Empuje a todos los huesos (no al root)
        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb == rootRb) continue;
            rb.AddForce(direccion * fuerza, ForceMode.Impulse);
        }

        StartCoroutine(DesactivarDespues(duracion));
    }

    private IEnumerator DesactivarDespues(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);

        SetEnabled(false);

        // Esperar un frame y disparar GetUp
        if (animator != null)
        {
            yield return null;
            animator.SetTrigger("GetUp");
        }
    }
}





