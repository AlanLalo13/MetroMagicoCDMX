using UnityEngine;
using System.Collections;

public class CleanMarker : MonoBehaviour
{
    public GameObject[] objetosParaOcultar;
    public PlayPauseButtonController playPauseController;
    public AudioSource marcadorAudio; // 🎯 Nuevo: el audio a controlar
    public float tiempoParaReactivar = 3.0f; // Segundos para volver a activar

    public void CleanThisMarker()
    {
        // 1. Ocultar objetos
        foreach (GameObject objeto in objetosParaOcultar)
        {
            if (objeto != null)
            {
                objeto.SetActive(false);
            }
        }

        // 2. Detener y reiniciar el audio
        if (marcadorAudio != null)
        {
            marcadorAudio.Stop();   // Detenemos el audio
            marcadorAudio.time = 0;  // Reiniciamos su posición a 0
        }

        // 3. Resetear botón Play/Pause al ícono de Play
        if (playPauseController != null)
        {
            playPauseController.buttonImage.sprite = playPauseController.playSprite;
        }

        // 4. Empezar temporizador para reactivar
        StartCoroutine(ReactivarObjetos());
    }

    private IEnumerator ReactivarObjetos()
    {
        yield return new WaitForSeconds(tiempoParaReactivar);

        foreach (GameObject objeto in objetosParaOcultar)
        {
            if (objeto != null)
            {
                objeto.SetActive(true);
            }
        }
    }
}
