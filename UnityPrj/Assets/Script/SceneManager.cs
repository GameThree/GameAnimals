using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour 
{
    public static SceneManager Instance;

    [Header("可行走道路列表")]
    public List<RoadEntity> RoadList = new List<RoadEntity>();

    public GamePlayer HumanPlayer;
    public GamePlayer ComputerPlayer;

    public bool TrySetToRoad(AnimalEntity entity)
    {
        if (entity == null)
            return false;
        for (int i = 0; i < RoadList.Count; i++)
        {
            if (RoadList[i].TryOnRoad(entity.transform.position, entity.AttackDir))
            {
                if (entity.AttackDir == HumanPlayer.AttackDir)
                {
                    HumanPlayer.UseAnimal(entity, i);
                }
                else
                {
                    ComputerPlayer.UseAnimal(entity, i);
                }
                return true;
            }
        }
        entity.SetState(AnimalState.Wait);
        return false;
    }
	// Use this for initialization
	void Start () 
    {
        Instance = this;
        HumanPlayer.InitData(AttackDirection.Top, RoadList);
        ComputerPlayer.InitData(AttackDirection.Bottom, RoadList);
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < RoadList.Count; i++)
        {
            RoadList[i].UpdateState(Time.deltaTime); 
        }
	}

    void OnDestroy()
    {
        Instance = null;
    }
}
