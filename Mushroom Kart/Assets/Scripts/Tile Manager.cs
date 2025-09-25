using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    private Tilemap objectTilemap;

    private const float COIN_RESPAWN_TIME = 1.75f;
    private const float ITEM_BOX_RESPAWN_TIME = 2.5f;

    [SerializeField] private Tile[] offMapTiles;
    [SerializeField] private Tile[] jumpTiles;
    [SerializeField] private Tile[] speedBostTiles;
    [SerializeField] private Tile[] coinTiles;
    [SerializeField] private Tile[] itemBoxTiles;
    [SerializeField] private Tile[] disabledItemBoxTiles;

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

    public bool IsItemBoxTile(string tile)
    {
        foreach (Tile t in itemBoxTiles)
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
    
    public IEnumerator RespawnItemBox(Vector3Int pos, TileBase tile)
    {
        SetItemBoxTiles(pos, tile, disabledItemBoxTiles);
        
        yield return new WaitForSeconds(ITEM_BOX_RESPAWN_TIME);
        SetItemBoxTiles(pos, tile, itemBoxTiles);
    }

    private void SetItemBoxTiles(Vector3Int pos, TileBase tile, Tile[] tiles)
    {
        if (tile == itemBoxTiles[0]){
            objectTilemap.SetTile(pos,tiles[0]);
            objectTilemap.SetTile(pos + Vector3Int.right,tiles[1]);
            objectTilemap.SetTile(pos + Vector3Int.down,tiles[2]);
            objectTilemap.SetTile(pos + Vector3Int.right + Vector3Int.down,tiles[3]);
        }
        else if (tile == itemBoxTiles[1]){
            objectTilemap.SetTile(pos,tiles[1]);
            objectTilemap.SetTile(pos + Vector3Int.left,tiles[0]);
            objectTilemap.SetTile(pos + Vector3Int.down,tiles[3]);
            objectTilemap.SetTile(pos + Vector3Int.left + Vector3Int.down,tiles[2]);
        }
        else if (tile == itemBoxTiles[2]){
            objectTilemap.SetTile(pos,tiles[2]);
            objectTilemap.SetTile(pos + Vector3Int.up,tiles[0]);
            objectTilemap.SetTile(pos + Vector3Int.right,tiles[3]);
            objectTilemap.SetTile(pos + Vector3Int.up + Vector3Int.right,tiles[1]);
        }
        else if (tile == itemBoxTiles[3]){
            objectTilemap.SetTile(pos,tiles[3]);
            objectTilemap.SetTile(pos + Vector3Int.left,tiles[2]);
            objectTilemap.SetTile(pos + Vector3Int.up,tiles[1]);
            objectTilemap.SetTile(pos + Vector3Int.left + Vector3Int.up,tiles[0]);
        }
    }

    public Vector3Int FindNearbyTile(Vector3Int pos)
    {
        TileBase tile;
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                tile = objectTilemap.GetTile(pos + new Vector3Int(x, y, 0));
                if (tile)
                {
                    return pos + new Vector3Int(x, y, 0);
                }
            }
        }
        return pos;
    }
}
