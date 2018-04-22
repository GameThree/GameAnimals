using UnityEngine;
using System.Collections;

public class AnimalEntity : MonoBehaviour
{
    private static Camera m_sceneCamera;
    [Header("需要给每个动物加碰撞")]
    [Header("挂上动物的碰撞组件")]
    public Animation animation;
    [Header("力量")]
    public float power;
    [Header("力量小")]
    public float minPower;
    [Header("力量大")]
    public float maxPower;
    [Header("身体长度")]
    public float bodyLength;
    [Header("动物的默认速度")]
    public float AnimalDefaultSpeed;
    [Header("完成增加的分数")]
    public int FinshPoint = 0;
    //所在Seat位置，或者所在路的位置
    public int Index;
    public AttackDirection AttackDir = AttackDirection.Top;
    public AnimalState curState = AnimalState.Wait;
    
    public float moveDistance = 0;
    

    void Start()
    {
        if (animation == null)
        {
            animation = gameObject.GetComponentInChildren<Animation>();
            if (animation == null)
            {
                animation = gameObject.AddComponent<Animation>();
            }
        }
    }

    private void OnMouseDrag()
    {

        if (curState == AnimalState.Select)
        {
            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hitInfo;
            //Physics.Raycast(ray, out hitInfo, 1000, LayerMask.GetMask("Scene"));
            //transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, 0) ;
            Vector3 mousPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y,  - m_sceneCamera.transform.position.z);
            Vector3 objPosition = m_sceneCamera.ScreenToWorldPoint(mousPos);
            transform.position = objPosition;
        }
    }

    void OnMouseDown()
    {
        if (m_sceneCamera == null)
        {
            m_sceneCamera = GameObject.Find("SceneCamera").GetComponent<Camera>();
        }
        if (curState == AnimalState.Wait)
        {
            SetState(AnimalState.Select);
        }
    }



    void OnMouseUp()
    {
        if (curState == AnimalState.Select)
        {
            if (SceneManager.Instance && !SceneManager.Instance.TrySetToRoad(this))
                transform.localPosition = Vector3.zero;
        }
       
    }
    public void StartRun()
    {
        SetState(AnimalState.Run);
    }
    public void SetState(AnimalState state)
    {
        if (curState == state)
            return;
        curState = state;
        if (curState == AnimalState.Finish || curState == AnimalState.Dead)
        {
            moveDistance = 0;
            Index = -1;
        }
        else if (curState == AnimalState.Connect)
        {

        }
        else if (curState == AnimalState.Wait)
        {
            ToSeatDefault();
        }
        else if (curState == AnimalState.Select)
        {
            animation.Play(gameObject.name+"_"+curState.ToString());
        }
        else
        {
            animation.Play(gameObject.name + "_" + curState.ToString());
        }
    }
    public void Move(float deltaMove)
    {
        transform.localPosition += new Vector3(0,  AttackDir == AttackDirection.Top ? deltaMove : -deltaMove,0);
        moveDistance += deltaMove;
    }
    public void OnToRoad(float xPos,float zPos) 
    {
       gameObject.SetActive(true);
       transform.parent = null;
       power = Random.Range(minPower, maxPower);
       transform.rotation =AttackDir == AttackDirection.Top? Quaternion.Euler(new Vector3(0f, 0f, 0f)): Quaternion.Euler(new Vector3(0f, 0f, 180f));
       transform.localPosition = new Vector3(xPos, zPos, 0);

       StartRun();
    }
    public void ToSeatDefault()
    {
        gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        moveDistance = 0f;
    }

    public float Power
    {
        get { return power; }
    }

    public float BodyLength
    {
        get { return bodyLength; }
    }

    public float MoveDistance
    {
        get { return moveDistance; }
    }

    public AnimalState CurState
    {
        get { return curState; }
    }

    public void Clear()
    {
        Index = -1;
        AttackDir = AttackDirection.Top;
        curState = AnimalState.None;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    }
}
