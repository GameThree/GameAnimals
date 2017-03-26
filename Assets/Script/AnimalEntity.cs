using UnityEngine;
using System.Collections;

public class AnimalEntity : MonoBehaviour
{
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
            animation = gameObject.GetComponent<Animation>();
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
        }
    }

    void OnMouseDrag()
    {
        if (curState == AnimalState.Select)
        {
            Camera camera = GameObject.Find("SceneCamera").GetComponent<Camera>();
            //Debug.LogError("screen " + Input.mousePosition);
            // Debug.LogError(camera.ViewportToScreenPoint(Input.mousePosition));
            //Vector3 pos = camera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.LogError(pos);
            var screenPos = Input.mousePosition + new Vector3(0, 0, -camera.transform.position.y);
            //var screenPos = new Vector3(Input.mousePosition.x, -camera.transform.position.y, Input.mousePosition.y);
            Vector3 pos = camera.ScreenToWorldPoint(screenPos);
            //Debug.LogError("wordpos   " +pos);
            transform.position = new Vector3(-pos.x, 0, -pos.z);
        }
        
    }

    void OnMouseUpAsButton()
    {
       if (!SceneManager.Instance.TrySetToRoad(this, true))
           transform.localPosition =Vector3.zero;
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
            animation.Play(curState.ToString());
        }
        else
        {
            animation.Play(curState.ToString());
        }
    }

    public void Move(float deltaMove)
    {
        transform.localPosition += new Vector3(0, 0, deltaMove);
        moveDistance += deltaMove;
    }

    public bool CanDiffSideConnect(float distance,float bodyLength)
    {
        return (distance + moveDistance + bodyLength / 2 + BodyLength / 2) >= ConstValue.RoadLength;
    }

    public bool CanSameSideConnect(float distance, float bodyLength)
    {
        return (distance - moveDistance - bodyLength / 2 - BodyLength / 2) <= 0;
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
