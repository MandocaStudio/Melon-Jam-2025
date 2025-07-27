using UnityEngine;

public class buttonSwitcher : MonoBehaviour
{

    public AudioSource sfxButtonplayer;

    public AudioClip switchAudio;


    public void switchAudioOn()
    {

        sfxButtonplayer.PlayOneShot(switchAudio);
    }
}
