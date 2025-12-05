using System.Collections.Generic;
using UnityEngine;

public class ClienteSpawner : MonoBehaviour
{
    [Header("Pool de clientes")]
    [SerializeField] private GameObject clientePrefab;
    [SerializeField] private int poolSize = 20;

    [Header("Spawn")]
    public Transform[] spawnPoints;   // 👈 arrastrás los puntos reales de la escena
    public Transform destinoProducto; // 👈 arrastrás el destino real (ej. mostrador)
    public Transform salidaSuper;     // 👈 arrastrás la salida real (puerta de salida)
    public int cantidadPorOleada = 10;

    [Header("Meshes aleatorios")]
    public Mesh[] pantalonesMeshes;
    public Mesh[] zapatillasMeshes;
    public Mesh[] pelosMeshes;
    public Mesh[] remeraMeshes;
    public Mesh[] caraMeshes;
    public Mesh[] lentesMeshes;
    public Mesh[] guantesMeshes;

    private List<GameObject> pool;

    void Start()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject cliente = Instantiate(clientePrefab);
            cliente.SetActive(false);
            pool.Add(cliente);
        }
    }

    public void SpawnOleada()
    {
        for (int i = 0; i < cantidadPorOleada; i++)
        {
            GameObject cliente = ObtenerDelPool();
            if (cliente != null)
            {
                int index = Random.Range(0, spawnPoints.Length);
                Transform punto = spawnPoints[index];

                cliente.transform.position = punto.position;
                cliente.transform.rotation = Quaternion.identity;
                cliente.SetActive(true);

                ClienteIA clienteIA = cliente.GetComponent<ClienteIA>();

                // 👇 Resetear estados internos
                clienteIA.InitCliente(2f, 6f); // por ejemplo paciencia entre 2 y 6 segundos

                clienteIA.SetDestino(destinoProducto);
                clienteIA.producto = destinoProducto;
                clienteIA.salidaSuper = salidaSuper;

                // 👉 Randomizar meshes
                RandomizarMeshes(cliente);
            }
        }
    }

    private GameObject ObtenerDelPool()
    {
        foreach (var cliente in pool)
        {
            if (!cliente.activeInHierarchy)
                return cliente;
        }
        return null;
    }

    private void RandomizarMeshes(GameObject cliente)
    {
        // Buscar los renderers en el prefab
        SkinnedMeshRenderer pantalones = cliente.transform.Find("Pants").GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer zapatillas = cliente.transform.Find("Shoes").GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer pelo = cliente.transform.Find("Hat").GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer remera = cliente.transform.Find("Outerwear").GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer cara = cliente.transform.Find("Faces").GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer lente = cliente.transform.Find("Glasses").GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer guante = cliente.transform.Find("Gloves").GetComponent<SkinnedMeshRenderer>();

        if (pantalones != null && pantalonesMeshes.Length > 0)
            pantalones.sharedMesh = pantalonesMeshes[Random.Range(0, pantalonesMeshes.Length)];

        if (zapatillas != null && zapatillasMeshes.Length > 0)
            zapatillas.sharedMesh = zapatillasMeshes[Random.Range(0, zapatillasMeshes.Length)];

        if (pelo != null && pelosMeshes.Length > 0)
            pelo.sharedMesh = pelosMeshes[Random.Range(0, pelosMeshes.Length)];

        if (remera != null && remeraMeshes.Length > 0)
            remera.sharedMesh = remeraMeshes[Random.Range(0, remeraMeshes.Length)];

        if (cara != null && caraMeshes.Length > 0)
            cara.sharedMesh = caraMeshes[Random.Range(0, caraMeshes.Length)];

        if (lente != null && lentesMeshes.Length > 0)
            lente.sharedMesh = lentesMeshes[Random.Range(0, lentesMeshes.Length)];

        if (guante != null && guantesMeshes.Length > 0)
            guante.sharedMesh = guantesMeshes[Random.Range(0, guantesMeshes.Length)];

    }
}






