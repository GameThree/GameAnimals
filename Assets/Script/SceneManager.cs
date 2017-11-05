using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour 
{
    public static SceneManager Instance;

    [Header("可行走道路列表")]
    public List<RoadEntity> RoadList = new List<RoadEntity>();
    [Header("座位列表，产生动物放置点")]
    public List<SeatEntity> SeatList = new List<SeatEntity>();
    [Header("玩家动物生产工厂")]
    public AnimalFactory PlayerFactory;
    [Header("AI动物生产工厂")]
    public AnimalFactory EnimyFactory;
    public bool NeedCreate = true;
    public bool EnimyNeedCreate = false;
    private bool InCreating = false;
    //public List<AnimalEntity> 
    public bool TrySetToRoad(AnimalEntity entity,bool isToTop)
    {
        if (entity == null)
            return false;
        for (int i = 0; i < RoadList.Count; i++)
        {
            if (RoadList[i].TryOnRoad(entity.transform.position, isToTop))
            {
                if (isToTop)
                {
                    SeatList[entity.Index].Animal = null;
                    NeedCreate = true;
                }
                RoadList[i].OnRoad(entity, isToTop);
                return true;
            }
        }
        entity.SetState(AnimalState.Wait);
        return false;
    }

    public Vector2 GetStartPos(int index,bool isTop)
    {
        Vector2 start =new Vector2();
        if(isTop)
        {
            start.y = RoadList[index].TopMarkPos;
            
        }
        else
        {
            start.y = RoadList[index].BottomMarkPos;
        }
        start.x = RoadList[index].transform.position.x;
        return start;
    }
    public void CeateAnimal()
    {
        if (InCreating)
            return;
        NeedCreate = false;
        InCreating = true;
        StartCoroutine(AnimalCreator());
    }

    public void SetSelfAnimaToSeat(AnimalEntity entity )
    {
        if (entity == null)
        {
            Debug.LogError("传入空的动物实体");
            return;
        }
        bool find = false;
        for (int i = 0; i < SeatList.Count; i++)
        {
            if (SeatList[i].Animal == null)
            {
                if (find)
                {
                    NeedCreate = true;
                    break;
                }
                else
                {
                    SeatList[i].Animal = entity;
                    entity.gameObject.SetActive(true);
                    entity.SetState(AnimalState.Wait);
                    find = true;
                }
            }
        }
        InCreating = false;
    }

    public void CreateEnemyAnimal()
    {
        var entity = EnimyFactory.CreateAnimals();
        if (entity == null)
        {
            Debug.LogError("生产动物失败");
            return;
        }
        int i = 0;
        while (true)
        {
            i++;
            int roadIndex = Random.Range(0, RoadList.Count);
            float posX = (RoadList[roadIndex].RoadLeft + RoadList[roadIndex].RoadRight) / 2f;
            entity.transform.position = new Vector3(posX, 0, 0);
            if (TrySetToRoad(entity, false))
            {
                break;
            }
            if (i == 1000)
            {
                AnimalFactory.AddToCollectList(entity);
                Debug.LogError("循环了1000次");
                break;
            }
        }
       
    }
    //每隔两秒生产一个
    private IEnumerator AnimalCreator()
    {
        yield return new WaitForSeconds(2);
        var entiy = PlayerFactory.CreateAnimals();
        SetSelfAnimaToSeat(entiy);
        CreateEnemyAnimal();
        
    }
	// Use this for initialization
	void Start () 
    {
        Instance = this;
        for (int i = 0; i < SeatList.Count; i++)
        {
            SeatList[i].SeatIndex = i;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (NeedCreate)
        {
            CeateAnimal();
        }
        if (EnimyNeedCreate)
        {
            EnimyNeedCreate = false;
            CreateEnemyAnimal();
        }
	}

    void OnDestroy()
    {
        Instance = null;
    }
}
