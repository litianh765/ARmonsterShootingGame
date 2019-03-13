﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
//using System;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }

    public delegate void CoinMove();
    

    public GameObject textCamPos;

    public GameObject camPosText;

    public GameObject deviceGO;

    public float time;
    public float shotTimer;
    bool shot;

    public GameObject bulletGO;
    public GameObject coin;
    public GameObject monsterGo;
    [SerializeField]
    private SpawngridConfig sconfig;
    //grids where monster can spawn
    public List<SpawnGrid> gridList=new List<SpawnGrid>();


    public GameObject treasureBox;
    public static event CoinMove coinMove;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this.gameObject); }

        var height = sconfig.height;
        var width = sconfig.width;
        var length = sconfig.length;
        var gap = sconfig.gap;
        SpwanGridCreater(height, width, length, gap);
        //foreach (SpawnGrid sg in gridList)
        //{
        //    Instantiate(treasureBox, sg.GetPos(), Quaternion.identity);
        //}
    }

    private void SpwanGridCreater(float height, float width, float length, float gap)
    {
        var countH = (int)(2*height / gap);
        var countW = (int)(2*width / gap);
        var countL = (int)(2*length / gap);
        //Debug.LogFormat("{0}  {1}  {2}",countH,countW,countL);
        height = -height;
        for (int h = 0; h < countH; h++)
        {
            width = -width;
            for (int w = 0; w < countW; w++)
            {
                length = -length;
                for (int l = 0; l < countL; l++)
                {
                    gridList.Add(new SpawnGrid(width,height , length));
                    length += gap;
                }
                width += gap;
            }
            height += gap;
        }
    }

    private void Start()
    {
        time = 0.2f;
        shotTimer = 0f;
        shot = false;

    }
    private void Update()
    {
        Shoot(bulletGO);
        //#if UNITY_STANDALONE_WIN
        //        if (Input.GetButtonDown("Fire1"))
        //        {
        //            shot = true;
        //            //only hit treasure layer
        //            LayerMask layermask = 1<<12|1<<10;
        //            //var touchPos = new Vector2(0.2f,0.2f) ;
        //            Ray ray = new Ray(deviceGO.transform.position, Camera.main.transform.forward);
        //            Debug.DrawRay(deviceGO.transform.position, Camera.main.transform.forward, Color.red, 10f);
        //            textCamPos.GetComponent<Text>().text = "I shoot somthing";
        //            RaycastHit hit;
        //            Debug.Log("touch");
        //            if (Physics.Raycast(ray, out hit,Mathf.Infinity,layermask))
        //            {
        //                //todo
        //                //make some respond in game
        //                //textCamPos.GetComponent<Text>().text = hit.transform.gameObject.layer.ToString();
        //                //if (hit.transform.gameObject.layer == 9)
        //                //{
        //                //    textCamPos.GetComponent<Text>().text = hit.transform.name;
        //                //    transform.GetComponent<Test>().Kill(hit.transform.gameObject, coin);
        //                //}
        //                //else if (hit.transform.gameObject.layer == 10)
        //                //{
        //                //    textCamPos.GetComponent<Text>().text = hit.transform.name;
        //                //    transform.GetComponent<Test>().Kill(hit.transform.gameObject);
        //                //}
        //                if (hit.transform.gameObject.layer == 12)
        //                {
        //                    Instantiate(treasureBox, hit.point, Quaternion.identity);

        //                }
        //                if (hit.transform.gameObject.layer == 10)
        //                {
        //                    Destroy(hit.transform.gameObject);
        //                    TestSpawn();
        //                    camPosText.GetComponent<Text>().text ="Hit treasure";
        //                }
        //            }
        //        }
        //        else if (Input.GetButtonUp("Fire1"))
        //        {
        //            shot = false;
        //        }
        //#endif

        //if (coinMove != null) coinMove();
        coinMove?.Invoke();

#if UNITY_ANDROID
        Touch touch;
        //when there is no touch, no atcion
        if (Input.touchCount==0)shot=false ;
        if (Input.touchCount < 1|| (touch=Input.GetTouch(0)).phase!=TouchPhase.Began)
        {
            return;
        }
        else if (Input.touchCount > 0 && Input.touchCount<2)
        {
            shot = true;
            //layermask:
            //ground is 12
            //treasure is 10
            LayerMask layerMask = 1<<12|1<<10;
            var touchPos = Input.GetTouch(0).position;
            Ray ray = new Ray(deviceGO.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                //todo
                //make some respond in game
                //camPosText.GetComponent<Text>().text = hit.transform.gameObject.layer.ToString();
                if (hit.transform.gameObject.layer == 12)
                {
                    Instantiate(treasureBox, hit.point, Quaternion.identity);
                    camPosText.GetComponent<Text>().text = hit.point.ToString();
                }
                else if(hit.transform.gameObject.layer == 10)
                {
                    Destroy(hit.transform.gameObject);
                    TestSpawn();
                    camPosText.GetComponent<Text>().text = hit.point.ToString();
                }
            }
        }

#endif
        
    }

    private void LateUpdate()
    {
        
    }



    void Shoot(GameObject bulletGO)
    {
        if (shot)
        {
            shotTimer += Time.deltaTime;
        }
        else { shotTimer = 0f; }
        if (shotTimer >= time)
        {
            var bul=Instantiate(bulletGO,deviceGO.transform.position,Quaternion.identity);
            bul.SendMessage("SetDir",deviceGO.transform.forward);
            shotTimer = 0f;
        }
    }

    

    public void Kill(GameObject go)
    {
        Destroy(go);
        SpawnCoin(coin,go/*,coinCollection*/);
    }

    void SpawnCoin(GameObject go, GameObject parent/*,List<GameObject> coins*/)
    {
        
        var coin=Instantiate(go, parent.transform.position, Quaternion.identity);
        //coins.Add(coin);
    }

    public void TestSpawn()
    {
        GameObject deviceGO = GameManager.Instance.deviceGO;
        //get a random pos from gird;
        try
        {
            SpawnGrid sg = GetGrid(gridList);
            Vector3 pos = sg.GetPos();
            Vector3 offsetPos = deviceGO.transform.position;
            //instantiate and set the value of bullet obj
            var spawnGO = Instantiate(monsterGo, pos + offsetPos, Quaternion.identity);
            spawnGO.SendMessage("OccupyGrid", sg);
            //spawnGO.SendMessage("SetDir", GameManager.Instance.deviceGO.transform.forward);
            //Kill(spawnGO,coin);
        }
        catch(System.Exception e) {
            Debug.Log(e.StackTrace);
        }
        finally
        {
            Debug.LogFormat("All slots are full not more place to hold more monster");
        }
    }

    private SpawnGrid GetGrid(List<SpawnGrid> sgList)
    {
        SpawnGrid sg;
        var index = Random.Range(0,sgList.Count);
        sg = sgList[index];
        sgList.RemoveAt(index);
        return sg;
    }
}
