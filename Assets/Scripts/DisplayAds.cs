using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DisplayAds : MonoBehaviour
{

    public const string url1 = "http://lab.greedygame.com/arpit-dev/unity-assignment/templates/text_only.json";
    public const string url2 = "http://lab.greedygame.com/arpit-dev/unity-assignment/templates/text_color.json";
    public const string url3 = "http://lab.greedygame.com/arpit-dev/unity-assignment/templates/frame_only.json";
    public const string url4 = "http://lab.greedygame.com/arpit-dev/unity-assignment/templates/frame_color.json";

    public Text enterTxt;
    public Text txtDisplay;
    public RawImage imageDisplay;
    public Text invalidTxt;


    public void EnterButtonClicked()
    {
        txtDisplay.text = enterTxt.text;
    }

    public void OnDisplay1Click()
    {
        StartCoroutine(GetData(url1));

    }
    public void OnDisplay2Click()
    {
        StartCoroutine(GetData(url2));

    }
    public void OnDisplay3Click()
    {
        StartCoroutine(GetData(url3));

    }
    public void OnDisplay4Click()
    {
        StartCoroutine(GetData(url4));
    }

    IEnumerator GetData(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            layersList result = JsonUtility.FromJson<layersList>(www.downloadHandler.text);
            ProcessData(result);
        }
    }

    void ProcessData(layersList result)
    {
        switch (result.layers[0].type)
        {
            case "text":
                ChangePosition(txtDisplay.gameObject, result);
                if (result.layers[0].operations.Count > 0)
                {
                    if (ColorUtility.TryParseHtmlString(result.layers[0].operations[0].argument, out Color newCol))
                        txtDisplay.color = newCol;
                }
                break;
            case "frame":
                ChangePosition(imageDisplay.gameObject, result);
                StartCoroutine(setImage(result.layers[0].path, result));
                break;
        }
    }

    void ChangePosition(GameObject _gameObject, layersList layersList)
    {
        _gameObject.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(layersList.layers[0].placement[0].position.x, layersList.layers[0].placement[0].position.y, 0);
        _gameObject.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(layersList.layers[0].placement[0].position.width, layersList.layers[0].placement[0].position.height);
    }

    IEnumerator setImage(string url, layersList result)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.error != null)
        {
            //invalid image can be handled from here
            invalidTxt.text = "INVALID IMAGE";
            Debug.Log("Invalid Image Link");
        }
        else
        {
            imageDisplay.texture = www.texture;
            if (result.layers[0].operations.Count > 0)
            {
                if (ColorUtility.TryParseHtmlString(result.layers[0].operations[0].argument, out Color newCol))
                    imageDisplay.color = newCol;
            }
        }
    }
}

public class layersList
{
    public List<layers> layers = new List<layers>();
}

[System.Serializable]
public class layers
{
    public string type;
    public List<placement> placement = new List<placement>();
    public List<operations> operations = new List<operations>();
    public string path;
}

[System.Serializable]
public class placement
{
    public position position;
}
[System.Serializable]
public class position
{
    public float x;
    public float y;
    public float width;
    public float height;

}

[System.Serializable]
public class operations
{
    public string name;
    public string argument;
}
