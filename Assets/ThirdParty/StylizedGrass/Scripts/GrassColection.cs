using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
class MatrixCollection {
    [SerializeField]
    public List<Matrix4x4> Matrices = new List<Matrix4x4>();
}
[System.Serializable]
public class GrassCollection
{
    MeshFilter[] m_GrassLODMeshes;
    LOD[] m_LODs;

    [SerializeField]
    GameObject m_Prefab;

    [SerializeField]
    float m_LODMultiplier = 2f;
    [SerializeField]
    float m_HexRadiusSize = 10f;

    [SerializeField]
    List<MatrixCollection> m_Matrices; //Grouped by a size of 1023

    public GameObject prefab => m_Prefab;

    public List<Matrix4x4> GetMatrices() {
        List<Matrix4x4> matrices = new List<Matrix4x4>();
        
        foreach (MatrixCollection matrixCollection in m_Matrices)
        {
            matrices.AddRange(matrixCollection.Matrices);
        }

        return matrices;
    }
    public GrassCollection(GameObject prefab, float hexRadius, float LODMultiplier)
    {
        m_Prefab = prefab;
        m_HexRadiusSize = hexRadius;
        m_LODMultiplier = LODMultiplier;

        m_Matrices = new List<MatrixCollection>() { new MatrixCollection() };

        Initialize();
    }

    void Initialize() {
        m_GrassLODMeshes = m_Prefab.GetComponentsInChildren<MeshFilter>();
        m_LODs = m_Prefab.GetComponent<LODGroup>().GetLODs();
    }

    public float GetHeight()
    {
        return m_LODs[0].renderers[0].bounds.size.y;
    }

    public void AddMatrix(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        int maxSize = 1023;

        int CurrentIndex = m_Matrices.Count - 1;
        if (m_Matrices[CurrentIndex].Matrices.Count >= maxSize)
        {
            m_Matrices.Add(new MatrixCollection());
            CurrentIndex++;
        }

        Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale) * m_GrassLODMeshes[0].transform.localToWorldMatrix;
        m_Matrices[CurrentIndex].Matrices.Add(matrix);
    }

    public void Draw(float distance,Camera camera)
    {
        if (m_LODs == null)
        {
            Initialize();
        }

        int LOD = CalculateLOD(distance,camera);

        if (LOD >= m_LODs.Length) //Culling
            return;

        foreach (var list in m_Matrices)
        {
            Graphics.DrawMeshInstanced(m_GrassLODMeshes[LOD].sharedMesh, 0, m_LODs[LOD].renderers[0].sharedMaterial, list.Matrices, new MaterialPropertyBlock(), UnityEngine.Rendering.ShadowCastingMode.Off, true, 0, camera);
        }
    }


    int CalculateLOD(float distance,Camera camera)
    {
        Vector3 rendererBoundsSize = m_LODs[0].renderers[0].bounds.size;

        float size = Mathf.Max(rendererBoundsSize.x, rendererBoundsSize.y, rendererBoundsSize.z);


        float screenSpaceHeight = Mathf.Abs(Mathf.Atan2(size, distance) * Mathf.Rad2Deg * QualitySettings.lodBias) / camera.fieldOfView;

        float RadiusScale = m_HexRadiusSize * m_LODMultiplier;


        for (int i = 0; i < m_LODs.Length; i++)
        {
            if (screenSpaceHeight > m_LODs[i].screenRelativeTransitionHeight / RadiusScale)
                return i;
        }
        return m_LODs.Length;
    }
}