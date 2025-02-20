using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ExampleWindow : EditorWindow
{

    Color color;

    [MenuItem("Window/Colorizer")]
    public static void ShowWindow()
    {
        GetWindow<ExampleWindow>("Colorizer");
    }
        
    private void OnGUI()
    {
        GUILayout.Label("Color the selected obj.",EditorStyles.boldLabel);
        color = EditorGUILayout.ColorField("Color", color);

        if(GUILayout.Button("Colorize"))
        {
            Colorized();
        }
    }
   
    public void Colorized()
    {

        foreach (GameObject obj in Selection.gameObjects)
        {

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial.color = color;
            }
        }
    }


}
