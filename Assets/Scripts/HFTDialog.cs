using UnityEngine;
using System;

// example:
// HFTDialog.MessageBox("error", "Sorry but you're S.O.L", () => { Application.Quit() });

public class HFTDialog : MonoBehaviour
{
    private GUIStyle windowStyle = new GUIStyle();

    Rect m_windowRect;
    Action m_action;
    string m_title;
    string m_msg;

    static public void MessageBox(string title, string msg, Action action)
    {
        GameObject go = new GameObject("HFTDialog");
        HFTDialog dlg = go.AddComponent<HFTDialog>();
        dlg.Init(title, msg); //, action);
    }


    void Start()
    {
        Texture2D bg = new Texture2D(1, 1);
        bg.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 1.0f));
        windowStyle.normal.textColor = Color.red;
        /*
        customSkin.customStyles [0].alignment = TextAnchor.MiddleCenter;
        customSkin.customStyles [0].normal.textColor = Color.black;
        customSkin.customStyles [0].fontSize = Mathf.CeilToInt (105 * scale_y);
        GUI.Label(new Rect (630 * scale_x, 85 * scale_y, 250 * scale_x, 180 * scale_y),"STORE",customSkin.customStyles[0]);
        closeIAPRect = new Rect (1280 * scale_x, 105 * scale_y, 95 * scale_x, 100 * scale_y);
        GUI.DrawTexture (new Rect (1280 * scale_x, 105 * scale_y, 95 * scale_x, 100 * scale_y), closeIAP);
        customStyle.fontSize = Mathf.CeilToInt (66 * scale_y);
        customStyle.normal.textColor = Color.black;
        */
    }

    void Init(string title, string msg) //, Action action)
    {
        m_title = title;
        m_msg = msg;
        //m_action = action;
    }

    void OnGUI()
    {
        //const int maxWidth = 640;
        //const int maxHeight = 480;

        int width = Screen.width - 20; //Mathf.Min(maxWidth, Screen.width - 20);
        int height = Screen.height - 20; //Mathf.Min(maxHeight, Screen.height - 20);
        m_windowRect = new Rect(
            (Screen.width - width) / 2,
            (Screen.height - height) / 2,
            width,
            height);


        Color tmpColor = GUI.color;
        GUI.color = new Color(1, 1, 1, 1.0f);
        //GUILayout.Window(0, m_windowRect, WindowFunc, m_title); 
        m_windowRect = GUI.Window(0, m_windowRect, WindowFunc, m_title,windowStyle);
        //GUI.color = tmpColor;


    }

    void WindowFunc(int windowID)
    {
        const int border = 10;
        const int width = 50;
        const int height = 25;
        const int spacing = 10;


        // main rect
        Rect l = new Rect(
            border,
            border + spacing,
            m_windowRect.width,  // - border * 2,
            m_windowRect.height); // - border * 2 - height - spacing);
        GUI.Label(l, m_msg);

        //buttons
        Rect a = new Rect(
            m_windowRect.width - width - border,
            m_windowRect.height - height - border,
            width,
            height);

        if (GUI.Button(a, "1"))
        {
            Destroy(this.gameObject);
            Debug.Log("Press 1");
            //m_action();
        }

        Rect b = new Rect(
            m_windowRect.width - width - 2*border,
            m_windowRect.height - height - border,
            width,
            height);

        if (GUI.Button(b, "2"))
        {
            Destroy(this.gameObject);
            Debug.Log("Press 2");
            //m_action();
        }

    }
}