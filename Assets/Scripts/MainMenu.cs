using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Escena de juego")]
    [SerializeField] private Object gameScene; // arrastrás la escena aquí
    [SerializeField] private GameObject instructionsPanel;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;


    void Start()
    {
        // 👇 asegurar que el mouse esté activo al entrar al menú
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Jugar()
    {
       audioSource.Play();
        AsyncOperation operacion = SceneManager.LoadSceneAsync(1);

        // 👇 opcional: ocultar cursor al entrar al juego
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Salir()
    {
        audioSource.Play();
        Application.Quit();
        Debug.Log("Salir del juego"); // solo se ve en editor
    }

    public void ToggleInstructionsPanel()
    {
        if (instructionsPanel != null)
        {
            audioSource.Play();
            bool isActive = instructionsPanel.activeSelf;
            instructionsPanel.SetActive(!isActive);
        }
    }
}



