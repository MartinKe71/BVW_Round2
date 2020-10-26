using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDetector : MonoBehaviour
{
    private TerrainData _terrainData;
    private int _alphamapWidth;
    private int _alphamapHeight;
    private float[,,] _splatmapData;
    private int _numTextures;

    public TerrainDetector()
    {
        _terrainData = Terrain.activeTerrain.terrainData;
        _alphamapWidth = _terrainData.alphamapWidth;
        _alphamapHeight = _terrainData.alphamapHeight;

        _splatmapData = _terrainData.GetAlphamaps(0, 0, _alphamapWidth, _alphamapHeight);
        _numTextures = _splatmapData.Length / (_alphamapWidth * _alphamapHeight);
    }

    public int GetTerrainTextureIndex(Vector3 pos)
    {
        Vector3 terrainCord = ConvertWorldToTerrainCord(pos);
        int activeTextureIndex = 0;
        float largestOpacity = 0f;

        for (int i = 0; i < _numTextures; i++)
        {
            if (largestOpacity < _splatmapData[(int)terrainCord.z, (int)terrainCord.x, i])
            {
                activeTextureIndex = i;
                largestOpacity = _splatmapData[(int)terrainCord.z, (int)terrainCord.x, i];
            }
        }
        return activeTextureIndex;
    }

    private Vector3 ConvertWorldToTerrainCord(Vector3 pos)
    {
        Vector3 terrainCord = new Vector3();
        Terrain terrain = Terrain.activeTerrain;
        Vector3 terrainPos = terrain.transform.position;
        terrainCord.x = ((pos.x - terrainPos.x) / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth;
        terrainCord.z = ((pos.z - terrainPos.z) / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight;

        return terrainCord;
    }
}
