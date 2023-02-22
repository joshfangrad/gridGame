using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileTypes
{
    public class Tile
    {
        private GameObject gameObject;
        public TileObject tileScript;
        public Vector2Int position;

        public Tile(Vector2Int pos)
        {
            position = pos;
        }

        public bool AddObjectToTile(GameObject newObject, Vector3 pos, Vector3 scale)
        {
            if (gameObject == null)
            {
                gameObject = GameObject.Instantiate(newObject, pos, Quaternion.identity);
                TileObject _tileScript = gameObject.GetComponent<TileObject>();
                //make non nullable in future please
                if (_tileScript) {
                    tileScript = _tileScript;
                }
                return true;
            }
            return false;
        }

        public bool RemoveObjectFromTile()
        {
            if (gameObject != null)
            {
                GameObject.Destroy(gameObject);
                gameObject = null;
                tileScript = null;
                return true;
            }
            return false;
        }

        //check if a tile has an object in it
        public bool HasObject()
        {
            return gameObject != null;
        }

        public bool IsCrop()
        {
            if (gameObject != null && gameObject.name.Contains("Crop"))
            {
                return true;
            }
            return false;
        }

        public bool IsCropFullyGrown()
        {
            if (IsCrop())
            {
                Crop crop = gameObject.GetComponent<Crop>();
                return crop.IsFullyGrown();
            }
            return false;
        }
    }
}

