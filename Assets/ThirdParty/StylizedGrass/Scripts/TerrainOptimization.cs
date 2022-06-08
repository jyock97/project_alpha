using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

[ExecuteInEditMode]
public class TerrainOptimization : MonoBehaviour
{

    [SerializeField]
    [Min(0.5f)]
    float m_HexRadius = 10f;

    [SerializeField]
    [Min(1f)]
    float m_LODMultiplier = 2f;

    [Space]
    [SerializeField]
    bool m_NeverCull = false;
    [SerializeField]
    List<GameObject> m_GrassToOptimize;

    [SerializeField]
    HexGrid m_HexGrid;

    [SerializeField]
    GrassHexNode[] m_GrassPatches;

    [SerializeField]
    Vector3 m_TerrainOffset;
    [SerializeField]
    int m_Height;
    [SerializeField]
    int m_Width;
    public void Initialize()
    {
        TerrainData terrain = GetComponent<Terrain>().terrainData;

        m_TerrainOffset = transform.position;
        m_HexGrid = new HexGrid(m_HexRadius,transform.position);
        m_Width = Mathf.CeilToInt(terrain.size.x / (m_HexGrid.InnerRadius * 2f)) + 1;
        m_Height = Mathf.CeilToInt(terrain.size.z / (m_HexGrid.OuterRadius * 1.5f)) + 1;

        m_GrassPatches = new GrassHexNode[m_Width * m_Height];


        foreach (var treeinstance in terrain.treeInstances)
        {
            if (m_GrassToOptimize.Contains(terrain.treePrototypes[treeinstance.prototypeIndex].prefab))
            {
                Vector3 worldPosition = new Vector3(treeinstance.position.x * terrain.size.x, treeinstance.position.y * terrain.size.y, treeinstance.position.z * terrain.size.z) + transform.position;

                Vector2Int HexPosition = m_HexGrid.WorldToHex(worldPosition);

                if (m_GrassPatches[FlattenArrayPosition(HexPosition.x, HexPosition.y)] == null)
                    m_GrassPatches[FlattenArrayPosition(HexPosition.x, HexPosition.y)] = new GrassHexNode(m_HexRadius, m_LODMultiplier);

                m_GrassPatches[FlattenArrayPosition(HexPosition.x, HexPosition.y)].AddMatrix(terrain.treePrototypes[treeinstance.prototypeIndex].prefab, worldPosition, Quaternion.Euler(0f, treeinstance.rotation, 0f), new Vector3(treeinstance.widthScale, treeinstance.heightScale, treeinstance.widthScale));
            }
        }


        var newTreePrototypes = terrain.treePrototypes.Where(x => (!m_GrassToOptimize.Contains(x.prefab))).ToArray();
        Dictionary<GameObject, int> RemappedFoliage = new Dictionary<GameObject, int>();

        for (int i = 0; i < newTreePrototypes.Length; i++)
            RemappedFoliage.Add(newTreePrototypes[i].prefab, i);

        var newTreeInstances = terrain.treeInstances.Where(x => (!m_GrassToOptimize.Contains(terrain.treePrototypes[x.prototypeIndex].prefab))).ToArray();

       for(int i=0;i<newTreeInstances.Length;i++)
        {
            newTreeInstances[i].prototypeIndex = RemappedFoliage[terrain.treePrototypes[newTreeInstances[i].prototypeIndex].prefab];
        }

        terrain.treeInstances = newTreeInstances;
        terrain.treePrototypes = newTreePrototypes;

    }

    public void UnInitialize() {
        TerrainData terrain = GetComponent<Terrain>().terrainData;

        List<TreeInstance> treeInstances = new List<TreeInstance>();
        Dictionary<GameObject,int> treeGameObjects = new Dictionary<GameObject, int>();

        foreach (var grassnode in m_GrassPatches)
        {
            var grasscollections = grassnode.grassCollections;
            foreach (var grasscollection in grasscollections)
            {
                if (!treeGameObjects.ContainsKey(grasscollection.prefab))
                {
                    treeGameObjects.Add(grasscollection.prefab, treeGameObjects.Count);
                }
                int index = treeGameObjects[grasscollection.prefab];

                var matrices = grasscollection.GetMatrices();
                foreach (var matrix in matrices)
                {
                    Vector3 position = matrix.GetColumn(3);
                    position -= m_TerrainOffset;

                    // Extract new local rotation
                    Quaternion rotation = Quaternion.LookRotation(
                        matrix.GetColumn(2),
                        matrix.GetColumn(1)
                    );
                    // Extract new local scale
                    Vector3 scale = new Vector3(
                        matrix.GetColumn(0).magnitude,
                        matrix.GetColumn(1).magnitude,
                        matrix.GetColumn(2).magnitude
                    );

                    TreeInstance treeInstance = new TreeInstance();
                    treeInstance.position = new Vector3(position.x / terrain.size.x,position.y / terrain.size.y, position.z / terrain.size.z);
                    treeInstance.rotation = rotation.eulerAngles.y;
                    treeInstance.heightScale = scale.y;
                    treeInstance.widthScale = scale.x;
                    treeInstance.prototypeIndex = index+terrain.treePrototypes.Length;

                    treeInstances.Add(treeInstance);
                }
            }
        }

        TreePrototype[] treeprototypes = new TreePrototype[treeGameObjects.Count];
        foreach (var gameobjecttree in treeGameObjects)
        {
            TreePrototype treePrototype = new TreePrototype();
            treePrototype.prefab = gameobjecttree.Key;

            treeprototypes[gameobjecttree.Value] = treePrototype;
        }

        terrain.treePrototypes = terrain.treePrototypes.Concat(treeprototypes).ToArray();
        terrain.treeInstances = terrain.treeInstances.Concat(treeInstances).ToArray();

        m_GrassPatches = null;
        m_Width = 0;
        m_Height = 0;
    }

    public bool IsInitialized() {
        return (m_GrassPatches != null);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (m_GrassPatches != null)
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    Vector3 Hexpos = m_HexGrid.HexToWorld(new Vector2Int(x, y));
                    Hexpos.y = transform.position.y;

                    m_HexGrid.DrawHex(Hexpos);
                }
            }
    }
    private void Draw(ScriptableRenderContext context, Camera camera)
    {
        if (camera.cameraType == CameraType.Preview)
            return;

        if (m_GrassPatches != null)
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    Vector3 worldpos = m_HexGrid.HexToWorld(new Vector2Int(x, y));
                    float distance = Vector3.Distance(camera.transform.position, worldpos);

                    m_GrassPatches[FlattenArrayPosition(x, y)]?.Draw(worldpos, distance, camera, m_NeverCull);
                }
            }

    }

    int FlattenArrayPosition(int x, int y)
    {
        return x + y * m_Width;
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering -= Draw;
        RenderPipelineManager.beginCameraRendering += Draw;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= Draw;
    }
}
