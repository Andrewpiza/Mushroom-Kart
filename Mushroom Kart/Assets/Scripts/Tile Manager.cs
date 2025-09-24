using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    private Tilemap objectTilemap;

    private const float COIN_RESPAWN_TIME = 2;

    [SerializeField] private Tile[] offMapTiles;
    [SerializeField] private Tile[] jumpTiles;
    [SerializeField] private Tile[] speedBostTiles;
    [SerializeField] private Tile[] coinTiles;

    void Awake()
    {
        Instance = this;
        objectTilemap = transform.GetChild(1).GetComponent<Tilemap>();
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

    public IEnumerator RespawnCoin(Vector3Int pos, TileBase tile)
    {
        objectTilemap.SetTile(pos, null);
        yield return new WaitForSeconds(COIN_RESPAWN_TIME);
        objectTilemap.SetTile(pos, tile);
    }
}
