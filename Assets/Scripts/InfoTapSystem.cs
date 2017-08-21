using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class InfoTapSystem : MonoBehaviour
{
    private const int MAX_INFO_MSGS = 5;

    [SerializeField]
    // The prefab to instantiate for every double tap
    private GameObject template;

    struct InfoBounds
    {
        //bounds in viewport space
        public Vector2 minBounds;
        public Vector2 maxBounds;
        public string infoMsg;
        public Vector3 panelPosition;
    };

    InfoBounds[] infoArray = new InfoBounds[MAX_INFO_MSGS];
    // Use this for initialization
    void Start()
    {
        infoArray[0].minBounds.Set(0.415625f, 0.6f);
        infoArray[0].maxBounds.Set(0.58f, 0.9f);
        infoArray[0].infoMsg = "You tapped the ICL";
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        GetComponent<TapGesture>().Tapped += tappedHandler;
    }

    private void OnDisable()
    {
        GetComponent<TapGesture>().Tapped -= tappedHandler;
    }

    private void tappedHandler(object sender, System.EventArgs e)
    {
        Debug.Log("double tap");
        Vector2 screenPos = GetComponent<TapGesture>().NormalizedScreenPosition;
        for(int i = 0; i < MAX_INFO_MSGS; ++i)
        {
            if(screenPos.x > infoArray[i].minBounds.x && screenPos.x < infoArray[i].maxBounds.x &&
                screenPos.y > infoArray[i].minBounds.y && screenPos.y < infoArray[i].maxBounds.y)
            {
                //show the string on screen in some sort of UI bubble..
                Debug.Log(infoArray[i].infoMsg);
                if(template != null)
                {
                    GameObject newPanel = Instantiate(template);
                    newPanel.GetComponent<RectTransform>().position = infoArray[i].panelPosition;
                    //have it stay visible for some time amount...
                }
            }
        }
    }
}
