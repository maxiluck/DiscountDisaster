using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float pushForce = 5f;

    public float fuerzaEmpuje = 500f;
    public float duracionRagdoll = 1f;

    void OnCollisionEnter(Collision collision)
    {
        // Empujar rigidbodies
        Rigidbody rb = collision.rigidbody;
        if (rb != null && !rb.isKinematic)
        {
            Vector3 pushDir = collision.contacts[0].normal * -1f;
            rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
        }

        ClienteIA clienteIA = collision.gameObject.GetComponentInParent<ClienteIA>();
            if (clienteIA != null)
            {
                clienteIA.ImpactadoPorItem(fuerzaEmpuje, duracionRagdoll, transform.up);
            }

        // Empujar CharacterControllers
        CharacterController cc = collision.gameObject.GetComponent<CharacterController>();
        if (cc != null)
        {
            Vector3 pushDir = collision.contacts[0].normal * -1f;
            cc.Move(pushDir * pushForce * Time.deltaTime);
        }
    }
}

