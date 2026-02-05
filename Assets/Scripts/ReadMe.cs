using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public class ReadMe : MonoBehaviour
{
}

[CustomEditor(typeof(ReadMe))]
public class ReadmeEditor : Editor 
{
    public override void OnInspectorGUI() 
    { 
        var readme = (ReadMe)target; 

        EditorGUILayout.Space(); DrawDefaultInspector();

        EditorGUILayout.HelpBox("Ceci est un texte explicatif sur plusieurs lignes. " + "Parfait pour un mini README dans l’inspecteur.", MessageType.Info);


    }
}