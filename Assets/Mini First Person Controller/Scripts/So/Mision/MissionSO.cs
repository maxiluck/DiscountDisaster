using UnityEngine;

public enum MissionType { Principal, Secundaria }
public enum MissionEval { NoLogrado, Casi, Logrado }

[CreateAssetMenu(menuName = "Rondas/Mision")]
public class MissionSO : ScriptableObject
{
    public string titulo;
    [TextArea] public string descripcion;
    public MissionType tipo;
    public int objetivoEntero;       // ej: "evitar 90% satisfechos" => 90
    public float objetivoFloat;      // ej: tiempo, porcentajes exactos
    public string itemRequeridoTag;  // opcional, para "usar tal ítem"

    // runtime (no persistir en asset)
    [HideInInspector] public int progresoEntero;
    [HideInInspector] public float progresoFloat;

    public enum MissionCountMode { Satisfechos, Frustrados }

    public MissionCountMode countMode;


    public MissionEval Evaluar()
    {
        // Ejemplos: ajustá la lógica por tipo
        if (tipo == MissionType.Principal)
        {
            return progresoEntero >= objetivoEntero ? MissionEval.Logrado
                 : progresoEntero >= Mathf.RoundToInt(objetivoEntero * 0.7f) ? MissionEval.Casi
                 : MissionEval.NoLogrado;
        }
        else
        {
            return progresoEntero >= objetivoEntero || progresoFloat >= objetivoFloat
                ? MissionEval.Logrado
                : progresoEntero > 0 || progresoFloat > 0 ? MissionEval.Casi
                : MissionEval.NoLogrado;
        }
    }

    public void ResetProgreso()
    {
        progresoEntero = 0;
        progresoFloat = 0f;
    }
}

