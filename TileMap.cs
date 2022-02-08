using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    //[SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject hightLightPLayer;
    [SerializeField] private GameObject hightLightOpponentPrefab;
    [SerializeField] private GameObject selectedPrefab;
    public bool hightLightOpponent = false;
    public GridManager gridManager;

    public bool selected = false;

    private void Start()
    {
        
    }

    //need box collider 
    private void OnMouseEnter()
    {
        if (!selected)
        {
            hightLightPLayer.SetActive(true);
        }

        //send info to grid for opponent 
        gridManager.SendOpponentHeightLight(transform.position);
    }

    //private void OnMouseOver()
    //{
    //    hightLight.SetActive(true);
    //}

    private void OnMouseExit()
    {
        hightLightPLayer.SetActive(false);
    }

    private void OnMouseDown()
    {
        //selected = true;

        //selectedPrefab.SetActive(true);
        if (GameManager.playable && GameManager.itIsUrTurn)
        {
            gridManager.SpawnUnitDisc(transform.position);
        }
        
    }

    public void SetOpponentHilight()
    {
        hightLightOpponentPrefab.SetActive(true);
    }

    public void DisableOpponentHilight()
    {
        hightLightOpponentPrefab.SetActive(false);
    }
}
