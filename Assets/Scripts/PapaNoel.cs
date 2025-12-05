using UnityEngine;
using TMPro;
using System.Collections;

public class PapaNoel : MonoBehaviour
{
    [Header("Config")]
    public GameObject globoActual;
    public Transform globoAnchor;
    public ConstruccionSO regalo;
    [TextArea] public string[] groserias;

    [Header("UI Prompt")]
    public GameObject promptUI; // Texto "Press E" o ícono

    private EstadoPapaNoel estado = EstadoPapaNoel.Saludo;
    private TMP_Text globoTMP;

    private void Update()
{
    bool cerca = JugadorCerca();

    // Mostrar/ocultar prompt
    if (promptUI != null) 
        promptUI.SetActive(cerca);

    // Interactuar si está cerca y toca E
    if (cerca && Input.GetKeyDown(KeyCode.E))
    {
        Interactuar();
    }

    if (globoActual != null && !cerca)
    {
        globoActual.SetActive(false);
    }

    // Orientar prompt hacia la cámara
    if (promptUI != null && Camera.main != null)
    {
        promptUI.transform.LookAt(Camera.main.transform);
        promptUI.transform.forward = -promptUI.transform.forward;
    }

    // Orientar globo hacia la cámara
    if (globoActual != null && Camera.main != null)
    {
        globoActual.transform.LookAt(Camera.main.transform);
        globoActual.transform.forward = -globoActual.transform.forward;
    }
}


    bool JugadorCerca()
    {
        return Vector3.Distance(transform.position, Camera.main.transform.position) < 3f;

    }

    void Interactuar()
{
    if (!globoActual.activeSelf) // si está desactivado, lo activamos
    {
        globoActual.SetActive(true);
        globoTMP = globoActual.GetComponentInChildren<TMP_Text>();
    }

    switch (estado)
    {
        case EstadoPapaNoel.Saludo:
            ActualizarGlobo("Speak quickly, I don’t have all day.");
            estado = EstadoPapaNoel.Regalo;
            break;

        case EstadoPapaNoel.Regalo:
            ActualizarGlobo($"Take this {regalo.nombre} and leave me alone. You can find it in your construction wheel.");
            DarRegalo();
            estado = EstadoPapaNoel.Groserias;
            break;

        case EstadoPapaNoel.Groserias:
            string groseria = groserias[Random.Range(0, groserias.Length)];
            ActualizarGlobo(groseria);
            break;
    }
}


    void ActualizarGlobo(string texto)
    {
        globoTMP.text = texto;
        StartCoroutine(PopEffect());
    }

    IEnumerator PopEffect()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.2f;
            globoActual.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        globoActual.transform.localScale = Vector3.one;
    }

    void DarRegalo()
{
    Debug.Log($"Jugador recibió: {regalo.nombre}");
    regalo.desbloqueado = true; // 🔓 desbloquear

    // Buscar el RadialUI y refrescar
    FindObjectOfType<RadialUI>().RefrescarRadial();
}

}

public enum EstadoPapaNoel
{
    Saludo,
    Regalo,
    Groserias
}


