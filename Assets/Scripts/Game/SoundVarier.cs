using UnityEngine;
using System.Collections;

/**
 * Varies the sound, an extension of AudioSource implementation.
 * This can be run locally.
 */
 [RequireComponent(typeof(AudioSource))]
public class SoundVarier : MonoBehaviour {

    [Header("Pitch Shift")]
    public float pitch = 1.0f;
    public Vector2 pitchShift = new Vector2(0f, 0f);

    [Header("Volume Shift")]
    public float volume = 1.0f;
    public Vector2 volumeShift = new Vector2(0f, 0f);

    [Header("Other")]
    public bool playOnAwake;

    AudioSource source;

    void Start () {
        //Override sound settings
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
	}

    void OnEnable() {
        if (playOnAwake && source != null) {
            source.pitch = pitch + Random.Range(pitchShift.x, pitchShift.y);
            source.volume = volume + Random.Range(volumeShift.x, volumeShift.y);
            source.enabled = true;
            source.Play();
        }
    }
	
}
