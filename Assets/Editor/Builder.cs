using UnityEditor;
using UnityEngine;

public class Builder : MonoBehaviour
{
    static string[] m_Scenes = new string[] {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/Main.unity"
    };

    [MenuItem("Build/Build Win32")]
    public static void BuildWin32()
    {
        BuildPipeline.BuildPlayer(m_Scenes, "Build/Win32/tanks.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    }
}