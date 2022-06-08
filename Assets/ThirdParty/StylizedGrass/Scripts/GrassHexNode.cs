using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrassHexNode
{
    [SerializeField]
    float m_Height;
    [SerializeField]
    float m_HexSize;
    [SerializeField]
    float m_LODMultiplier;

    [SerializeField]
    SerializableDictionary<GameObject, GrassCollection> m_Grasses = new SerializableDictionary<GameObject, GrassCollection>();

    public IEnumerable<GrassCollection> grassCollections => m_Grasses.Values;

    public GrassHexNode(float hexSize, float lodMultiplier)
    {
        m_Height = 1f;
        m_LODMultiplier = lodMultiplier;
        m_HexSize = hexSize;
    }

    public void AddMatrix(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        if (!m_Grasses.ContainsKey(prefab))
        {
            m_Grasses.Add(prefab, new GrassCollection(prefab, m_HexSize, m_LODMultiplier));

            float height = m_Grasses[prefab].GetHeight();
            if (m_Height < height)
                m_Height = height;
        }

        m_Grasses[prefab].AddMatrix(position, rotation, scale);
    }
    public void Draw(Vector3 position, float distance,Camera camera, bool neverCull)
    {

        if (!neverCull)
            if (ShouldCull(position,camera))
                return;

        foreach (var grasscollection in m_Grasses.Values)
        {
            grasscollection.Draw(distance, camera);
        }
    }

    bool ShouldCull(Vector3 worldpos,Camera camera)
    {
        if (IsCameraInBounds(worldpos,camera))
            return false;

        float Height = m_Height;


        var pos1 = camera.WorldToScreenPoint(worldpos + new Vector3(-m_HexSize, -Height, -m_HexSize));
        var pos2 = camera.WorldToScreenPoint(worldpos + new Vector3(-m_HexSize, Height, -m_HexSize));
        var pos3 = camera.WorldToScreenPoint(worldpos + new Vector3(m_HexSize, Height, -m_HexSize));
        var pos4 = camera.WorldToScreenPoint(worldpos + new Vector3(m_HexSize, -Height, -m_HexSize));

        var pos5 = camera.WorldToScreenPoint(worldpos + new Vector3(-m_HexSize, -Height, m_HexSize));
        var pos6 = camera.WorldToScreenPoint(worldpos + new Vector3(-m_HexSize, Height, m_HexSize));
        var pos7 = camera.WorldToScreenPoint(worldpos + new Vector3(m_HexSize, Height, m_HexSize));
        var pos8 = camera.WorldToScreenPoint(worldpos + new Vector3(m_HexSize, -Height, m_HexSize));




        return !(IsInVision(pos1) || IsInVision(pos2) || IsInVision(pos3) || IsInVision(pos4) ||
                IsInVision(pos5) || IsInVision(pos6) || IsInVision(pos7) || IsInVision(pos8));
    }

    bool IsCameraInBounds(Vector3 worldpos,Camera camera)
    {
        float xmin, xmax, ymin, ymax, zmin, zmax;

        xmin = worldpos.x - m_HexSize;
        xmax = worldpos.x + m_HexSize;

        ymin = worldpos.y - m_Height * 10;
        ymax = worldpos.y + m_Height * 10;

        zmin = worldpos.z - m_HexSize;
        zmax = worldpos.z + m_HexSize;

        return (xmin <= camera.transform.position.x && camera.transform.position.x <= xmax) && (ymin <= camera.transform.position.y && camera.transform.position.y <= ymax) && (zmin <= camera.transform.position.z && camera.transform.position.z <= zmax);
    }
    bool IsInVision(Vector3 position)
    {
        if (position.z < 0)
            return false;

        return Screen.safeArea.Contains(position);
    }
}