using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;
using TouchScript.Pointers;
using TouchScript;

struct pointer
{
    public Pointer p;
    public Vector2 startPos;
    public GameObject audioObject;

    //Constructor (not necessary, but helpful)
    public pointer(Pointer p, GameObject audioObject, Vector2 startPos)
    {
        this.p = p;
        this.audioObject = audioObject;
        this.startPos = startPos;
    }
};

//script that makes popping noises the faster you drag on the screen

public class dragPop : MonoBehaviour
{

    public AudioClip popClip;
    private List<pointer> pointerList;
    private int numPointers;
    private float deltaPos;

    // Use this for initialization
    void Start()
    {
        pointerList = new List<pointer>();
        numPointers = 0;
        if (popClip == null)
        {
            Debug.LogError("No pop sound inserted");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (pointer point in pointerList) {
            deltaPos = Vector2.Distance(point.startPos, point.p.Position);
            if (!point.audioObject.GetComponent<AudioSource>().isPlaying && Vector2.Distance(point.p.Position, point.p.PreviousPosition) > 20)
            {
                point.audioObject.GetComponent<AudioSource>().pitch = (.8f + deltaPos * .0005f);
                point.audioObject.GetComponent<AudioSource>().Play();
            }
            //point.audioObject.GetComponent<AudioSource>().pitch += 
        }
    }

    private void OnEnable()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.PointersPressed += pointerAddHandler;
            TouchManager.Instance.PointersReleased += pointerRemoveHandler;
        }
            
    }

    private void OnDisable()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.PointersPressed -= pointerAddHandler;
            TouchManager.Instance.PointersReleased -= pointerRemoveHandler;
        }
    }

    private void pointerAddHandler(object sender, PointerEventArgs e)
    {
        Pointer p;
        p = e.Pointers[0];
        GameObject audioObject = new GameObject();
        AudioSource source = audioObject.AddComponent<AudioSource>();
        //set audio source params
        source.clip = popClip;
        source.playOnAwake = false;
        source.pitch = .8f;
        source.volume = 1;
        source.loop = false;
        source.Play();
        //add pointer to list
        pointer newPointer = new pointer(p, audioObject, p.Position);
        //Debug.Log("pop");
        pointerList.Add(newPointer);
        numPointers++;
    }

    private void pointerRemoveHandler(object sender, PointerEventArgs e)
    {
        Pointer p;
        p = e.Pointers[0];
        for (int i = 0; i < numPointers; i++)
        {
            if (p.Id == pointerList[i].p.Id)
            {
                pointerList.RemoveAt(i);
                numPointers--;
                return;
            }
        }
        Debug.LogError("A pointer was lost somewhere!");
    }
}