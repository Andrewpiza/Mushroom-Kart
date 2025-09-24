using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    [SerializeField] private Tile[] offMapTiles;
    [SerializeField] private Tile[] jumpTiles;
    [SerializeField] private Tile[] speedBostTiles;

    void Awake()
    {
        Instance = this;
    }

    public bool IsOffMapTile(string tile)
    {
        foreach (Tile t in offMapTiles)
        {
            if (t.name == tile) return true;
        }
        return false;
    }

    public bool IsJumpTile(string tile)
    {
        foreach (Tile t in jumpTiles)
        {
            if (t.name == tile) return true;
        }
        return false;
    }
    
    public bool IsSpeedBoostTile(string tile)
    {
        foreach (Tile t in speedBostTiles)
        {
            if (t.name == tile)return true;
        }
        return false;
    }
}
