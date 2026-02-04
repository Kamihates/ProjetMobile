using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.MessageBox;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // affiche l'editor de base du script
        Repaint();

        GridManager grid = (GridManager)target;

        EditorGUILayout.Space();

        GUILayout.Label("-- GRILLE --");

        if (grid.GridData == null) return;


        for (int row = 0; row < grid.GridData.Count; ++row)
        {
            EditorGUILayout.BeginHorizontal();

            for (int col = 0; col < grid.GridData[row].Count; col++)
            {
                if (grid.GridData[row][col] != null)
                {
                    if (grid.GridData[row][col].Region != null)
                        GUILayout.Button(grid.GridData[row][col].Region.Type.ToString()[0].ToString(), GUILayout.Width(20), GUILayout.Height(20));
                }
                else
                {
                    if (GUILayout.Button("/", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        // si on clique sur le bouton, ca ajoute un region data au pif
                        RegionPiece piece = new RegionPiece();
                        piece.Init(new RegionData { RegionID = 3, Type = RegionType.Urbain });

                        grid.GridData[row][col] = piece;

                        EditorUtility.SetDirty(target);

                    }
                }

            }

            EditorGUILayout.EndHorizontal();
        }

    }
}
