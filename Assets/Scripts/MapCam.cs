using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCam : MonoBehaviour,IMoveObj
{
    private float speed = Mathf.Infinity;
    public float Speed { get => speed; set=>speed=value; }
    GameObject targetGo;
    float zero;
    public void Movement()
    {
        Vector3 targetPos = targetGo.transform.position;
        float offsetY = transform.position.y;
        targetPos.y = offsetY;
        transform.position = Vector3.MoveTowards(transform.position,targetPos,speed*Time.deltaTime);
        transform.rotation = Quaternion.Euler(90f,targetGo.transform.eulerAngles.y,0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        targetGo = GameManager.Instance.deviceGO;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
}
