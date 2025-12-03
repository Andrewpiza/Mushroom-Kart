using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]private GameObject sound;

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();

    [SerializeField]private List<AudioSource> sounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        AddSound("Coin",Resources.Load<AudioClip>("Audio/Coin"));
        AddSound("Jump",Resources.Load<AudioClip>("Audio/Jump"));
        AddSound("Big Jump",Resources.Load<AudioClip>("Audio/Big Jump"));
        AddSound("Boost",Resources.Load<AudioClip>("Audio/Boost"));
        AddSound("Getting Item",Resources.Load<AudioClip>("Audio/Getting Item"));
        AddSound("Got Item",Resources.Load<AudioClip>("Audio/Got Item"));
        AddSound("Hit",Resources.Load<AudioClip>("Audio/Hit"));
        AddSound("Lap",Resources.Load<AudioClip>("Audio/Lap"));
        AddSound("Trick",Resources.Load<AudioClip>("Audio/Trick"));
        AddSound("Mario Circuit",Resources.Load<AudioClip>("Audio/Mario Circuit"));
        AddSound("Mario Circuit Final",Resources.Load<AudioClip>("Audio/Mario Circuit Final"));
        sounds = new List<AudioSource>();
        PlaySound("Mario Circuit",0.3f);
    }

    private void AddSound(string soundName, AudioClip audio)
    {
        sfxDictionary.Add(soundName, audio);
    }

    public void PlaySound(string soundName, float volume = 1, float pitch = 1)
    {
        GameObject soundObject = Instantiate(sound,transform.position,transform.rotation);
        AudioSource source = soundObject.GetComponent<AudioSource>();
        source.volume = volume;
        source.pitch = pitch;
        source.PlayOneShot(sfxDictionary[soundName]);


        sounds.Add(source);
    }

    // Update is called once per frame
    void Update()
    {
        List<AudioSource> delete = new List<AudioSource>();
        foreach (AudioSource audio in sounds)
        {
            if (!audio.isPlaying)
            {
                delete.Add(audio);
                
            }
        }

        foreach (AudioSource audio in delete)
        {
            sounds.Remove(audio);
            Destroy(audio.gameObject);
        }
    }
}
