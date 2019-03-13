using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    SpawnGrid sg;
    // Start is called before the first frame update
    void Start()
    { 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Hitted(GameObject go)
    {
        Debug.LogFormat("{0} got hit",this.transform.gameObject.name);
        try
        {
            ReleaseGrid();
        }
        finally { Debug.Log("no gs occupied"); }
        GameManager.Instance.Kill(go);
    }

    void ReleaseGrid()
    {
        GameManager.Instance.gridList.Add(sg);
    }

    void OccupyGrid(SpawnGrid sg)
    {
        this.sg = sg;
    }
}
