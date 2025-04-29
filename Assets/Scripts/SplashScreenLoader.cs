using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenLoader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float tiempoVisible = 2.5f;
    public float duracionFade = 1.5f;
    public string escenaPrincipal = "InterfazInicio";

    private bool estaFadeando = false;
    private float tiempoFade = 0f;

    void Start()
    {
        canvasGroup.alpha = 1f; // Totalmente visible
        Invoke("EmpezarFadeOut", tiempoVisible); // Esperar antes de empezar a desvanecer
    }

    void EmpezarFadeOut()
    {
        estaFadeando = true;
        tiempoFade = 0f; // Reiniciar contador
    }

    void Update()
    {
        if (estaFadeando)
        {
            tiempoFade += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, tiempoFade / duracionFade);

            if (tiempoFade >= duracionFade)
            {
                canvasGroup.alpha = 0f; // Seguridad
                SceneManager.LoadScene(escenaPrincipal); // Cargar escena hasta que fade terminó
            }
        }
    }
}
