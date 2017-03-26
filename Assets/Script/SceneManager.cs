using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour 
{
    public static SceneManager Instance;
    public List<GameObject> AnimalsPrefab=new List<GameObject>();
    public List<RoadEntity> RoadList = new List<RoadEntity>();
    public List<SeatEntity> SeatList = new List<SeatEntity>();
    private List<AnimalEntity> CollectAnimalList = new List<AnimalEntity>();
    public bool NeedCreate = true;
    private bool InCreating = false;
    //public List<AnimalEntity> 
    public bool TrySetToRoad(AnimalEntity entity,bool isTop)
    {
        if (entity == null)
            return false;
        for (int i = 0; i < RoadList.Count; i++)
        {
            if (RoadList[i].TryOnRoad(entity, isTop))
            {
                SeatList[entity.Index].Animal = null;
                entity.SetState(AnimalState.Run);
                NeedCreate = true;
                return true;
            }
        }
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
        
        NeedCreate = false;
        if (InCreating)
            return;
        InCreating = true;
        StartCoroutine(CreateAnimalAnsyc("prefab_daxiang"));
    }

    IEnumerator CreateAnimalAnsyc(string name)
    {       
        yield return new WaitForSeconds(2);
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
                    animal.name = AnimalsPrefab[0].name;
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
