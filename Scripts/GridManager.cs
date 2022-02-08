using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GridManager : MonoBehaviour
{
    bool inDebug = true;
    bool inDebugMatrix = true;
    bool inDebugPlayMechanics = true;
    [SerializeField] private int widthGrid, heightGrid;
    [SerializeField] private TileMap tilePrefab;
    [SerializeField] private Transform camTransform;
    //[SerializeField] private UnitDisc unitDiscPrefab;
    [SerializeField] private GameObject unitDiscPrefabPLayer1;
    [SerializeField] private GameObject unitDiscPrefabPLayer2;
    [SerializeField] private GameObject parentFromTile;
    [SerializeField] private GameObject parentFromUnitDiscs;
    List<TileMap> tileMaps = new List<TileMap>();
    List<GameObject> UnitDiscs = new List<GameObject>();
    [SerializeField] GameManager gameManager;
    TileMap OpponentTileActual;

    private (float, TileMap)[,] matrixGrid;

    PhotonView MyPhotonView;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGridAndMatrix();
        //GenerateMatrixGrid();
        PrintMatrixGrid();

        OpponentTileActual = matrixGrid[0, 0].Item2;

        //photon 
        MyPhotonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateGrid()
    {
        for (int x = 0; x < widthGrid; x++)
        {
            for (int y = 0; y < heightGrid; y++)
            {
                TileMap spawnTilePrefab = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                spawnTilePrefab.name = $"Tile {x}, {y}";
            }
        }

        if (inDebug) { Debug.Log("Grid was Generator"); }

        camTransform.position = new Vector3((float)(widthGrid / 2) - 10f, (float)heightGrid / 2 - 10f, -10f);
    }

    void GenerateGridAndMatrix()
    {
        //create matrix
        matrixGrid = new (float, TileMap)[widthGrid, heightGrid];

        for (int x = 0; x < widthGrid; x++)
        {
            for (int y = 0; y < heightGrid; y++)
            {
                TileMap spawnTilePrefab = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                spawnTilePrefab.name = $"Tile {x}, {y}";
                spawnTilePrefab.gridManager = this;
                spawnTilePrefab.transform.SetParent(parentFromTile.transform);
                tileMaps.Add(spawnTilePrefab);

                matrixGrid[x, y] = (0f, spawnTilePrefab);
            }
        }

        if (inDebug) { Debug.Log("Grid and Matrix was created"); }

        //camTransform.position = new Vector3((float)widthGrid / 2 - 4, (float)heightGrid / 2, -10f);
    }

    void GenerateMatrixGrid()
    {
        //The X and the Y are flipped 
        string tempString;

        //create matrix
        matrixGrid = new (float, TileMap)[widthGrid, heightGrid];

        for (int y = 0; y < heightGrid; y++)
        {
            for (int x = 0; x < widthGrid; x++)
            {
                matrixGrid[x, y] = (0f, null);
            }
        }

        if (inDebug) { Debug.Log("Matrix grid was created"); }

        if (inDebugMatrix)
        {
            for (int y = 0; y < heightGrid; y++)
            {
                tempString = "";
                for (int x = 0; x < widthGrid; x++)
                {
                    //Debug.Log("X -> " + x + " Y -> " + y + " Value: " + matrixGrid[x,y]);
                    tempString += matrixGrid[x, y].Item1.ToString();
                    tempString += "\t";
                }
                Debug.Log("X -> " + y + " : " + tempString);
            }
        }
    }

    //Print the matrix 
    public void PrintMatrixGrid()
    {
        //The X and the Y are flipped 
        string tempString;

        if (inDebug) { Debug.Log("Matrix grid was created"); }

        if (inDebugMatrix)
        {
            for (int y = heightGrid - 1; y >= 0; y--)
            {
                tempString = "";
                for (int x = 0; x < widthGrid; x++)
                {
                    //Debug.Log("X -> " + x + " Y -> " + y + " Value: " + matrixGrid[x,y]);
                    tempString += matrixGrid[x, y].Item1.ToString();
                    tempString += "\t";
                }
                Debug.Log("Pos :" + matrixGrid[0, y].Item2.transform.position + " " +
                    "Y -> " + y + " : " + tempString);
            }
        }
    }

    public void UpdateGridMatrix()
    {
        for (int x = 0; x < widthGrid; x++)
        {
            for (int y = 0; y < heightGrid; y++)
            {
                if (matrixGrid[x, y].Item2.selected)
                {
                    matrixGrid[x, y].Item1 = 1;
                }
            }
        }
    }

    //Spawn the Unit disc from the player that is playing and start his movement and update matrix 
    public void SpawnUnitDisc(Vector3 trigerPos)
    {
        //CAll RPC
        MyPhotonView.RPC("RPCSpawnUnitDisc", RpcTarget.AllBuffered, trigerPos);

        //if it is in menu game u can't play
        //if (!gameManager.InMenuGame)
        //{
        //    Vector3 presentPos;
        //    Vector3 nextPos;

        //    //x is the columm 
        //    for (int y = heightGrid - 1; y >= 0; y--)
        //    {
        //        //check if columm is full
        //        if ((y == heightGrid - 1) && matrixGrid[(int)trigerPos.x, y].Item1 != 0)
        //        {
        //            if (inDebugPlayMechanics)
        //            {
        //                Debug.Log("the columm " + trigerPos.x + " is full");
        //            }

        //            //not possible no play

        //            break;
        //        }

        //        //is empty
        //        if ((y == 0) && matrixGrid[(int)trigerPos.x, y].Item1 == 0)
        //        {
        //            if (inDebugPlayMechanics)
        //            {
        //                Debug.Log("the columm " + trigerPos.x + " is empty");
        //            }

        //            //init spawner
        //            //UnitDisc spawnUnitDiscPrefab = Instantiate(unitDiscPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //            GameObject spawnUnitDiscPrefab = Instantiate(GetUnitDiscByPlayer(), new Vector3(0, 0, 0), unitDiscPrefabPLayer1.transform.rotation);

        //            //add parent
        //            spawnUnitDiscPrefab.transform.SetParent(parentFromUnitDiscs.transform);

        //            //Get code from unit disc
        //            UnitDisc tempUnitDisc = spawnUnitDiscPrefab.GetComponent<UnitDisc>();

        //            //Add to list
        //            UnitDiscs.Add(spawnUnitDiscPrefab);

        //            //start the movement in disc code
        //            tempUnitDisc.StartMovement(
        //                matrixGrid[(int)trigerPos.x, heightGrid - 1].Item2.transform.position,
        //                matrixGrid[(int)trigerPos.x, 0].Item2.transform.position);

        //            //update matrix 
        //            matrixGrid[(int)trigerPos.x, 0].Item1 = GetUnitDiscByPlayerGridId();
        //            matrixGrid[(int)trigerPos.x, 0].Item2.selected = true;

        //            //Dont let the other player play 
        //            GameManager.playable = false;

        //            //Finish turn
        //            //EndTurn();

        //            break;
        //        }

        //        presentPos = matrixGrid[(int)trigerPos.x, y].Item2.transform.position;
        //        nextPos = matrixGrid[(int)trigerPos.x, y - 1].Item2.transform.position;

        //        if (matrixGrid[(int)trigerPos.x, y - 1].Item1 != 0)
        //        {
        //            //init spawner 
        //            GameObject spawnUnitDiscPrefab = Instantiate(GetUnitDiscByPlayer(), new Vector3(0, 0, 0), unitDiscPrefabPLayer1.transform.rotation);

        //            //add parent
        //            spawnUnitDiscPrefab.transform.SetParent(parentFromUnitDiscs.transform);

        //            //Get code from unit disc 
        //            UnitDisc tempUnitDisc = spawnUnitDiscPrefab.GetComponent<UnitDisc>();

        //            //Add to list
        //            UnitDiscs.Add(spawnUnitDiscPrefab);

        //            //start the movement in disc code
        //            tempUnitDisc.StartMovement(
        //                matrixGrid[(int)trigerPos.x, heightGrid - 1].Item2.transform.position,
        //                matrixGrid[(int)trigerPos.x, y].Item2.transform.position);

        //            //update matrix 
        //            matrixGrid[(int)trigerPos.x, y].Item1 = GetUnitDiscByPlayerGridId();
        //            matrixGrid[(int)trigerPos.x, y].Item2.selected = true;

        //            //Dont let the other player play 
        //            GameManager.playable = false;

        //            //Finish turn
        //            //EndTurn();

        //            break;
        //        }

        //    }
        //}

    }

    void EndTurn()
    {
        gameManager.EndTurn();
    }

    //get the prefab from the player that is playing 
    GameObject GetUnitDiscByPlayer()
    {
        if (GameManager.player1Turn)
        {
            return unitDiscPrefabPLayer1;
        }
        else if (GameManager.player2Turn)
        {
            return unitDiscPrefabPLayer2;
        }
        else
        {
            Debug.Log("ERROR Player1 -> " + GameManager.player1Turn + " PLayer2 -> " + GameManager.player2Turn);
            return null;
        }
    }

    //Get grid Id from the player that is playing
    float GetUnitDiscByPlayerGridId()
    {
        if (GameManager.player1Turn)
        {
            return 1;
        }
        else if (GameManager.player2Turn)
        {
            return 2;
        }
        else
        {
            Debug.Log("ERROR Player1 -> " + GameManager.player1Turn + " PLayer2 -> " + GameManager.player2Turn);
            return 0;
        }
    }

    //Verify if there is a winner
    public bool VerifyVictoryCondiction()
    {
        float playerGridID = GetUnitDiscByPlayerGridId();

        // horizontalCheck 
        for (int j = 0; j < heightGrid - 3; j++)
        {
            for (int i = 0; i < widthGrid; i++)
            {
                if (matrixGrid[i, j].Item1 == playerGridID && matrixGrid[i, j + 1].Item1 == playerGridID && matrixGrid[i, j + 2].Item1 == playerGridID && matrixGrid[i, j + 3].Item1 == playerGridID)
                {
                    return true;
                }
            }
        }
        // verticalCheck
        for (int i = 0; i < widthGrid - 3; i++)
        {
            for (int j = 0; j < heightGrid; j++)
            {
                if (matrixGrid[i, j].Item1 == playerGridID && matrixGrid[i + 1, j].Item1 == playerGridID && matrixGrid[i + 2, j].Item1 == playerGridID && matrixGrid[i + 3, j].Item1 == playerGridID)
                {
                    return true;
                }
            }
        }
        // ascendingDiagonalCheck 
        for (int i = 3; i < widthGrid; i++)
        {
            for (int j = 0; j < heightGrid - 3; j++)
            {
                if (matrixGrid[i, j].Item1 == playerGridID && matrixGrid[i - 1, j + 1].Item1 == playerGridID && matrixGrid[i - 2, j + 2].Item1 == playerGridID && matrixGrid[i - 3, j + 3].Item1 == playerGridID)
                    return true;
            }
        }
        // descendingDiagonalCheck
        for (int i = 3; i < widthGrid; i++)
        {
            for (int j = 3; j < heightGrid; j++)
            {
                if (matrixGrid[i, j].Item1 == playerGridID && matrixGrid[i - 1, j - 1].Item1 == playerGridID && matrixGrid[i - 2, j - 2].Item1 == playerGridID && matrixGrid[i - 3, j - 3].Item1 == playerGridID)
                    return true;
            }
        }

        return false;
    }

    //Restart Game
    public void RestartTheGame()
    {
        if (inDebug) { Debug.Log("Restart Game"); }

        ClearMatrix();
        ClearTiles();
        ClearUnitsDiscs();
    }

    void ClearMatrix()
    {
        for (int x = 0; x < widthGrid; x++)
        {
            for (int y = 0; y < heightGrid; y++)
            {
                matrixGrid[x, y].Item1 = 0f;
            }
        }

        if (inDebug) { Debug.Log("Matrix Clear"); }
    }

    void ClearTiles()
    {
        foreach (TileMap tile in tileMaps)
        {
            tile.selected = false;
        }

        if (inDebug) { Debug.Log("Grid Clear"); }
    }

    void ClearUnitsDiscs()
    {
        for (int i = UnitDiscs.Count - 1; i >= 0; i--)
        {
            Destroy(UnitDiscs[i]);
        }

        if (inDebug) { Debug.Log("Unit Discs Clear"); }
    }

    //Photon
    [PunRPC]
    void RPCSpawnUnitDisc(Vector3 trigerPos)
    {
        //if it is in menu game u can't play
        if (!gameManager.InMenuGame)
        {
            Vector3 presentPos;
            Vector3 nextPos;

            //x is the columm 
            for (int y = heightGrid - 1; y >= 0; y--)
            {
                //check if columm is full
                if ((y == heightGrid - 1) && matrixGrid[(int)trigerPos.x, y].Item1 != 0)
                {
                    if (inDebugPlayMechanics)
                    {
                        Debug.Log("the columm " + trigerPos.x + " is full");
                    }

                    //not possible no play

                    break;
                }

                //is empty
                if ((y == 0) && matrixGrid[(int)trigerPos.x, y].Item1 == 0)
                {
                    if (inDebugPlayMechanics)
                    {
                        Debug.Log("the columm " + trigerPos.x + " is empty");
                    }

                    //init spawner
                    //UnitDisc spawnUnitDiscPrefab = Instantiate(unitDiscPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    GameObject spawnUnitDiscPrefab = Instantiate(GetUnitDiscByPlayer(), new Vector3(0, 0, 0), unitDiscPrefabPLayer1.transform.rotation);

                    //add parent
                    spawnUnitDiscPrefab.transform.SetParent(parentFromUnitDiscs.transform);

                    //Get code from unit disc
                    UnitDisc tempUnitDisc = spawnUnitDiscPrefab.GetComponent<UnitDisc>();

                    //Add to list
                    UnitDiscs.Add(spawnUnitDiscPrefab);

                    //start the movement in disc code
                    tempUnitDisc.StartMovement(
                        matrixGrid[(int)trigerPos.x, heightGrid - 1].Item2.transform.position,
                        matrixGrid[(int)trigerPos.x, 0].Item2.transform.position);

                    //update matrix 
                    matrixGrid[(int)trigerPos.x, 0].Item1 = GetUnitDiscByPlayerGridId();
                    matrixGrid[(int)trigerPos.x, 0].Item2.selected = true;

                    //Dont let the other player play 
                    GameManager.playable = false;

                    //Finish turn
                    //EndTurn();

                    break;
                }

                presentPos = matrixGrid[(int)trigerPos.x, y].Item2.transform.position;
                nextPos = matrixGrid[(int)trigerPos.x, y - 1].Item2.transform.position;

                if (matrixGrid[(int)trigerPos.x, y - 1].Item1 != 0)
                {
                    //init spawner 
                    GameObject spawnUnitDiscPrefab = Instantiate(GetUnitDiscByPlayer(), new Vector3(0, 0, 0), unitDiscPrefabPLayer1.transform.rotation);

                    //add parent
                    spawnUnitDiscPrefab.transform.SetParent(parentFromUnitDiscs.transform);

                    //Get code from unit disc 
                    UnitDisc tempUnitDisc = spawnUnitDiscPrefab.GetComponent<UnitDisc>();

                    //Add to list
                    UnitDiscs.Add(spawnUnitDiscPrefab);

                    //start the movement in disc code
                    tempUnitDisc.StartMovement(
                        matrixGrid[(int)trigerPos.x, heightGrid - 1].Item2.transform.position,
                        matrixGrid[(int)trigerPos.x, y].Item2.transform.position);

                    //update matrix 
                    matrixGrid[(int)trigerPos.x, y].Item1 = GetUnitDiscByPlayerGridId();
                    matrixGrid[(int)trigerPos.x, y].Item2.selected = true;

                    //Dont let the other player play 
                    GameManager.playable = false;

                    //Finish turn
                    //EndTurn();

                    break;
                }

            }
        }
    }

    [PunRPC]
    void RPCSendOpponentHeightLight(int indexInOpponentX, int indexInOpponentY)
    {
        //desable actual
        OpponentTileActual.DisableOpponentHilight();

        OpponentTileActual = matrixGrid[indexInOpponentX, indexInOpponentY].Item2;

        OpponentTileActual.SetOpponentHilight();

        if (inDebugMatrix) Debug.Log("Opponent hillight  at x -> " + indexInOpponentX + " y-> " + indexInOpponentY);
    }

    public void SendOpponentHeightLight(Vector3 pos)
    {
        //search
        for (int x = 0; x < widthGrid; x++)
        {
            for (int y = 0; y < heightGrid; y++)
            {
                if (matrixGrid[x, y].Item2.transform.position == pos)
                {
                    if (inDebugMatrix) Debug.Log("Send info for opponent hillight x -> " + x + " y-> " + y);

                    //CAll RPC
                    MyPhotonView.RPC("RPCSendOpponentHeightLight", RpcTarget.OthersBuffered, x, y);
                }
               
            }
        }
    }
}
