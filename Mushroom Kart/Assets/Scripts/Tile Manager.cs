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

    [SerializeField] private Tile[] offRoadTiles;
    [SerializeField] private Tile[] offMapTiles;
    [SerializeField] private Tile[] jumpTiles;
    [SerializeField] private Tile[] bigJumpTiles;
    [SerializeField] private Tile[] speedBostTiles;
    [SerializeField] private Tile[] coinTiles;
    [SerializeField] private Tile[] itemBoxTiles;
    [SerializeField] private Tile[] disabledItemBoxTiles;

    void Awake()
    {
        Instance = this;
        for (int i = 0;i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Object Tilemap")
            {
                objectTilemap = transform.GetChild(i).GetComponent<Tilemap>();
                break;
            }
        }
    }

    public bool IsOffRoadTile(string tile)
    {
        foreach (Tile t in offRoadTiles)
        {
            if (t.name == tile) return true;
        }
        return false;
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

    public bool IsBigJumpTile(string tile)
    {
        foreach (Tile t in bigJumpTiles)
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
        Vector3Int[] offSet;

        for (int i = 0; i < 4; i++) {
            if (tile == itemBoxTiles[i])
            {
                offSet = GetOffSetList(i);
                objectTilemap.SetTile(pos + offSet[0], tiles[0]);
                objectTilemap.SetTile(pos + offSet[1], tiles[1]);
                objectTilemap.SetTile(pos + offSet[2], tiles[2]);
                objectTilemap.SetTile(pos + offSet[3], tiles[3]);
            }
        }
    }

    private Vector3Int[] GetOffSetList(int n)
    {
        if (n == 0) return new Vector3Int[4] { Vector3Int.zero, Vector3Int.right, Vector3Int.down, Vector3Int.right + Vector3Int.down };
        if (n == 1) return new Vector3Int[4] { Vector3Int.left, Vector3Int.zero, Vector3Int.down + Vector3Int.left, Vector3Int.down };
        if (n == 2) return new Vector3Int[4] { Vector3Int.up, Vector3Int.up+ Vector3Int.right, Vector3Int.zero, Vector3Int.right};
        return new Vector3Int[4] { Vector3Int.left + Vector3Int.up, Vector3Int.up, Vector3Int.left, Vector3Int.zero };
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
