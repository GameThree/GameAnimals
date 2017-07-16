using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour 
{
    public static SceneManager Instance;
    [Header("动物资源列表")]
    public List<GameObject> AnimalsPrefab=new List<GameObject>();
    [Header("可行走道路列表")]
    public List<RoadEntity> RoadList = new List<RoadEntity>();
    [Header("座位列表，产生动物放置点")]
    public List<SeatEntity> SeatList = new List<SeatEntity>();
    private List<AnimalEntity> CollectAnimalList = new List<AnimalEntity>();
    public bool NeedCreate = true;
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
        entity.ToSeatDefault();
        return false;
    }
    public void AddToCollectList(AnimalEntity entity)
    {
        entity.gameObject.SetActive(false);
        entity.Index = -1;
        entity.transform.parent = null;
        CollectAnimalList.Add(entity);
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
        int index = Random.Range(0, AnimalsPrefab.Count);
        string name = AnimalsPrefab[index].name;
        StartCoroutine(AnimalFactory("daxiang"));
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
        int index = Random.Range(0, AnimalsPrefab.Count);
        var entity = CreateAnimalEntity(AnimalsPrefab[index].name);
        if (entity == null)
        {
            Debug.LogError("生产动物失败" + AnimalsPrefab[index].name);
            return;
        }
        int roadIndex = Random.Range(0, RoadList.Count);
        entity.transform.position = RoadList[roadIndex].transform.position;
        TrySetToRoad(entity, false);
    }
    //每隔两秒生产一个
    private IEnumerator AnimalFactory(string name )
    {
        yield return new WaitForSeconds(2);
        var entiy = CreateAnimalEntity(name);
        SetSelfAnimaToSeat(entiy);
        CreateEnemyAnimal();
        
    }
    private AnimalEntity CreateAnimalEntity(string name)
    {
        AnimalEntity entity=null;
        for(int i=0;i<CollectAnimalList.Count;i++)
        {
            if (CollectAnimalList[i].name.Equals(name))
            {
                entity = CollectAnimalList[i];
                CollectAnimalList.RemoveAt(i);
                break;
            }
        }
        if (entity == null)
        {
            for (int i = 0; i < AnimalsPrefab.Count; i++)
            {
                if (AnimalsPrefab[i].name.Equals(name))
                {
                    GameObject animal = Object.Instantiate(AnimalsPrefab[i]) as GameObject;
                    animal.name = AnimalsPrefab[i].name;
                    entity = animal.GetComponent<AnimalEntity>();
                    break;
                }
            }
        }
        if (entity == null)
        {
           GameObject animalPrefab = Resources.Load<GameObject>("animals/" + name);
           if (animalPrefab != null)
           {
               AnimalsPrefab.Add(animalPrefab);
               GameObject animal = Object.Instantiate(animalPrefab) as GameObject;
               animal.name = animalPrefab.name;
               entity = animal.GetComponent<AnimalEntity>();
           }
           else
           {
               Debug.LogError("找不到 ：" + name);
           }
        }
        return entity;
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
	}

    void OnDestroy()
    {
        Instance = null;
    }
}
