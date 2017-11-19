using System;
public enum AnimalState
{
    None,
    Wait,
    Select,
    Run,
    Connect,
    Dead,
    Finish,
}

public enum ExcuteOrder
{
    None = 0,//无操作
    Random,//随机骚扰
    Attack,//进攻
    Defend,//防守
    ImportDefend,//重点防守

}

public enum AttackDirection
{
    Top,
    Bottom,
}

/**
 * ==============================================================================================
 *  上面是枚举 下面是简单数据
 * ==============================================================================================
 * **/


[Serializable]
public class AIBattleInfo
{
    //整条路我方动物力量
    public float RoadSelfAllPower;
    //整条路上对方动物力量
    public float RoadOtherAllPower;
    //对方动物移动最大距离
    public float OtherMaxMoveDis;

    public bool IsLastBodyMoveOut;


}