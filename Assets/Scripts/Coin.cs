using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour,IMoveObj
{
    private float speed;
    public float Speed { get =>speed; set => throw new System.NotImplementedException(); }
    public int BounsVal { get; set; }
    //target is deivece
    private GameManager gm;

    void Awake()
    {
        speed = 2f;

        //test bounsval set
        BounsVal = 10;
    }
    public void Movement()
    {
        transform.position = Vector3.MoveTowards(transform.position,gm.deviceGO.transform.position,speed*Time.deltaTime);
        Debug.Log("move");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == gm.deviceGO.transform.position)
        {
            gm.CurrentScore+=BounsVal;
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        gm = GameManager.Instance;
        GameManager.coinMove += this.Movement;
        Debug.Log("enabled");
    }

    private void OnDisable()
    {
        GameManager.coinMove -= Movement;
    }
}
