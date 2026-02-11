using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR

public class ReadMe : MonoBehaviour
{
    [SerializeField] private TEST_GD testGd;
    [SerializeField] private DominoMovementController mvtController;
    [SerializeField] private DominoSpawner spawner;
    [SerializeField] private GridManager gridmanager;
    [SerializeField] private GridDrawer origin;
}



[CustomEditor(typeof(ReadMe))]
public class ReadmeEditor : Editor 
{
    public override void OnInspectorGUI() 
    { 
        var readme = (ReadMe)target; 

        DrawDefaultInspector();

        EditorGUILayout.Space(20);

        var style = new GUIStyle(EditorStyles.label); 
        style.normal.textColor = Color.white; // ou new Color(r,g,b) EditorGUILayout.LabelField("Texte rouge",
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        var stylewrapp = new GUIStyle(EditorStyles.wordWrappedLabel);

        EditorGUILayout.LabelField("GUIDE POUR LES GD", style);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Pour modifier des parametres concernant le jeu, il faut aller sur les GameObjects corespondants ", stylewrapp);


        EditorGUILayout.Space(25);

        GUI.enabled = false;

        var prop = serializedObject.FindProperty("testGd");
        EditorGUILayout.PropertyField(prop, new GUIContent("Double cliquez :"));

        GUI.enabled = true;


        EditorGUILayout.LabelField("-> Il y a des options a tester (fall fluide ou case par case ?", stylewrapp);

        EditorGUILayout.Space(20);

        GUI.enabled = false;

        var prop1 = serializedObject.FindProperty("mvtController");
        EditorGUILayout.PropertyField(prop1, new GUIContent("Double cliquez :"));

        GUI.enabled = true;

        EditorGUILayout.LabelField("-> changer les parametres de controles", stylewrapp);

        EditorGUILayout.Space(20);

        GUI.enabled = false;

        var prop2 = serializedObject.FindProperty("spawner");
        EditorGUILayout.PropertyField(prop2, new GUIContent("Double cliquez :"));

        GUI.enabled = true;

        EditorGUILayout.LabelField("-> changer les paramètres de spawn", stylewrapp);

        EditorGUILayout.Space(20);

        GUI.enabled = false;

        var prop3 = serializedObject.FindProperty("gridmanager");
        EditorGUILayout.PropertyField(prop3, new GUIContent("Double cliquez :"));

        GUI.enabled = true;

        EditorGUILayout.LabelField("-> changer les paramètres de la grille", stylewrapp);

        EditorGUILayout.Space(20);

        GUI.enabled = false;

        var prop4 = serializedObject.FindProperty("origin");
        EditorGUILayout.PropertyField(prop4, new GUIContent("Double cliquez :"));

        GUI.enabled = true;

        EditorGUILayout.LabelField("-> UNIQUEMENT : modifier sa position pour changer la position de la grille ", stylewrapp);

    }


}
#endif