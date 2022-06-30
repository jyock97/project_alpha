using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainOptimization))]
public class TerrainOptimizationEditor : Editor
{
    SerializedProperty m_HexRadius;
    SerializedProperty m_LODMultiplier;
    SerializedProperty m_NeverCull;
    SerializedProperty m_GrassToOptimize;

    void OnEnable()
    {
        m_HexRadius = serializedObject.FindProperty("m_HexRadius");
        m_LODMultiplier = serializedObject.FindProperty("m_LODMultiplier");
        m_NeverCull = serializedObject.FindProperty("m_NeverCull");
        m_GrassToOptimize = serializedObject.FindProperty("m_GrassToOptimize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_GrassToOptimize);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_HexRadius);
        EditorGUILayout.PropertyField(m_LODMultiplier);
        EditorGUILayout.PropertyField(m_NeverCull);

        EditorGUILayout.Space();

        TerrainOptimization terrainOptimization = (target as TerrainOptimization);

        GUI.enabled = !terrainOptimization.IsInitialized();
        if (GUILayout.Button("Convert To Optimized Grass"))
        {
            terrainOptimization.Initialize();
        }

        GUI.enabled = terrainOptimization.IsInitialized();
        if (GUILayout.Button("Convert Back To Terrain Grass"))
        {
            terrainOptimization.UnInitialize();
        }
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }
}
