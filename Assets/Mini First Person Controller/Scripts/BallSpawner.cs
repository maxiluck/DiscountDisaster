using System.Collections.Generic;
using UnityEngine;

public class BallPoolSpawner : MonoBehaviour
{
    [Header("Prefab y Pool")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private int poolSize = 20;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float fuerzaLanzamiento = 10f;
    [SerializeField] private float tiempoVida = 3f; // cuánto dura activa
    [SerializeField] private float intervaloDisparo = 1f; // cada cuánto dispara

    private List<GameObject> pool;
    private float timer;

    void Start()
    {
        // Crear pool
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject ball = Instantiate(ballPrefab);
            ball.SetActive(false);
            pool.Add(ball);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= intervaloDisparo)
        {
            Disparar();
            timer = 0f;
        }
    }

    private void Disparar()
{
    GameObject ball = ObtenerDelPool();
    if (ball != null)
    {
        ball.transform.position = spawnPoint.position;
        ball.transform.rotation = spawnPoint.rotation;
        ball.SetActive(true);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Resetear física para que no arrastre valores viejos
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // 👉 Dar fuerza hacia adelante
            rb.AddForce(spawnPoint.forward * fuerzaLanzamiento, ForceMode.Impulse);
        }

        StartCoroutine(DesactivarDespues(ball, tiempoVida));
    }
}


    private GameObject ObtenerDelPool()
    {
        foreach (var ball in pool)
        {
            if (!ball.activeInHierarchy)
                return ball;
        }
        return null; // si todas están ocupadas
    }

    private System.Collections.IEnumerator DesactivarDespues(GameObject ball, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        ball.SetActive(false);
    }

}


