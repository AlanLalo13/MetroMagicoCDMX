using UnityEngine;
using UnityEngine.UI;

public class PlayPauseButtonController : MonoBehaviour
{
    public Sprite playSprite;
    public Sprite pauseSprite;
    public Image buttonImage;
    public AudioSource audioSource; // Audio directamente

    private bool estaEnPausa = true;

    public void TogglePlayPause()
    {
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                buttonImage.sprite = playSprite;
                estaEnPausa = true;
            }
            else
            {
                audioSource.Play();
                buttonImage.sprite = pauseSprite;
                estaEnPausa = false;
            }
        }
    }
}
