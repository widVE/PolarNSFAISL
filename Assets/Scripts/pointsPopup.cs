using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointsPopup : MonoBehaviour {

    public Color color;
    public float scroll;
    public float duration;
    public float alpha;
    public UnityEngine.UI.Text text;

	// Use this for initialization
	void Start () {
        float value = float.Parse(text.text);
        color = new Color(2f*(1f - (value/100f)), 2f * (value / 100f), 0, 1.0f);
        scroll = 50f;
        duration = 1.5f;
        text.material.color = color;
        alpha = 1;
	}

	
	// Update is called once per frame
	void Update () {
        if (alpha > 0)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + scroll * Time.deltaTime, gameObject.transform.position.z);
            alpha -= Time.deltaTime / duration;
            color.a = alpha;
            text.material.color = color;
        }
        else
        {
            Destroy(gameObject); // text vanished - destroy itself
        }
    }
}
