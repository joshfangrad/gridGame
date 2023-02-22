using System.Collections;
using System.Collections.Generic;
using TileTypes;
using UnityEngine;

public class Replanter : TileObject
{
    public override TileType tileType => TileType.Replanter;

    private GameObject progressBar;
    private ProgressBar progressBarScript;

    [SerializeField] private float defaultProcSpeed = 1f, procTime = 10f;
    private float procSpeed, currentTime;


    public override void Reset() { }

    private void Awake()
    {
        procSpeed = defaultProcSpeed;
    }
    void Start()
    {
        progressBar = transform.Find("ProgressBar").gameObject;
        progressBarScript = progressBar.GetComponent<ProgressBar>();
    }

    void Update()
    {
        if (currentTime >= procTime)
        {
            currentTime = 0f;
            //do proc thing here
        }
        else
        {
            currentTime += Time.deltaTime * procSpeed;
        }

        progressBarScript.SetFillLevel(currentTime / procTime);
    }
}
