using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAI : MonoBehaviour {
    [SerializeField]
    public PlayerBase AnimalChecker;
    [Header("创建动物间隔")]
    public float CreateAnimalDelta = 2;
    [Header("是否开启自动战斗")]
    public bool AutoFight = false;

    [Header("开启防守距离")]
    public float DefendDis = 0;
    [Header("重点防守距离")]
    public float ImportantDefendDis = 0;
    [Header("放弃防守距离")]
    public float GiveUpDis = 0;
    [Header("每隔多少秒做一次决策")]
    public float DecisionTime = 0.2f;
    [Header("首次决策延迟时间")]
    public float FirstDecision = 2f;
    public bool StartAI = false;
    private float m_countTime = -1f;
    private bool m_isFirst = false;
    private int m_excuteIndex = -1;
    private ExcuteOrder m_excuteOpr =0;
    private List<int> m_randomList = new List<int>();
    private List<int> m_attackList = new List<int>();
    private float m_createFinishTime = 0;
	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update()
    {
        if (!StartAI)
            return;
        if (m_isFirst)
        {
            m_countTime = Time.time + FirstDecision;
            m_isFirst = true;
        }
        if (Time.time > m_countTime)
        {
            m_countTime = Time.time + DecisionTime;
            if (AutoFight)
            CheckBattleState();
        }
        if (m_createFinishTime <= 0)
        {
            m_createFinishTime = Time.time +CreateAnimalDelta;
        }
        if (Time.time >= m_createFinishTime)
        {
            m_createFinishTime = Time.time + CreateAnimalDelta;
            if (AnimalChecker&&AnimalChecker.CanCreateAnimal())
            {
                AnimalChecker.CreateAnimal();
            }
        }
	}

    public void CheckBattleState()
    {
        m_excuteIndex = -1;
        m_excuteOpr =0;
        m_randomList.Clear();
        m_attackList.Clear();
        var battleRoads = AnimalChecker.GetAIBattleInfo();

        for (int i = 0; i < battleRoads.Length; i++)
        {
            if (battleRoads[i].OtherMaxMoveDis >= GiveUpDis /**|| !battleRoads[i].IsLastBodyMoveOut**/)
            {
                continue;
            }
            else if (battleRoads[i].OtherMaxMoveDis >= ImportantDefendDis &&battleRoads[i].RoadOtherAllPower>=battleRoads[i].RoadSelfAllPower)
            {
                SetExcuteOpr(i, ExcuteOrder.ImportDefend);
            }
            else if (battleRoads[i].OtherMaxMoveDis >= DefendDis && battleRoads[i].RoadOtherAllPower >= battleRoads[i].RoadSelfAllPower)
            {
                SetExcuteOpr(i, ExcuteOrder.Defend);
            }
            else if (battleRoads[i].OtherMaxMoveDis == 0)
            {
                SetExcuteOpr(i, ExcuteOrder.Attack);
                m_attackList.Add(i);
            }
            else 
            {
                SetExcuteOpr(i, ExcuteOrder.Random);
                m_randomList.Add(i);
            }
        }
        if (m_excuteOpr == ExcuteOrder.Random)
        {
            int excuteIndex = UnityEngine.Random.Range(0, m_randomList.Count);
            AnimalChecker.ExcuteOpr(excuteIndex,0);
        }
        else if (m_excuteOpr == ExcuteOrder.Attack) 
        {
            int excuteIndex = UnityEngine.Random.Range(0, m_attackList.Count);
            AnimalChecker.ExcuteOpr(excuteIndex, 0);
        }
        else if (m_excuteOpr != ExcuteOrder.None)
        {
            float power = battleRoads[m_excuteIndex].RoadOtherAllPower - battleRoads[m_excuteIndex].RoadSelfAllPower;
            AnimalChecker.ExcuteOpr(m_excuteIndex, power);
        }
    }

    private void SetExcuteOpr(int index, ExcuteOrder opr)
    {
        if (m_excuteOpr < opr)
        {
            m_excuteIndex = index;
            m_excuteOpr = opr;
        }
    }

    public void Reset()
    {
 
    }
}
