using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class WindowHandler : MonoBehaviour {


    //Import window changing function
    [DllImport("USER32.DLL")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    //Import find window function
    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
    static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

    //Import force window draw function
    [DllImport("user32.dll")]
    static extern bool DrawMenuBar(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);


    private readonly string WINDOW_NAME;            //name of the window
    private const int GWL_STYLE = -16;              //hex constant for style changing
    private const int WS_BORDER = 0x00800000;       //window with border
    private const int WS_CAPTION = 0x00C00000;      //window with a title bar
    private const int WS_SYSMENU = 0x00080000;      //window with no borders etc.
    private const int WS_MINIMIZEBOX = 0x00020000;  //window with minimizebox
    private const int WS_POPUP = unchecked((int)0x80000000);
    private const int SWP_SHOWWINDOW = 0x0040;      //displays the window

    float startTime = 0f;
    bool removedBorder = false;
    int firstWidth = 0;
    int firstHeight = 0;
    private bool didNotFindDll = false;

    public WindowState currentwinstate;

    public WindowHandler()
    {
        WINDOW_NAME = "Polar Virtual Reality Exhibit";
    }

    /// <summary>
    /// Removes all the borders but keep it in a window that cant be resized.
    /// </summary>
    /// <param name="_width">This should be the screen's resolution width (Unity should provide a propper method for this)</param>
    /// <param name="_height">This should be the screen's resolution width (Unity should provide a propper method for this)</param>
    public void WindowedMaximized(int _width, int _height)
    {
        try
        {
            IntPtr window = FindWindowByCaption(IntPtr.Zero, WINDOW_NAME);
            SetWindowLong(window, GWL_STYLE, WS_POPUP);//unchecked((int)0x80000000L));//WS_SYSMENU);
            SetWindowPos(window, -2, 0, 0, _width, _height, SWP_SHOWWINDOW);
            DrawMenuBar(window);
        }
        catch (DllNotFoundException ex)
        {
            Debug.LogWarning(ex);
            didNotFindDll = true;
        }
    }

    /// <summary>
    /// Make it into a window with borders etc.
    /// </summary>
    public void WindowedMode()
    {
        try
        {
            IntPtr window = FindWindowByCaption(IntPtr.Zero, WINDOW_NAME);
            SetWindowLong(window, GWL_STYLE, WS_CAPTION | WS_BORDER | WS_SYSMENU | WS_MINIMIZEBOX);
            DrawMenuBar(window);
        }
        catch (DllNotFoundException ex)
        {
            Debug.LogWarning(ex);
            didNotFindDll = true;
        }
    }

    public void MakePlayerWindow(int _width, int _height, bool fullscreen, WindowState winstate)
    {
        //this function should be filled with usefull code to manage the windows' states and handle the options.
    }

    public enum WindowState
    {
        FullScreen, Windowed, Maximized,
    }
 
    void Start()
    {
        //WindowedMode();
        firstWidth = Screen.width;
        firstHeight = Screen.height;
        Screen.SetResolution(Screen.width-1, Screen.height-1, false);
        startTime = UnityEngine.Time.time;
        //WindowedMaximized(1365, 767);
    }

    void Update()
    {
        if (!didNotFindDll && !removedBorder)
        {
            if (UnityEngine.Time.time - startTime > 3f)
            {
                WindowedMaximized(firstWidth - 1, firstHeight - 1);
                removedBorder = true;
            }
        }
    }
}
