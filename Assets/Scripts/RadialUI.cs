using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class RadialUI : MonoBehaviour
{
    [SerializeField] private GameObject radialSectionPrefab;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private float separacion = 2f;
    [SerializeField] private float animSpeed = 10f;
    [SerializeField] private float bounceStrength = 0.2f;
    [SerializeField] private float bounceSpeed = 6f;

    private List<GameObject> radialSections = new List<GameObject>();
    private List<TMP_Text> cantidadTexts = new List<TMP_Text>();
    private List<Image> iconos = new List<Image>();

    private int currentSelectedRadialPart = -1;
    public UnityEvent<int> onRadialPartSelected;
    private bool menuActivo = false;

    [Header("Construcciones disponibles")]
    public List<ConstruccionSO> construcciones;

    [SerializeField] private ConstructionPlacer placer;

    void Start()
    {
        foreach (var construccion in construcciones)
            construccion.ResetearCantidad();

        canvasTransform.gameObject.SetActive(false);
        GenerarRadialUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            MostrarMenu();

        if (Input.GetKey(KeyCode.Q) && menuActivo)
            DetectarSeleccionMouse();

        if (Input.GetKeyUp(KeyCode.Q) && menuActivo)
            OcultarMenuYConfirmar();

        ActualizarAnimaciones();
    }

    private void MostrarMenu()
    {
        canvasTransform.gameObject.SetActive(true);
        menuActivo = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ActualizarCantidades();
    }

    private void OcultarMenuYConfirmar()
    {
        if (currentSelectedRadialPart >= 0 && currentSelectedRadialPart < construcciones.Count)
        {
            ConstruccionSO seleccionada = construcciones[currentSelectedRadialPart];
            if (seleccionada.desbloqueado && seleccionada.cantidadActual < seleccionada.cantidadMaxima)
                placer.PrepararConstruccion(seleccionada);
        }

        canvasTransform.gameObject.SetActive(false);
        menuActivo = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void DetectarSeleccionMouse()
    {
        Vector2 centroPantalla = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mousePos = Input.mousePosition;
        Vector2 direccion = (mousePos - centroPantalla).normalized;

        float angle = Mathf.Atan2(direccion.x, direccion.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        currentSelectedRadialPart = Mathf.FloorToInt(angle * radialSections.Count / 360f);
    }

    private void ActualizarAnimaciones()
    {
        for (int i = 0; i < radialSections.Count; i++)
        {
            Image img = radialSections[i].GetComponent<Image>();

            Vector3 targetScale = (i == currentSelectedRadialPart) ? Vector3.one * 1.2f : Vector3.one;
            Color targetColor = (i == currentSelectedRadialPart) ? Color.yellow : Color.white;

            radialSections[i].transform.localScale = Vector3.Lerp(
                radialSections[i].transform.localScale,
                targetScale,
                Time.deltaTime * animSpeed
            );

            img.color = Color.Lerp(img.color, targetColor, Time.deltaTime * animSpeed);

            if (i == currentSelectedRadialPart)
            {
                float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceStrength;
                radialSections[i].transform.localScale += Vector3.one * bounce;
            }
        }
    }

    private void GenerarRadialUI()
{
    radialSections.Clear();
    iconos.Clear();
    cantidadTexts.Clear();

    // 🔎 Solo las desbloqueadas
    List<ConstruccionSO> disponibles = construcciones.FindAll(c => c.desbloqueado);

    int cantidadSecciones = disponibles.Count;

    for (int i = 0; i < cantidadSecciones; i++)
    {
        GameObject section = Instantiate(radialSectionPrefab, canvasTransform);
        float angulo = -i * 360f / cantidadSecciones;
        section.transform.localEulerAngles = new Vector3(0, 0, angulo);

        Image sectorImg = section.GetComponent<Image>();
        sectorImg.fillAmount = 1f / cantidadSecciones - (separacion / 360f);

        Transform iconoTransform = section.transform.Find("Icono");
        Image icono = iconoTransform.GetComponent<Image>();
        TMP_Text cant = iconoTransform.Find("cant").GetComponent<TMP_Text>();

        // 🧭 Rotación inversa para que el contenido mire siempre hacia arriba
        iconoTransform.localEulerAngles = new Vector3(0, 0, -angulo);

        iconos.Add(icono);
        cantidadTexts.Add(cant);
        radialSections.Add(section);
    }
    ActualizarCantidades();
    ActualizarIconos();
}

public void RefrescarRadial()
{
    // Primero limpiar hijos del canvas
    foreach (Transform child in canvasTransform)
        Destroy(child.gameObject);

    // Volver a generar
    GenerarRadialUI();
}



    private void ActualizarCantidades()
    {
        for (int i = 0; i < cantidadTexts.Count; i++)
            cantidadTexts[i].text = $"{construcciones[i].cantidadActual}/{construcciones[i].cantidadMaxima}";
    }

    private void ActualizarIconos()
    {
        for (int i = 0; i < iconos.Count; i++)
        {
            iconos[i].sprite = construcciones[i].icono;
            iconos[i].color = construcciones[i].desbloqueado ? Color.white : Color.gray;
        }
    }
}






