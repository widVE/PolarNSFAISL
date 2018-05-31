using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{

    public static LocalizationManager instance;

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    public GameObject gameMode;
    public bool spanish = false;

    //public GameObject langChoice;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        localizedText = new Dictionary<string, string>();
    }

    void Start()
    {
        LoadLocalizedText("localizedText_en.json");
    }

    public void LoadLocalizedText(string fileName)
    {
        localizedText.Clear();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }

        //
        if (fileName.Equals("localizedText_en.json"))
        {
            spanish = false;
            //langChoice.GetComponent<UnityEngine.UI.SpanishChoice>().interactable = false;
            Debug.Log("loaded english survey");
        }
        else
        {
            spanish = true;
            Debug.Log("loaded spanish survey");
        }

        isReady = true;

        updateLocalizedTexts();
        UpdateSummaryPanel();
        UpdateEventPanel();
        
        SwipeRecognizer s = UnityEngine.Camera.main.gameObject.GetComponent<SwipeRecognizer>();
        if(s != null)
        {
            s.updateScore();
        }

        SwipeGameMode hs = gameMode.GetComponent<SwipeGameMode>();
        if (hs != null)
        {
            hs.HighScore();
        }
    }

    public void UpdateSummaryPanel()
    {
        GameObject g = GameObject.Find("SummaryPanel");
        if (g != null)
        {
            for(int i = 0; i < g.transform.childCount; ++i)
            {
                GameObject c = g.transform.GetChild(i).gameObject;
                if(c.name.StartsWith("Event: "))
                {
                    EventInfo ei = c.GetComponent<EventInfo>();
                    if(ei != null)
                    {
                        ei.OnEnable();
                    }
                }
                if (c.name.StartsWith("SummaryText"))
                {
                    Debug.Log("updating summary....");
                    SwipeRecognizer s = UnityEngine.Camera.main.gameObject.GetComponent<SwipeRecognizer>();
                    SwipeGameMode sc = gameMode.GetComponent<SwipeGameMode>();

                    g.transform.GetChild(4).gameObject.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("game_summary1") + s.neutrinoCount +
                                   LocalizationManager.instance.GetLocalizedValue("game_summary2") + "\n" + LocalizationManager.instance.GetLocalizedValue("game_summary3") + " " + s.neutrinoScore;
                }
            }
        }
    }

    public void UpdateEventPanel()
    {
        GameObject e = GameObject.Find("EventPanel");
        if (e != null)
        {
            for (int i = 0; i < e.transform.childCount; ++i)
            {
                GameObject cmp = e.transform.GetChild(i).gameObject;

                if (cmp.name.StartsWith("Event: "))
                {
                    EventInfo eii = cmp.GetComponent<EventInfo>();
                    if (eii != null)
                    {
                        eii.OnEnable();
                    }
                }
            }
        }
    }

    //translates the text in text script
    public void updateLocalizedTexts()
    {
        LocalizedText[] components = Resources.FindObjectsOfTypeAll<LocalizedText>(); 
        foreach(LocalizedText component in components)
        {
            Text text = component.gameObject.GetComponent<Text>();
            text.text = LocalizationManager.instance.GetLocalizedValue(component.key);
        }
    }

    public string GetLocalizedValue(string key)
    {
        string result = missingTextString;
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;

    }

    public bool GetIsReady()
    {
        return isReady;
    }

}