using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("HUD TMP")]
    [SerializeField] private TextMeshProUGUI bigTitleTMP;
    [SerializeField] private TextMeshProUGUI headerTMP;
    [SerializeField] private GameObject misionPanel;
    [SerializeField] private TextMeshProUGUI missionListTMP;
    [SerializeField] private TextMeshProUGUI countdownTMP;
    [SerializeField] private TextMeshProUGUI summaryTMP;
    [SerializeField] private TMP_Text timerText;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    [Header("Stars UI")]
    [SerializeField] private Image[] stars;
    [SerializeField] private Color inactiveColor = new Color(0.6f, 0.6f, 0.6f);
    [SerializeField] private Color activeColor   = new Color(1f, 0.84f, 0f);

    [Header("Clientes Barra")]
    [SerializeField] private Slider barraClientes;
    [SerializeField] private Image caraFeliz;
    [SerializeField] private Image caraEnojada;
    [SerializeField] private TextMeshProUGUI barraLabelTMP; // opcional
    [SerializeField] private Color colorFeliz = Color.green;
    [SerializeField] private Color colorEnojado = Color.red;


    [Header("Refs")]
    [SerializeField] private RoundManager roundManager;

    // --- HUD ---
    public void UpdateTimerUI(float tiempoRestante)
    {
        int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
        timerText.text = $"{minutos:00}:{segundos:00}";
    }
    public void UpdateClientesMoodBar(int frustrados, int satisfechos, int totales)
{
    if (barraClientes == null) return;

    // porcentaje de frustrados (0 = todos felices, 1 = todos frustrados)
    float porcentajeFrustrados = (float)frustrados / Mathf.Max(totales, 1);
    barraClientes.value = porcentajeFrustrados;

    // actualizar texto opcional
    if (barraLabelTMP != null)
    {
        barraLabelTMP.text = $"😀 {satisfechos}/{totales} | 😡 {frustrados}/{totales}";
    }

    // elegir color según tu regla
    // por ejemplo: si hay más frustrados que satisfechos → rojo, si no → verde
    Color color = frustrados > satisfechos ? colorEnojado : colorFeliz;
    barraClientes.fillRect.GetComponent<Image>().color = color;
}


    public void ShowBigTitle(string texto, float duracion) => StartCoroutine(ShowBigTitleRoutine(texto, duracion));
    private IEnumerator ShowBigTitleRoutine(string texto, float duracion)
    {
        bigTitleTMP.gameObject.SetActive(true);
        bigTitleTMP.text = texto;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            bigTitleTMP.alpha = Mathf.Lerp(0f, 1f, t);
            bigTitleTMP.transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);
            yield return null;
        }

        yield return new WaitForSeconds(duracion);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            bigTitleTMP.alpha = Mathf.Lerp(1f, 0f, t);
            bigTitleTMP.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, t);
            yield return null;
        }

        bigTitleTMP.gameObject.SetActive(false);
    }

    public void SetPersistentHeader(string tienda, string modo, Color color)
    {
        headerTMP.gameObject.SetActive(true);
        headerTMP.text = $"{tienda} - {modo}";
        headerTMP.color = color;
    }

    public void ShowMissionList(MissionSO principal, MissionSO[] secundarias)
    {
        misionPanel.SetActive(true);
        missionListTMP.gameObject.SetActive(true);

        string texto = $"Main: {principal.descripcion}\n";
        foreach (var m in secundarias) texto += $"- {m.titulo}\n";
        missionListTMP.text = texto;

        StartCoroutine(ColorPulse(missionListTMP, Color.yellow));
    }

    private IEnumerator ColorPulse(TextMeshProUGUI tmp, Color targetColor)
    {
        Color baseColor = tmp.color;
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime;
            tmp.color = Color.Lerp(baseColor, targetColor, (Mathf.Sin(t * 2f) + 1f) / 2f);
            yield return null;
        }
    }

    public void StartCountdown(float segundos) => StartCoroutine(CountdownRoutine(segundos));
    private IEnumerator CountdownRoutine(float segundos)
    {
        countdownTMP.gameObject.SetActive(true);
        float t = segundos;
        while (t > 0)
        {
            countdownTMP.text = Mathf.CeilToInt(t).ToString();
            yield return StartCoroutine(PopEffect(countdownTMP));
            yield return new WaitForSeconds(1f);
            t -= 1f;
        }
        countdownTMP.text = "¡GO!";
        yield return StartCoroutine(PopEffect(countdownTMP));
    }

    private IEnumerator PopEffect(TextMeshProUGUI tmp)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
            tmp.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        tmp.transform.localScale = Vector3.one;
    }

    public void ShowRoundSummary(MissionEval principal, MissionEval[] secundarias, int estrellas, int sat, int tot)
    {
        summaryTMP.gameObject.SetActive(true);
        summaryTMP.text =
            $"Resultado:\nPrincipal: {principal}\nSecundarias: {string.Join(", ", secundarias)}\n" +
            $"Estrellas: {estrellas}\nClientes satisfechos: {sat}/{tot}";

        UpdateStars(estrellas);
    }

    public IEnumerator WaitForSummaryContinue()
    {
        bool continuar = false;
        while (!continuar) yield return null;
    }

    // --- Menús ---
    public void ShowPauseMenu() => ShowPanel(pausePanel);
    public void ShowGameOver(int estrellas = 0) { ShowPanel(gameOverPanel); UpdateStars(estrellas); }
    public void ShowWin(int estrellas = 0) { ShowPanel(winPanel); UpdateStars(estrellas); }

    private void ShowPanel(GameObject panel)
    {
        HideAll();
        panel.SetActive(true);
        misionPanel.SetActive(false);
        timerText.gameObject.SetActive(false); // ocultar timer
        Time.timeScale = 0f;
        EnableCursor();
    }

    public void HideAll()
    {
        pausePanel.SetActive(false);
        misionPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        timerText.gameObject.SetActive(true); // mostrar timer
        Time.timeScale = 1f;
        DisableCursor();
    }

    // --- Botones ---
    public void ResumeGame() => HideAll();
    public void RestartGame() { HideAll(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void GoToMainMenu(string menuSceneName) { HideAll(); AsyncOperation operacion = SceneManager.LoadSceneAsync(0); }
    public void QuitGame() { Application.Quit(); Debug.Log("Salir del juego"); }
    public void NextRoundButton() { HideAll(); if (roundManager != null) roundManager.NextRound(); }
    public void RetryButton() { HideAll(); if (roundManager != null) roundManager.RetrySameRound(); else SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

    // --- Estrellas ---
    public void UpdateStars(int count)
    {
        if (stars == null || stars.Length == 0) return;
        count = Mathf.Clamp(count, 0, stars.Length);
        for (int i = 0; i < stars.Length; i++)
            stars[i].color = i < count ? activeColor : inactiveColor;
    }

    // --- Cursor control ---
    private void EnableCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void DisableCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}





