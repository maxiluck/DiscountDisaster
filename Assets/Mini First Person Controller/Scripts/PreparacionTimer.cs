using UnityEngine;
using UnityEngine.UI;

public class PreparacionTimer : MonoBehaviour
{
    public float tiempoPreparacion = 120f;
    public Text timerText;
    public ClienteSpawner spawner;

    private float tiempoRestante;

    void Start()
    {
        tiempoRestante = tiempoPreparacion;
    }

    void Update()
    {
        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            //timerText.text = Mathf.Ceil(tiempoRestante).ToString();
        }
        else
        {
            spawner.SpawnOleada();
            enabled = false; // desactiva el timer
        }
    }
}

