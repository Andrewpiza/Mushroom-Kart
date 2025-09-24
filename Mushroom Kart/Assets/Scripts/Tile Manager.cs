using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    private Tilemap objectTilemap;

    [SerializeField] private Tile[] offMapTiles;
    [SerializeField] private Tile[] jumpTiles;
    [SerializeField] private Tile[] speedBostTiles;
    [SerializeField] private Tile[] coinTiles;
    private List<Vector3Int> respawningCoins;

    void Awake()
    {
        Instance = this;
        respawningCoins = new List<Vector3Int>();
        objectTilemap = transform.GetChild(1).GetComponent<Tilemap>();
    }

    void Update()
    {
        
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
            if (t.name == tile) return true;
        }
        return false;
    }

    public bool IsCoinTile(string tile)
    {
        foreach (Tile t in coinTiles)
        {
            if (t.name == tile) return true;
        }
        return false;
    }

    public void CoinRespawn(Vector3Int pos)
    {
        objectTilemap.SetTile(pos, null);
        respawningCoins.Add(pos);
    }
}
