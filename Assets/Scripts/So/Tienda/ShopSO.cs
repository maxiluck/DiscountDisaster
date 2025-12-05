using UnityEngine;

[CreateAssetMenu(menuName = "Rondas/Tienda")]
public class ShopSO : ScriptableObject
{
    public string nombreTienda;   // Ej: "ElectroManía"
    public Sprite iconoTienda;    // Logo o ícono para la UI
    public Color colorUI;         // Color temático para el HUD
}



