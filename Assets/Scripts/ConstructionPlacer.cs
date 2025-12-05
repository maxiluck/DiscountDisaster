using UnityEngine;

public class ConstructionPlacer : MonoBehaviour
{
    private ConstruccionSO construccionSeleccionada;
    private GameObject previewInstance;
    [SerializeField] GameObject canva;
    private Renderer[] previewRenderers;

    [Header("Layers")]
    public LayerMask sueloMask;       // capa del suelo
    public LayerMask obstaculosMask;  // capa de objetos que bloquean
    public LayerMask sueloNoConstruibleMask; // capa de suelo donde no se puede construir

    [Header("Preview")]
    public Color colorValido = Color.blue;
    public Color colorInvalido = Color.red;
    public float raycastDistancia = 10; // configurable

    private float currentRotationY = 0f;

    // --- Preparar preview ---
    public void PrepararConstruccion(ConstruccionSO construccion)
    {
        construccionSeleccionada = construccion;
        canva.SetActive(true);

        // 🔥 destruir la preview anterior si existe
        if (previewInstance != null)
            Destroy(previewInstance);

        // instanciar nueva preview
        previewInstance = Instantiate(construccion.previewPrefab);

        // desactivar scripts
        foreach (var comp in previewInstance.GetComponentsInChildren<MonoBehaviour>())
            comp.enabled = false;

        // desactivar NavMeshObstacle si existe
        foreach (var nav in previewInstance.GetComponentsInChildren<UnityEngine.AI.NavMeshObstacle>())
            nav.enabled = false;

        // desactivar colliders para que no interfiera
        foreach (var col in previewInstance.GetComponentsInChildren<Collider>())
            col.enabled = false;

        previewRenderers = previewInstance.GetComponentsInChildren<Renderer>();

        currentRotationY = 0f;
    }

    void Update()
    {
        if (construccionSeleccionada == null || previewInstance == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistancia, sueloMask | sueloNoConstruibleMask))
        {
            previewInstance.transform.position = hit.point;

            // rotación con la ruedita
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
                currentRotationY += scroll * 100f;

            previewInstance.transform.rotation = Quaternion.Euler(0, currentRotationY, 0);

            // validación de espacio
            bool valido = !Physics.CheckBox(hit.point, Vector3.one * 0.5f, Quaternion.identity, obstaculosMask | sueloNoConstruibleMask);

            foreach (Renderer r in previewRenderers)
            {
                foreach (var mat in r.materials)
                    mat.color = valido ? colorValido : colorInvalido;
            }

            // confirmar con click izquierdo
            if (Input.GetMouseButtonDown(0) && valido)
                ColocarConstruccion(hit.point);

            // ❌ cancelar con click derecho o ESC
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                CancelarConstruccion();
        }
    }

    private void ColocarConstruccion(Vector3 posicion)
    {
        canva.SetActive(false);
        if (construccionSeleccionada.cantidadActual < construccionSeleccionada.cantidadMaxima)
        {
            Instantiate(construccionSeleccionada.prefab, posicion, Quaternion.Euler(0, currentRotationY, 0));
            construccionSeleccionada.cantidadActual++;
        }
        else
        {
            Debug.Log("Ya no quedan más de este ítem: " + construccionSeleccionada.nombre);
        }

        CancelarConstruccion();
    }

    public void CancelarConstruccion()
    {
        canva.SetActive(false);
        if (previewInstance != null)
            Destroy(previewInstance);

        construccionSeleccionada = null;
        previewInstance = null;
        previewRenderers = null;
    }
}


