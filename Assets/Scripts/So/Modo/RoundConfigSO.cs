using UnityEngine;

[CreateAssetMenu(menuName = "Rondas/Configuracion de ronda")]
public class RoundConfigSO : ScriptableObject
{
    public ShopSO tienda;
    public GameModeSO modo;

    [Header("Tiempos")]
    public float preRoundTiempo = 60f;   // preparación (1-2 min)
    public float roundDuracion = 120f;   // duración de la ronda en segundos

    [Header("Clientes")]
    public int cantidadClientes = 10;    // por oleada
}


