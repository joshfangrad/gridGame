using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileGridTypes;
using TileTypes;

public class TileGrid
{ 
    public Tile[,] tileGrid;
    public TileGridType tileGridType;
    public Vector2Int offset;
    public Vector2Int size;

    public TileGrid(TileGridType _tileGridType, Vector2Int _size, Vector2Int _offset = new Vector2Int())
    {
        tileGridType = _tileGridType;
        offset = _offset;
        size = _size;
        //create 2d array of tiles and populate
        tileGrid = new Tile[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                tileGrid[x, y] = new Tile(new Vector2Int(x, y));
            }
        }

        GameObject.Find("floor").GetComponent<FloorMaker>().CreateGridTiles(size, offset);
    }


    // adds a GameObject to a tile.
    public bool AddToTile(GameObject gameObject, Vector2Int targetTile, Vector3 objectOffset, Vector3 objectScale)
    {

        Vector3 pos = new Vector3(targetTile.x + offset.x + objectOffset.x, objectOffset.y, targetTile.y + offset.y + objectOffset.z);
        return tileGrid[targetTile.x, targetTile.y].AddObjectToTile(gameObject, pos, objectScale);
    }


    // removes an object from a tile
    public bool RemoveFromTile(Vector2Int targetTile)
    {

        Tile tile = tileGrid[targetTile.x, targetTile.y];
        //if the tile is a crop, add currency
        if (tile.IsCropFullyGrown())
        {
            GameSystem.Instance.AddCurrency(1);
        }
        return tile.RemoveObjectFromTile();
    }


    public List<Tile> GetTilesInRadius(Vector2Int pos, TileType tileType, int radius)
    {
        if (radius <= 0) { return new List<Tile>(); }

        int bottomX = Mathf.Clamp(pos.x - radius, 0, size.x-1), bottomY = Mathf.Clamp(pos.y - radius, 0, size.y-1);
        int topX = Mathf.Clamp(pos.x + radius, 0, size.x-1), topY = Mathf.Clamp(pos.y + radius, 0, size.y-1);

        List<Tile> tiles = new List<Tile>();
        for (int x = bottomX; x <= topX; x++)
        {
            for (int y = bottomY; y <= topY; y++)
            {
                Tile tile = tileGrid[x, y];
                if (tile.HasObject() && tile.tileScript.tileType == tileType)
                {
                    tiles.Add(tile);
                }
            }
        }
        return tiles;
    }

    public Tile GetTile(Vector2Int targetTile)
    {
        return tileGrid[targetTile.x, targetTile.y];
    }
}
