using System.Collections;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [Header("Config")]
    public RoundConfigSO[] rondasPosibles;
    public ClienteSpawner spawner;
    public UIManager uiManager; // 👈 referencia al UIManager

    // runtime
    private RoundConfigSO rondaActual;
    private bool roundActiva = false;
    private int clientesSatisfechos = 0;
    private int clientesFrustrados = 0;
    private int clientesTotales = 0;

    void Start()
    {
        SeleccionarRondaRandom();
        StartCoroutine(RoundFlow());
    }

    void SeleccionarRondaRandom()
    {
        rondaActual = rondasPosibles[Random.Range(0, rondasPosibles.Length)];
        rondaActual.modo.misionPrincipal.ResetProgreso();
        foreach (var m in rondaActual.modo.misionesSecundarias)
            m.ResetProgreso();
    }

    IEnumerator RoundFlow()
    {
        // Intro: Black Friday
        uiManager.ShowBigTitle($"Black Friday in {rondaActual.tienda.nombreTienda}", 5f);
        yield return new WaitForSeconds(5f);

        // Intro: Modo
        uiManager.ShowBigTitle($"Mode: {rondaActual.modo.nombreModo}", 3f);
        yield return new WaitForSeconds(3f);

        // Banner fijo con tienda + modo + misiones
        uiManager.SetPersistentHeader(
            rondaActual.tienda.nombreTienda,
            rondaActual.modo.nombreModo,
            rondaActual.tienda.colorUI
        );
        uiManager.ShowMissionList(rondaActual.modo.misionPrincipal, rondaActual.modo.misionesSecundarias);

        // Pre-ronda
        uiManager.StartCountdown(rondaActual.preRoundTiempo);
        CloseDoors(rondaActual.tienda);
        yield return new WaitForSeconds(rondaActual.preRoundTiempo);

        // Inicio de ronda
        OpenDoors(rondaActual.tienda);
        roundActiva = true;

        // Configurar spawner
        spawner.cantidadPorOleada = rondaActual.cantidadClientes;
        spawner.SpawnOleada();
        clientesTotales += rondaActual.cantidadClientes;

        // Esperar fin de ronda
        yield return StartCoroutine(EsperarFinRonda());

        roundActiva = false;

        // Evaluar resultados
        var principalEval = rondaActual.modo.misionPrincipal.Evaluar();
        var secundariasEval = EvaluarSecundarias(rondaActual.modo.misionesSecundarias);

        int estrellas = CalcularEstrellas(principalEval, secundariasEval);

        // Mostrar resumen
        //ui.ShowRoundSummary(principalEval, secundariasEval, estrellas, clientesSatisfechos, clientesTotales);

        // Mostrar menú Win/GameOver según estrellas
        uiManager.ShowWin(estrellas);

        // Esperar botón continuar (Next Round o Retry)
        yield return uiManager.WaitForSummaryContinue();

        // Si fue Win y eligió Next Round
        SeleccionarRondaRandom();
        StartCoroutine(RoundFlow());
    }

    IEnumerator EsperarFinRonda()
    {
        float maxDuracion = rondaActual.roundDuracion;
        float t = 0f;

        while (t < maxDuracion && roundActiva)
        {
            t += Time.deltaTime;
            uiManager.UpdateTimerUI(maxDuracion - t);
            yield return null;
        }
    }

    MissionEval[] EvaluarSecundarias(MissionSO[] misiones)
    {
        var evals = new MissionEval[misiones.Length];
        for (int i = 0; i < misiones.Length; i++)
            evals[i] = misiones[i].Evaluar();
        return evals;
    }

    int CalcularEstrellas(MissionEval principal, MissionEval[] secundarias)
    {
        int baseEstrellas = principal == MissionEval.Logrado ? 3 :
                            principal == MissionEval.Casi ? 2 : 0;

        float extra = 0f;
        foreach (var e in secundarias)
            extra += e == MissionEval.Logrado ? 1f : e == MissionEval.Casi ? 0.5f : 0f;

        return Mathf.Clamp(Mathf.RoundToInt(baseEstrellas + extra), 0, 3);
    }

    void CloseDoors(ShopSO tienda) { /* cerrar puerta */ }
    void OpenDoors(ShopSO tienda)  { /* abrir puerta */ }

    // --- Eventos ---
    public void RegistrarClienteSatisfecho()
{
    clientesSatisfechos++;
    ActualizarProgreso();
}

public void RegistrarClienteFrustrado()
{
    clientesFrustrados++;
    ActualizarProgreso();
}

void ActualizarProgreso()
{
    var m = rondaActual.modo.misionPrincipal;

    if (m.countMode == MissionSO.MissionCountMode.Frustrados)
    {
        float porcentajeFrustrados = (float)clientesFrustrados / Mathf.Max(clientesTotales, 1) * 100f;
        m.progresoEntero = Mathf.RoundToInt(porcentajeFrustrados);

        // 👉 actualizar barra con frustrados
        uiManager.UpdateClientesMoodBar(clientesFrustrados, clientesSatisfechos, clientesTotales);
    }
    else
    {
        float porcentajeSatisfechos = (float)clientesSatisfechos / Mathf.Max(clientesTotales, 1) * 100f;
        m.progresoEntero = Mathf.RoundToInt(porcentajeSatisfechos);

        // 👉 actualizar barra con satisfechos
        uiManager.UpdateClientesMoodBar(clientesFrustrados, clientesSatisfechos, clientesTotales);
    }

    Debug.Log($"Frustrados: {clientesFrustrados}/{clientesTotales} = {m.progresoEntero}%");

    // 👇 cortar ronda si se cumple objetivo
    if (m.tipo == MissionType.Principal && m.progresoEntero >= m.objetivoEntero && roundActiva)
    {
        roundActiva = false;
    }
}




    // --- Métodos para UIManager ---
    public void NextRound()
    {
        StopAllCoroutines();
        roundActiva = false;
        SeleccionarRondaRandom();
        StartCoroutine(RoundFlow());
    }

    public void RetrySameRound()
    {
        StopAllCoroutines();
        roundActiva = false;
        rondaActual.modo.misionPrincipal.ResetProgreso();
        foreach (var m in rondaActual.modo.misionesSecundarias)
            m.ResetProgreso();
        StartCoroutine(RoundFlow());
    }
}



