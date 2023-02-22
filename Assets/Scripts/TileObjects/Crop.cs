using System;
using System.Collections;
using System.Collections.Generic;
using TileTypes;
using UnityEngine;
using TMPro;

public class Crop : TileObject
{
    [SerializeField] private GameObject fill, maxFill;
    [SerializeField] private float growTime = 10f;
    [SerializeField] private float cropWidth = 0.5f;
    [SerializeField] private float cropHeight = 2f;
    [SerializeField] private Material greenMaterial;
    private TMP_Text speedText;

    public override TileType tileType => TileType.Crop;

    private float defaultGrowSpeed = 1f;
    private float growSpeed;

    private float currentGrowth = 0f;
    private bool fullyGrown = false;

    private bool autoHarvest = false;
    private void Awake()
    {
        growSpeed = defaultGrowSpeed;
    }
    private void Start()
    {
        //get a reference to the speedText and set its position
        //speedText = this.GetComponent<TMP_Text>();
        speedText = this.GetComponentInChildren<TMP_Text>();
        speedText.transform.position += new Vector3(0f, cropHeight, 0f);
        UpdateSpeedText();

        //move the maxFill cube to the max height
        maxFill.transform.localPosition = new Vector3(0f, cropHeight + 0.02f, 0f);
        SetGrowthHeight();
    }
    void Update()
    {
        if (!fullyGrown)
        {
            currentGrowth += growSpeed * Time.deltaTime;
        }
        //if done growing, stop 
        if (currentGrowth >= growTime && !fullyGrown) 
        {
            fullyGrown = true;
            currentGrowth = growTime;
            maxFill.GetComponent<MeshRenderer>().material = greenMaterial;


        }
        else
        {
            SetGrowthHeight();
        }

        speedText.transform.rotation = Camera.main.transform.rotation;
    }

    private void SetGrowthHeight()
    {
        float currentHeight = (currentGrowth / growTime) * cropHeight;
        fill.transform.localScale = new Vector3(cropWidth, currentHeight, cropWidth);
        fill.transform.localPosition = new Vector3(0f, currentHeight/2, 0f);
    }

    public bool IsFullyGrown()
    {
        return fullyGrown;
    }

    public void addGrowSpeed(float speed)
    {
        growSpeed += speed;
        UpdateSpeedText();
    }

    public override void Reset()
    {
        growSpeed = defaultGrowSpeed;
        UpdateSpeedText();
    }
    private void UpdateSpeedText()
    {
        if (speedText != null)
        {
            speedText.text = growSpeed.ToString();
        }
    }
}
