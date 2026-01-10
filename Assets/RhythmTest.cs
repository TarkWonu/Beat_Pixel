using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RhythmTest : MonoBehaviour
{
    [SerializeField] MusicScript music;

    AudioSource audioSource;



    double currentTime = 0d;
    double beatPerSec
    {
        get{return 60d/music.bpm;}
    }
    int beatIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = music.Music;
        audioSource.Play();
    }



    void FixedUpdate()
    {
        if(beatIndex>=music.beats.Length) return;
        currentTime = audioSource.time;

        if (currentTime >= beatPerSec * music.beats[beatIndex])
        {
            Debug.Log("노트");
            beatIndex++;
        }
    }
}
