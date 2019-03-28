using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using System.Linq;
using UnityEngine.SceneManagement;
namespace ARMon
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public static GameManager Instance
        {
            get { return instance; }
        }

        public int CurrentScore { get; set; }

        public delegate void Move();


        public GameObject textCamPos;

        public GameObject camPosText;

        public GameObject deviceGO;

        public float time;
        public float shotTimer;
        private bool buttonHit;
        bool shot;

        public GameObject bulletGO;
        public GameObject coin;
        public GameObject monsterGo;
        [SerializeField]
        private SpawngridConfig sconfig;

        public SpawngridConfig Sconfig { get { return sconfig; } }

        public SpawnspotHandler ssHandler = new SpawnspotHandler();

        //detect radius of player where grid spawn
        public float radius;

        public GameObject treasureBox;
        public static event Move MoveHandler;


        //test field
        private Vector3 currentRoundPos = Vector3.zero;
        public Vector3 CurrentRoundPos { get { return currentRoundPos; } }

        private Vector3 previousPos;
                
        public Vector3 PreviousPos
        {
            get { return previousPos; }
        }


        //test ui 
        //public List<GameObject> zeroObjText;
        //public GameObject currentObjText;
        //public GameObject deviceObjText;
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
            radius = sconfig.radius;
            previousPos = Vector3.zero;
            SpwanGridCreater(width, height, length, gap, radius);

            //set currentscore to 0 for test
            CurrentScore = 0;
            //foreach (SpawnGrid sg in ssHandler.Obersvers)
            //{
            //    //sg.InRange = true;
            //    Instantiate(treasureBox, sg.GetPos(), Quaternion.identity);
            //}
            foreach (SpawnGrid sg in ssHandler.Obersvers)
            {
                sg.SignNeighbor(ssHandler.Obersvers, sconfig.gap);
            }

        }

        //create grid
        //witht the raduis, height/width/length is meanless, specially width & length
        private void SpwanGridCreater(float width, float height, float length, float gap, float r)
        {
            var localHeight = -height;
            while (CustomController.Less(localHeight, height))
            {
                var localLength = -length;
                while (CustomController.Less(localLength, length))
                {
                    var localWidth = -width;
                    while (CustomController.Less(localWidth, width))
                    {
                        var targetPos = new Vector3(localWidth, localHeight, localLength);
                        if (CustomController.InRange(deviceGO.transform.position, targetPos, r))
                        {
                            var newGrid = new SpawnGrid(targetPos);
                            ssHandler.Subscribe(newGrid);
                        }
                        localWidth += gap;
                    }
                    localLength += gap;
                }
                localHeight += gap;
            }
        }

        private void Start()
        {
            time = 0.3f;
            shotTimer = 0f;
            shot = false;

        }
        private void Update()
        {
            Shoot(bulletGO);
        }

        
        private void LateUpdate()
        {
            MoveHandler?.Invoke();
            //current scene score
            textCamPos.GetComponent<Text>().text = CurrentScore.ToString();
            //currentRoundPos = CustomController.RoundToClose(deviceGO.transform.position, sconfig.gap);
            //List<string> moveDir = CustomController.PositionCompair(currentRoundPos, deviceGO.transform.position, sconfig.gap);
            List<Direction> moveDir = CustomController.PositionCompair( previousPos,currentRoundPos, sconfig.gap);
            foreach (Direction s in moveDir)
            {
                previousPos = currentRoundPos;
                ObserverUpate(s);
            }
            //text show ui show content update
            //deviceObjText.GetComponent<Text>().text = previousPos.ToString();
            //currentObjText.GetComponent<Text>().text = currentRoundPos.ToString();
            //show sg position code
            //for (int i = 0; i < ssHandler.Obersvers.Count; i++)
            //{
            //    zeroObjText[i].GetComponent<Text>().text = "SG " + (i + 1) + ": " + ((SpawnGrid)ssHandler.Obersvers[i]).X.ToString() + /*"  " + ((SpawnGrid)ssHandler.Obersvers[i]).Y.ToString() + */"  " + ((SpawnGrid)ssHandler.Obersvers[i]).Z.ToString();
            //}
            //zeroObjText.GetComponent<Text>().text = ((SpawnGrid)ssHandler.Obersvers[0]).X.ToString()+"  "+((SpawnGrid)ssHandler.Obersvers[0]).Y.ToString()+"  "+((SpawnGrid)ssHandler.Obersvers[0]).Z.ToString();
        }

        private void FixedUpdate()
        {
            //camPosText.GetComponent<Text>().text = ssHandler.Obersvers.Count.ToString();        
        }

        //this is use for the touch/mouse event
        


        private void Shoot(GameObject bulletGO)
        {
            FireEvent();
            if (shot&&buttonHit)
            {
                shotTimer += Time.deltaTime;
            }
            else { shotTimer = 0f; }
            if (shotTimer >= time)
            {
                var bul = Instantiate(bulletGO, deviceGO.transform.position, Quaternion.identity);
                bul.SendMessage("SetDir", deviceGO.transform.forward);
                shotTimer = 0f;
            }
        }
        private void FireEvent()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                shot = true;
                #region this part mainly for test
                //new mechanic do not relay on spanw andy and hit andy to create monster, so comment this
                //only hit treasure layer
                //LayerMask layermask = 1 << 12 | 1 << 10;
                //var touchPos = new Vector2(0.2f,0.2f) ;
                //Ray ray = new Ray(deviceGO.transform.position, Camera.main.transform.forward);
                Debug.DrawRay(deviceGO.transform.position, Camera.main.transform.forward, Color.red, 10f);
                //textCamPos.GetComponent<Text>().text = "I shoot somthing";
                //RaycastHit hit;
                //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask))
                //{
                //    if (hit.transform.gameObject.layer == 12)
                //    {
                //        Instantiate(treasureBox, hit.point, Quaternion.identity);

                //    }
                //    if (hit.transform.gameObject.layer == 10)
                //    {
                //        Destroy(hit.transform.gameObject);
                //        //camPosText.GetComponent<Text>().text = "Hit treasure";
                //    }
                //} 
                #endregion
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                buttonHit = false;
                shot = false;
            }
            //#endif

            //if (coinMove != null) coinMove();
            

            //#if UNITY_ANDROID
            //            Touch touch;
            //            //when there is no touch, no atcion
            //            //if (Input.touchCount==0)shot=false ;
            //            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            //            {
            //                return;
            //            }
            //            else if (Input.touchCount > 0 && Input.touchCount < 2)
            //            {
            //                shot = true;
            //                //layermask:
            //                //ground is 12
            //                //treasure is 10
            //                LayerMask layerMask = 1 << 12 | 1 << 10;
            //                var touchPos = Input.GetTouch(0).position;
            //                Ray ray = new Ray(deviceGO.transform.position, Camera.main.transform.forward);
            //                RaycastHit hit;
            //                #region touch interaction
            //                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            //                {
            //                    //todo
            //                    //make some respond in game
            //                    //camPosText.GetComponent<Text>().text = hit.transform.gameObject.layer.ToString();
            //                    if (hit.transform.gameObject.layer == 12)
            //                    {
            //                        Instantiate(treasureBox, hit.point, Quaternion.identity);
            //                        //camPosText.GetComponent<Text>().text = hit.point.ToString();
            //                    }
            //                    else if (hit.transform.gameObject.layer == 10)
            //                    {
            //                        Destroy(hit.transform.gameObject);
            //                        TestSpawn();
            //                        //camPosText.GetComponent<Text>().text = hit.point.ToString();
            //                    }
            //                }
            //                #endregion
            //            }

            //#endif
        }
        //this used for the fire button
        public void TriggerFire()
        {
            buttonHit = true;
        }


        //spawn coin when monster is killed;
        //move to CustomController
        //public void SpawnCoin(GameObject spawnGO, GameObject posGO, int val)
        //{
        //    var coin = Instantiate(spawnGO, posGO.transform.position, Quaternion.identity);
        //    coin.transform.GetComponent<Coin>().BounsVal = val;
        //    //coins.Add(coin);
        //}

        public void Summon()
        {
            //GameObject deviceGO = GameManager.Instance.deviceGO;
            //get a random pos from gird;
            try
            {
                SpawnGrid sg = GetGrid(ssHandler.Obersvers);
                Vector3 pos = sg.GetPos();
                Vector3 offsetPos = deviceGO.transform.position;
                //instantiate and set the value of bullet obj
                var spawnGO = Instantiate(monsterGo, pos, Quaternion.identity);
                spawnGO.transform.localScale = new Vector3(1, 1, 1);
                spawnGO.SendMessage("OccupyGrid", sg);
                //spawnGO.SendMessage("SetDir", GameManager.Instance.deviceGO.transform.forward);
                //Kill(spawnGO,coin);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.StackTrace);
            }
        }

        //merge to function, remove this one
        //private SpawnGrid GetGrid(List<IObersver> sgList)
        //{
        //    SpawnGrid sg;
        //    sg = TestRandomPick(sgList);
        //    sg.IsOccupy = true;
        //    return sg;
        //}

        private SpawnGrid GetGrid(List<IObersver> list)
        {
            SpawnGrid output;
            List<IObersver> coll = list.Where(l => !l.IsOccupy).ToList();
            //camPosText.GetComponent<Text>().text = coll.Count.ToString();
            output = (SpawnGrid)coll[Random.Range(0, coll.Count())];
            output.IsOccupy = true;
            return output;
        }

        public void ObserverUpate(Direction dir)
        {
            ssHandler.Notfiy(currentRoundPos, dir, sconfig);
        }

        //public void InsTest(Vector3 pos)
        //{
        //    Instantiate(treasureBox, pos, Quaternion.identity);
        //}

        public void LoadTreeScene()
        {
            CustomController.SetCoin(CurrentScore);
            Debug.Log(CurrentScore);
            Debug.Log(CustomController.GetScore());
            SceneManager.LoadScene("Tree_Avatar");
        }
    }
}