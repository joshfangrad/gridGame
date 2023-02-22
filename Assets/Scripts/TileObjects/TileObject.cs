using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileTypes;

public abstract class TileObject : MonoBehaviour
{
    public abstract TileType tileType { get; }
    public abstract void Reset();
}
