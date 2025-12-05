using UnityEngine;

[CreateAssetMenu(menuName = "Rondas/Modo")]
public class GameModeSO : ScriptableObject
{
    public string nombreModo;          // "Estorbo", "Molestar", etc.
    [TextArea] public string descripcionModo;
    public Color colorUI;              // para acentos en UI
    public MissionSO misionPrincipal;  // una principal
    public MissionSO[] misionesSecundarias; // mini misiones
}

