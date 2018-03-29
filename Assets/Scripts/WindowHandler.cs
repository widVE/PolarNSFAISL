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
    private const int SWP_SHOWWINDOW = 0x0040;      //displays the window

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
        IntPtr window = FindWindowByCaption(IntPtr.Zero, WINDOW_NAME);
        SetWindowLong(window, GWL_STYLE, WS_SYSMENU);
        SetWindowPos(window, -2, 0, 0, _width, _height, SWP_SHOWWINDOW);
        DrawMenuBar(window);

    }

    /// <summary>
    /// Make it into a window with borders etc.
    /// </summary>
    public void WindowedMode()
    {
        IntPtr window = FindWindowByCaption(IntPtr.Zero, WINDOW_NAME);
        SetWindowLong(window, GWL_STYLE, WS_CAPTION | WS_BORDER | WS_SYSMENU | WS_MINIMIZEBOX);
        DrawMenuBar(window);
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
        WindowedMaximized(1366, 768);
    }
}
