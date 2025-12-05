using UnityEngine;

public class Soplador : MonoBehaviour
{
    public float fuerzaEmpuje = 500f;
    public float duracionRagdoll = 1f;

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            ActivarSoplador();
        }
    }

    void ActivarSoplador()
    {
        Collider[] afectados = Physics.OverlapBox(transform.position, transform.localScale / 2, transform.rotation);

        foreach (Collider col in afectados)
        {
            ClienteIA clienteIA = col.GetComponentInParent<ClienteIA>();
            if (clienteIA != null)
            {
                clienteIA.ImpactadoPorItem(fuerzaEmpuje, duracionRagdoll, transform.up);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, transform.localScale);
    }
}

