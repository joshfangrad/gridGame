using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMaker : MonoBehaviour
{

    [SerializeField]
    private Grid grid;
    [SerializeField]
    private GameObject whiteTile, blackTile;
    void Start()
    {
        //CreateGridTiles();
    }
    
    public void CreateGridTiles(Vector2Int size, Vector2Int offset)
    {
        //instantiate a grid of tiles on the floor
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                GameObject tileType = (x + z) % 2 == 1 ? blackTile : whiteTile;
                Vector3 position = new Vector3((x + offset.x) * grid.cellSize.x, 0f, (z + offset.y) * grid.cellSize.z);
                GameObject instance = Instantiate(tileType, position, Quaternion.identity, transform);
                //rescale object based on cell size
                instance.transform.localScale = new Vector3(grid.cellSize.x/10, grid.cellSize.y/10, grid.cellSize.z/10);
            }
        }
    }
}
