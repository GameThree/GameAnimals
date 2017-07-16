using UnityEngine;
using System.Collections;

public class AnimalEntity : MonoBehaviour
{
    [Header("需要给每个动物加碰撞")]
    [Header("挂上动物的碰撞组件")]
    public Animation animation;
    [Header("力量")]
    public float defaultPower;
    [Header("身体长度")]
    public float bodyLength;
    //所在Seat位置，或者所在路的位置
    public int Index;

    public bool RunToTop =true;
    private AnimalState curState = AnimalState.Wait;
    
    private float moveDistance = 0;

    private float curPower;
    

    void Start()
    {
        curPower = defaultPower;
        if (animation == null)
        {
            animation = gameObject.GetComponentInChildren<Animation>();
            if (animation == null)
            {
                animation = gameObject.AddComponent<Animation>();
            }
        }
    }
    void OnMouseDown()
    {
        if (curState == AnimalState.Wait)
        {
            SetState(AnimalState.Select);
            StartCoroutine(OnDrag());
        }
    }

    IEnumerator OnDrag()
    {
        if (curState == AnimalState.Select)
        {
            Camera camera = GameObject.Find("SceneCamera").GetComponent<Camera>();
            var offset = transform.position - camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            var wait =new WaitForFixedUpdate();
            while (AnimalState.Select == curState)
            {
                Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                Vector3 curPosition = camera.ScreenToWorldPoint(curScreenSpace);
                Debug.Log(Input.mousePosition + "    " + curPosition);
                transform.position = curPosition + offset;
                yield return wait;
            }
            
        }
        
    }

    void OnMouseUp()
    {
        if (curState == AnimalState.Select)
        {
            if (SceneManager.Instance&&!SceneManager.Instance.TrySetToRoad(this, true))
                transform.localPosition = Vector3.zero;
        }
       
    }

    public void SetState(AnimalState state)
    {
        if (curState == state)
            return;
        curState = state;
        if (curState == AnimalState.Finish)
        {
            moveDistance = 0;
            Index = -1;
        }
        else if (curState == AnimalState.Connect)
        {

        }
        else if (curState == AnimalState.Wait)
        {
 
        }
        else if (curState == AnimalState.Run)
        {
            Vector2 start = SceneManager.Instance.GetStartPos(Index, !RunToTop);
            transform.position = new Vector3(start.x, 0, start.y);
            animation.Play(gameObject.name + "_" + curState.ToString());
        }
        else if (curState == AnimalState.Select)
        {
            transform.position =transform.position + new Vector3(0, 0.5f, 0);
            animation.Play(gameObject.name+"_"+curState.ToString());
        }
        else
        {
            animation.Play(curState.ToString());
        }
    }

    public void Move(float deltaMove)
    {
        transform.localPosition += new Vector3(0, 0, RunToTop ? deltaMove : -deltaMove);
        moveDistance += deltaMove;
        Debug.Log(moveDistance + "      " + gameObject.name);
    }

    public void ToSeatDefault()
    {
        transform.localPosition = Vector3.zero;
    }

    public float CurPower
    {
        get { return curPower; }
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
}
