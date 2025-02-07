using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class ModifyPodfile
{
    [PostProcessBuild(45)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string podfilePath = Path.Combine(buildPath, "Podfile");

            if (File.Exists(podfilePath))
            {
                string podfileContents = File.ReadAllText(podfilePath);

                string targetString = "target 'Unity-iPhone' do";
                string podLines = "\n  pod 'DIOSDK', '4.3.1'\n  pod 'AppLovin-DIO-Adapter', '4.3.1'";

                // Ensure DIOSDK and AppLovin-DIO-Adapter are inside the Unity-iPhone target block
                if (!podfileContents.Contains("pod 'DIOSDK', '4.3.1'") || !podfileContents.Contains("pod 'AppLovin-DIO-Adapter', '4.3.1'"))
                {
                    int index = podfileContents.IndexOf(targetString);
                    if (index != -1)
                    {
                        int endIndex = podfileContents.IndexOf("end", index);
                        if (endIndex != -1)
                        {
                            podfileContents = podfileContents.Insert(endIndex, podLines);
                            File.WriteAllText(podfilePath, podfileContents);
                            Debug.Log("[ModifyPodfile] Successfully added DIOSDK and AppLovin-DIO-Adapter inside Unity-iPhone target.");
                        }
                    }
                }
                else
                {
                    Debug.Log("[ModifyPodfile] Pods already exist in the correct place. Skipping modification.");
                }
            }
            else
            {
                Debug.LogError("[ModifyPodfile] Podfile not found! Make sure CocoaPods integration is enabled in Unity.");
            }
        }
    }
}
