using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RoadEntity : MonoBehaviour 
{
    [Header("路的编号")]
    public int RoadIndex = 0;
    [Header("路的左边界(pos.x)")]
    public float RoadLeft = 0;
    [Header("路的右边界(pos.x)")]
    public float RoadRight = 0;
    [Header("上面出发位置(pos.y)")]
    public float TopMarkPos = 10;
    [Header("下面出发位置(pos.y)")]
    public float BottomMarkPos = -5;
    [Header("上路点，超过该点动物才会被放下(pos.y)")]
    public float BottomOnRoadPoint = -3.5f;
    public SideInfo t2bSide = new SideInfo();
    public SideInfo b2tSide = new SideInfo();
    private float RoadLength;
    void Start()
    {
        RoadLength = TopMarkPos - BottomMarkPos;
    }
    public bool TryOnRoad(Vector3 animalPos, AttackDirection dir)
    {
        Vector3 roadPos =transform.position;
       
        /**
         * 默认判断从上往下走，先判断此路上走的最后一个动物是否走出去了（走出身长的距离） 如果没走出去则不能放。
         * 否则判断从下往上走的第一个是否马上就到终点了，如果马上就到终点则此路此时不能放，
         * 如果是从下往上走也是相同逻辑
         * **/
        bool canPutDown = false;
        if (dir == AttackDirection.Top)
        {
           canPutDown = animalPos.z > BottomOnRoadPoint &&CanPutDown(b2tSide,t2bSide );
        }
        else
        {
             canPutDown = CanPutDown(t2bSide, b2tSide);
        }
        return canPutDown && animalPos.x >= RoadLeft && animalPos.x <= RoadRight;
    }

    public bool CanPutDown(SideInfo side,SideInfo other)
    {
        if (side.AllAnimalMoveOut())
        {
            var first = other.GetFirst();
            if (first)
            {
                return RoadLength - first.moveDistance > 0.0005f;
            }
            return true;
        }
        return false;
    }

    public void OnRoad(AnimalEntity animal)
    {
        animal.Index = RoadIndex;
        float zPos = TopMarkPos;
        float xPos = transform.localPosition.x;
        if (animal.AttackDir == AttackDirection.Bottom)
            t2bSide.AddAnimal(animal);
        else
        { 
            b2tSide.AddAnimal(animal);
            zPos = BottomMarkPos;
        }
        animal.transform.parent = null;
        animal.OnToRoad(xPos,zPos);
    }
    public void UpdateState(float deltaTime)
    {
        t2bSide.UpdateMove(deltaTime, RoadLength,b2tSide.GetFirstMoveDis());
        b2tSide.UpdateMove(deltaTime, RoadLength ,t2bSide.GetFirstMoveDis());
        t2bSide.UpdateState( b2tSide.GetFirst(), RoadLength,b2tSide.SidePower);
        b2tSide.UpdateState( t2bSide.GetFirst(), RoadLength,t2bSide.SidePower);
        t2bSide.UpdateSpeed(b2tSide.SidePower);
        b2tSide.UpdateSpeed(t2bSide.SidePower);
    }

    public float GetTotalPower(AttackDirection dir)
    {
        float maxPower = 0f;
        if (dir == AttackDirection.Top)
        {
            for (int i = 1; i < b2tSide.SideAnimals.Count; i++)
            {
                maxPower += b2tSide.SideAnimals[i].Power;
            }
        }
        else 
        {
            for (int i = 1; i < t2bSide.SideAnimals.Count; i++)
            {
                maxPower += t2bSide.SideAnimals[i].Power;
            }
        }
        return maxPower;
    }

    public float GetMaxDis(AttackDirection dir)
    {
        float maxDis = 0;
        if (dir == AttackDirection.Top)
        {
            maxDis = b2tSide.GetFirstMoveDis();
        }
        else
        {
            maxDis = t2bSide.GetFirstMoveDis();
        }
        return maxDis;
    }

    public int GetPoint(AttackDirection dir)
    {
        if (dir == AttackDirection.Top)
        {
            return b2tSide.Points;
        }
        else
        {
            return t2bSide.Points;
        }
    }

    public float GetLastConnectMoveDis(AttackDirection dir)
    {
        if (dir == AttackDirection.Top)
        {
            return b2tSide.GetLastConnectMoveDis();
        }
        else
        {
            return t2bSide.GetLastConnectMoveDis();
        }
    }
}
[Serializable]
public class SideInfo
{
    public List<AnimalEntity> SideAnimals = new List<AnimalEntity>();
    /// <summary>
    /// 基础速度的百分比
    /// </summary>
    public float SpeedPercent = 0;
    /// <summary>
    /// 一个方向的总体力量
    /// </summary>
    public float SidePower = 0;

    public int Points = 0;

