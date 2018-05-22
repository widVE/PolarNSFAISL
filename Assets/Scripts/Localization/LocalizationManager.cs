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

        isReady = true;

        updateLocalizedTexts();
        UpdateSummaryPanel();
        UpdateEventPanel();
        
        SwipeRecognizer s = UnityEngine.Camera.main.gameObject.GetComponent<SwipeRecognizer>();
        if(s != null)
        {
            s.updateScore();
        }

        SwipeGameMode hs = gameObject.GetComponent<SwipeGameMode>();
        if (hs != null)
        {
            hs.StopGame();
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