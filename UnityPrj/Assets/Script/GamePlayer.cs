using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class PlayerBase : MonoBehaviour
{

    /// <summary>
    /// 获取AI投放决策数据,数组代表每一路
    /// </summary>
    /// <returns></returns>
    public abstract AIBattleInfo[] GetAIBattleInfo();
    /// <summary>
    /// 获取AI创建决策数据
    /// </summary>
    /// <returns></returns>
    public abstract bool CanCreateAnimal();
    /// <summary>
    /// AI决策结果执行
    /// </summary>
    /// <param name="index"></param>
    /// <param name="opr"></param>
    public abstract void ExcuteOpr(int index, float needPower);
    /// <summary>
    /// 执行决策创建动物
    /// </summary>
    public abstract void CreateAnimal();
}
public class GamePlayer : PlayerBase
{
    [Header(" 可拥有的闲置动物最大数量")]
    public int AnimalMaxCount = 5;
    [Header("动物生产工厂")]
    public AnimalFactory Factory;
    [Header("使用AI")]
    public BattleAI UseAI;
    [Header("座位列表，产生动物放置点")]
    public List<SeatEntity> SeatList = new List<SeatEntity>();
    [Header("以下为战斗数据不做配置")]
    public AIBattleInfo[] BattleDatas;
    public List<AnimalEntity> WaitAnimals =new List<AnimalEntity>();
    public AttackDirection AttackDir;   
    private List<RoadEntity> m_roads = new List<RoadEntity>();

    public void InitData(AttackDirection dir,List<RoadEntity> roads)
    {
        AttackDir = dir;
        m_roads = roads;
        UseAI.StartAI = true;
        BattleDatas = new AIBattleInfo[roads.Count];
        for (int i = 0; i < roads.Count; i++)
        {
            BattleDatas[i] = new AIBattleInfo();
        }
        for (int i = 0; i < SeatList.Count; i++)
        {
            SeatList[i].SeatIndex = i;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void UpdateState () {
		
	}

    public override AIBattleInfo[] GetAIBattleInfo()
    {
        for (int i = 0; i < BattleDatas.Length; i++)
        {
            var otherDir = AttackDir== AttackDirection.Top?AttackDirection.Bottom:AttackDirection.Top;
            BattleDatas[i].OtherMaxMoveDis = m_roads[i].GetMaxDis(otherDir);
            BattleDatas[i].RoadOtherAllPower = m_roads[i].GetTotalPower(otherDir);
            BattleDatas[i].RoadSelfAllPower = m_roads[i].GetTotalPower(AttackDir);
            //BattleDatas[i].GiveUpConnectDis = m_roads[i].GetLastConnectMoveDis(AttackDir);
        }
        return BattleDatas;
    }

    public override bool CanCreateAnimal()
    {
        return WaitAnimals.Count < AnimalMaxCount;
    }

    public override void ExcuteOpr(int index, float needPower)
    {
        if(WaitAnimals.Count>0)
        {
            int idx = -1;
            float deltaPower = -1;
            for(int i =0;i<WaitAnimals.Count;i++)
            {
                float deltaPower1 = WaitAnimals[i].Power - needPower;
                if(idx <0)
                {
                    idx = i;
                    deltaPower = deltaPower1;
                }
                else if(deltaPower<0)
                {
                    if (deltaPower1>deltaPower)
                    {
                        idx = i;
                        deltaPower = deltaPower1;
                    }
                }
                else if(deltaPower1>=0&&deltaPower>deltaPower1)
                {
                     idx = i;
                     deltaPower = deltaPower1;
                }
            }

            var animal = WaitAnimals[idx];
            animal.gameObject.SetActive(true);
            WaitAnimals.RemoveAt(idx);
            m_roads[index].OnRoad(animal);
            UpdateSeatInfo();
        }
    }

    public void UseAnimal(AnimalEntity entity,int index)
    {
        for (int i = 0; i < WaitAnimals.Count; i++)
        {
            if (WaitAnimals[i] == entity)
            {
                WaitAnimals.RemoveAt(i);
                break;
            }
        }
        m_roads[index].OnRoad(entity);
        UpdateSeatInfo();
    }
    private void UpdateSeatInfo()
    {
        if(SeatList!= null)
        {
            for (int i = 0; i < SeatList.Count; i++)
            {
                if (i < WaitAnimals.Count)
                {
                    WaitAnimals[i].gameObject.SetActive(true);
                    SeatList[i].Animal = WaitAnimals[i];
                }
            }
        }
     
    }
    public override void CreateAnimal()
    {
        var animal = Factory.CreateAnimals();
        animal.AttackDir = AttackDir;
        animal.gameObject.transform.parent = gameObject.transform;
        animal.gameObject.SetActive(false);
        WaitAnimals.Add(animal);
        UpdateSeatInfo();
    }
}
