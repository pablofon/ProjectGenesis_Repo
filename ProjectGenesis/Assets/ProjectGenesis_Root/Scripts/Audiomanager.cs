using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audiomanager : MonoBehaviour
{
    [Header( "Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip Theme;
    public AudioClip PlayerLine;
    public AudioClip PlayerLine2;
    public AudioClip Shylady;
    public AudioClip Spectator;
    public AudioClip PlayerLine3;
}
