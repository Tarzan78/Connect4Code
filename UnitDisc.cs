using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDisc : MonoBehaviour
{
    
    private Vector3 startPoint;
    private Vector3 endPoint;

    bool startMove;
    [SerializeField] float speed;
    [SerializeField] float zOffSet;

    GameManager gameManger;

    // Start is called before the first frame update
    void Start()
    {
        gameManger = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startMove)
        {
            MoveUnitDisc();
        }
    }

    //Init Movement
    public void StartMovement(Vector3 initPos, Vector3 endPos)
    {
        startPoint = initPos;
        startPoint += new Vector3(0, 0, zOffSet);
        endPoint = endPos;
        endPoint += new Vector3(0, 0, zOffSet);
        startMove = true;
        transform.position = startPoint;
    }
    //Criar prefab e por a andar 

    //Move 
    void MoveUnitDisc()
    {
        float step = speed * Time.deltaTime;//speed
        transform.position = Vector3.MoveTowards(transform.position, endPoint, step);

        if (transform.position == endPoint ||
            Vector3.Distance(transform.position, endPoint) < 0.01f)
        {
            startMove = false;

            EndTurn();
        }
    }

    void EndTurn()
    {
        //if (GameManager.itIsUrTurn)
        //{
        //
        //}
        gameManger.EndTurn();
    }
}