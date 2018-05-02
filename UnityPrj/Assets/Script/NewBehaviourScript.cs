using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    public Camera camera;

    private void OnMouseDrag()
    {
        if (Camera.main!=null)
        {
            Vector3 mousPos1 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
            Vector3 mousPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z - Camera.main.transform.position.z);
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousPos);
            Vector3 objPosition1 = Camera.main.ScreenToWorldPoint(mousPos1);
            //transform.position = objPosition1;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hitInfo;
            //Physics.Raycast(ray, out hitInfo, 1000, LayerMask.GetMask("Scene"));
            Debug.DrawRay(ray.origin, ray.direction * 1000);
        }
        else
        {
            var ray1 = camera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray1.origin, ray1.direction * 10000);
        }


        //Debug.Log(mousPos + "         " + objPosition + "              " + (objPosition - Camera.main.transform.position) + "   " + objPosition1 + "  " + hitInfo.point);
    }

    // Use this for initialization
    void Start () {
        NewBehaviourScript1.Connect();

       // StartCoroutine(Recive());
    }
	
    void Recive()
    {

    }
	// Update is called once per frame
	void Update ()
    {
        if(NewBehaviourScript1.recives != null&& NewBehaviourScript1.recives.Length>0)
        {
            Debug.Log(NewBehaviourScript1.recives);
            NewBehaviourScript1.recives = null;
        }
    }
}
