using UnityEngine;

[CreateAssetMenu(fileName = "NuevaConstruccion", menuName = "Supermercado/Construccion")]
public class ConstruccionSO : ScriptableObject
{
    [Header("Datos básicos")]
    public string nombre;                 // Nombre del ítem
    public Sprite icono;                  // Ícono para mostrar en el menú radial

    [Header("Prefabs")]
    public GameObject prefab;             // Prefab real que se coloca en el mundo
    public GameObject previewPrefab;      // Prefab fantasma (transparente) para previsualización

    [Header("Restricciones")]
    public int cantidadMaxima = 5;        // Límite de colocaciones
    [HideInInspector] public int cantidadActual = 0; // Contador interno
    public bool desbloqueado = true;      // Si está disponible o bloqueado en el menú radial

    public void ResetearCantidad()
    {
        // Esto restablece el valor a su estado inicial para la sesión
        cantidadActual = 0; 
        // Si no quieres depender de 'cantidadInicial', simplemente usa:
        // cantidadActual = 0; 
    }
}

