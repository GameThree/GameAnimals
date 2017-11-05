using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBattleInfo 
{
    //整条路我方动物力量
    public float RoadSelfAllPower;
    //整条路上对方动物力量
    public float RoadOtherAllPower;
    //对方动物移动最大距离
    public float OtherMaxMoveDis;

}

public interface AIInterface 
{
    /// <summary>
    /// 获取AI投放决策数据
    /// </summary>
    /// <returns></returns>
    AIBattleInfo[] GetAIBattleInfo();
    /// <summary>
    /// 获取AI创建决策数据
    /// </summary>
    /// <returns></returns>
    bool CanCreateAnimal();
}
public class BattleAI : MonoBehaviour {
    [Header("创建动物间隔")]
    public float CreateAnimalDelta = 2;
    [Header("是否开启自动战斗")]
    public bool AutoFight = false;

    public float AttackDelay = 0f;

    [Header("开启防守距离")]
    public float DefendDis = 0;
    [Header("重点防守距离")]
    public float ImportantDefendDis = 0;
    [Header("放弃防守距离")]
    public float GiveUpDis = 0;
	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
