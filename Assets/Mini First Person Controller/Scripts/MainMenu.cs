using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Escena de juego")]
    [SerializeField] private Object gameScene; // arrastrás la escena aquí
    [SerializeField] private GameObject instructionsPanel;

    void Start()
    {
        // 👇 asegurar que el mouse esté activo al entrar al menú
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Jugar()
    {
       
        AsyncOperation operacion = SceneManager.LoadSceneAsync(1);

        // 👇 opcional: ocultar cursor al entrar al juego
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Salir del juego"); // solo se ve en editor
    }

    public void ToggleInstructionsPanel()
    {
        if (instructionsPanel != null)
        {
            bool isActive = instructionsPanel.activeSelf;
            instructionsPanel.SetActive(!isActive);
        }
    }
}



