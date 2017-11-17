using System.Reflection;
using UnityEngine;
using System.Collections;

[assembly: AssemblyVersion("1.0.*")]

//[RequireComponent(typeof(UnityEngine.UI.Text))]

public class BuildDate : MonoBehaviour
{
    [Tooltip("Date/time format.")]
    public string format = "g";    // see: https://msdn.microsoft.com/en-us/library/az4se3k1%28v=vs.110%29.aspx

    void Start()
    {
        //GetComponent<UnityEngine.UI.Text>().text = BuildDate.ToString(format);
    }

    public static System.Version Version()
    {
        return Assembly.GetExecutingAssembly().GetName().Version;
    }

    public static System.DateTime Date()
    {
        System.Version version = Version();
        System.DateTime startDate = new System.DateTime(2000, 1, 1, 0, 0, 0);
        System.TimeSpan span = new System.TimeSpan(version.Build, 0, 0, version.Revision * 2);
        System.DateTime buildDate = startDate.Add(span);
        return buildDate;
    }

    public static string ToString(string format = null)
    {
        System.DateTime date = Date();
        return date.ToString(format);
    }
}