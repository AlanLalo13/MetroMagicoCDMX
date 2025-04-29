using UnityEngine;

public class AudioOnMarkerDetect : MonoBehaviour
{
    public AudioSource marcadorAudio;

    public void PlayAudio()
    {
        if (marcadorAudio != null)
        {
            marcadorAudio.Stop();   // Detenemos en caso de que estuviera sonando antes
            marcadorAudio.Play();
        }
    }

    public void StopAudio()
    {
        if (marcadorAudio != null)
        {
            marcadorAudio.Stop();
        }
    }

    public void RestartAudio()
    {
        if (marcadorAudio != null)
        {
            marcadorAudio.Stop();
            marcadorAudio.Play();
        }
    }

}

