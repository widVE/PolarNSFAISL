using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sfxController : MonoBehaviour
{
    public AudioSource glass;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    public void playGlassClick()
    {
        Debug.Log("Click!");
        glass.Play();
    }
}