    public void AddAnimal(AnimalEntity entity)
    {
        SideAnimals.Add(entity);
    }
    //先更新移动再更新状态
    public void UpdateState(AnimalEntity diffSideFirstAnim, float roadLength, float otherPower)
    {
        for (int i = 0; i < SideAnimals.Count; i++)
        {
            //如果第一个动物未连接，则判断第一个动物和反向动物第一个碰头的时候设置为链接状态 
            if (i == 0 && SideAnimals[0].CurState != AnimalState.Connect)
            {
                bool canConnect = false;
                if (diffSideFirstAnim != null)
                {
                    canConnect = CanDiffSideConnect(diffSideFirstAnim, SideAnimals[0], roadLength);
                }
                //如果链接起来则设置此方向的力度
                if (canConnect)
                {
                    SidePower += SideAnimals[0].Power;
                    SideAnimals[0].SetState(AnimalState.Connect);
                }
            }
            else
            {
                //如果对面没动物了 要把所有设置为非链接状态并且方向力清零
                if (diffSideFirstAnim == null)
                {
                    if (SideAnimals[i].CurState == AnimalState.Connect)
                    {
                        SideAnimals[i].SetState(AnimalState.Run);
                    }
                    SidePower = 0;
                }
                //如果为链接状态则按照链接状态移动
                if (SideAnimals[i].CurState == AnimalState.Run)
                {//非链接状态判断是否需要链接 和前面一个动物的距离差值是否为前一动物的身长 是则链接 否则则按照非链接状态移动
                    bool canConnect = false;
                    if (i > 0 && SideAnimals[0].CurState == AnimalState.Connect)
                    {
                        canConnect = CanSameSideConnect(SideAnimals[i - 1], SideAnimals[i]);
                    }
                    //如果链接起来方向力度增加
                    if (canConnect)
                    {
                        SidePower += SideAnimals[i].Power;
                        SideAnimals[i].SetState(AnimalState.Connect);
                    }
                }
            }
        }

        if (SideAnimals.Count > 0)
        {
            //判断地0个动物是否跑完 跑完则把后面链接的动物的状态设为run,并且把方向力设置为0
            if (SideAnimals[0].MoveDistance >= roadLength+0.01f)
            {
                var entity = SideAnimals[0];
                if (entity.CurState == AnimalState.Connect)
                {
                    SidePower -= entity.Power;
                }
                entity.SetState(AnimalState.Finish);
                Points += entity.FinshPoint;
                SideAnimals.RemoveAt(0);
                AnimalFactory.AddToCollectList(entity);
                if (SideAnimals.Count > 0)
                {
                    SideAnimals[0].SetState(AnimalState.Run);
                }
                SidePower = 0;
                for (int i = 0; i < SideAnimals.Count; i++)
                {
                    //如果第0个动物是Run则后面每个动物都是run
                    if (SideAnimals[0].CurState == AnimalState.Run && SideAnimals[i].CurState == AnimalState.Connect)
                    {
                        SideAnimals[i].SetState(AnimalState.Run);
                    }
                }
            }
            //如果被对面推回去则方向力减小
            int lastIndex = SideAnimals.Count - 1;
            if (SideAnimals.Count > 0 && SideAnimals[lastIndex].MoveDistance <= 0)
            {
                var entity = SideAnimals[lastIndex];
                if (entity.CurState == AnimalState.Connect)
                {
                    SidePower -= entity.Power;
                }
                entity.SetState(AnimalState.Dead);
                SideAnimals.RemoveAt(lastIndex);
                AnimalFactory.AddToCollectList(entity);
            }
        }
    }

    public void UpdateMove(float deltaTime, float roadLength ,float otherMoveDis)
    {
        for (int i = 0; i < SideAnimals.Count; i++)
        {
            float normalDeltaMove = deltaTime * SideAnimals[i].AnimalDefaultSpeed;
            float connectDeltaMove = deltaTime * SideAnimals[i].AnimalDefaultSpeed * SpeedPercent;
            //如果为链接状态则按照链接状态移动
            if (SideAnimals[i].CurState == AnimalState.Connect)
            {
                SideAnimals[i].Move(connectDeltaMove);
            }
            else if (SideAnimals[i].CurState == AnimalState.Run)
            {//非链接状态判断是否需要链接 和前面一个动物的距离差值是否为前一动物的身长 是则链接 否则则按照非链接状态移动
                float useMove = normalDeltaMove;
                if (i == 0&&otherMoveDis > 0.01f)
                {
                    useMove = Math.Min(normalDeltaMove, roadLength - otherMoveDis - SideAnimals[i].MoveDistance);
                }
                else if (i>0&&SideAnimals[i-1].CurState == AnimalState.Connect)
                {
                    useMove = Math.Min(normalDeltaMove, SideAnimals[i-1].MoveDistance - SideAnimals[i].MoveDistance);
                }
                SideAnimals[i].Move(useMove);
            }
        }
    }

    public void UpdateSpeed(float otherPower)
    {
        if (otherPower == 0)
            SpeedPercent = 1;
        else 
        {
            SpeedPercent = (SidePower - otherPower) / (SidePower + otherPower);
        }
        //if (otherPower>0)
        //Debug.LogWarning(otherPower + "        "  + SidePower +"           "+ SpeedPercent);
        //(SidePower -otherPower)/SidePower
    }
    //判断相对跑的动物是否能链接起来
    public bool CanDiffSideConnect(AnimalEntity anima0,AnimalEntity animal1,float roadLength)
    {
        return (anima0.MoveDistance + animal1.MoveDistance) >= roadLength;
    }
    //判断同一侧的动物链接起来
    public bool CanSameSideConnect(AnimalEntity anima0, AnimalEntity animal1)
    {
        return (anima0.MoveDistance - animal1.MoveDistance - anima0.BodyLength ) <= 0;
    }
    public bool AllAnimalMoveOut()
    {
        if (SideAnimals.Count == 0)
        {
            return true;
        }
        return SideAnimals[SideAnimals.Count - 1].moveDistance > SideAnimals[SideAnimals.Count - 1].BodyLength;
    }
    public AnimalEntity GetFirst()
    {
        if (SideAnimals.Count > 0)
            return SideAnimals[0];

        return null;
    }

    public float GetFirstMoveDis()
    {
        if (SideAnimals.Count > 0)
            return SideAnimals[0].MoveDistance;
        return 0;
    }

    public float GetLastConnectMoveDis()
    {
        float moveDis = 0f;
        for (int i = 0; i < SideAnimals.Count; i++)
        {
            if (SideAnimals[i].CurState == AnimalState.Connect)
            {
                moveDis = SideAnimals[i].MoveDistance;
            }
            else 
            {
                break;
            }
        }

        return moveDis;
    }
}
