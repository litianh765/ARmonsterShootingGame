using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour,IMoveObj
{

    private Vector3 dir;
    private float speed;

    public float Speed { get => speed; set => speed=value; }

    // Start is called before the first frame update
    void Start()
    {
        speed = 2f;
        Invoke("SelfDestory",3f);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void OnTriggerEnter(Collider col)
    {
        col.transform.SendMessage("Hitted",col.gameObject);
    }

    public void SetDir(Vector3 dir)
    {
        this.dir = dir;
    }

    public void Movement()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
    }

    //public void Hit(Collider col) 
    //{
    //    //GameManager.Instance.textCamPos.GetComponent<Text>().text = "I shoot somthing";
    //    //transform.GetComponent<Test>().Kill(col.transform.gameObject);
    //}

    void SelfDestory()
    {
        Destroy(this.gameObject);
    }
}
