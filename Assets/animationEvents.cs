using System.Threading;
using UnityEngine;


public class animationEvents : MonoBehaviour
{

    [Header("Auudio source y audios en general")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fallingAudio, risita;

    [Header("risita variables")]

    [SerializeField] public bool seRio;



    public void fallingAudioEvent()
    {
        audioSource.PlayOneShot(fallingAudio);
    }

    public void StartRisitaTimer()
    {

        audioSource.PlayOneShot(risita); // Reproducir la risa

    }
}


