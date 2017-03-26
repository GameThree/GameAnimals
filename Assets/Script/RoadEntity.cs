using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadEntity : MonoBehaviour 
{
    public int RoadIndex = 0;
    public float TopMarkPos = 10;
    public float BottomMarkPos = -5;
    public float BottomOnRoadPoint = -3.5f;
    private SideInfo t2bSide = new SideInfo();
    private SideInfo b2tSide = new SideInfo();
    private float RoadLength;
    void Start()
    {
        RoadLength = TopMarkPos - BottomMarkPos;
    }
    public bool TryOnRoad(AnimalEntity animal, bool isTop)
    {
        Vector3 animalPos =animal.transform.position;
        Vector3 roadPos =transform.position;
        if (animalPos.z <= BottomOnRoadPoint && animalPos.x >= roadPos.x - 1 && animalPos.x <= roadPos.x + 1)
        {
            OnRoad(animal, isTop);
            return true;
        }
        return false;
    }
    public void OnRoad(AnimalEntity animal, bool isTop)
    {
        animal.Index = RoadIndex;
        if (isTop)
            t2bSide.AddAnimal(animal);
        else
            b2tSide.AddAnimal(animal);
    }
    void Update()
    {
        t2bSide.UpdateMove(Time.deltaTime, b2tSide.GetFirst(), RoadLength);
        b2tSide.UpdateMove(Time.deltaTime, t2bSide.GetFirst(), RoadLength);
        t2bSide.UpdateSpeed(b2tSide.SidePower);
        b2tSide.UpdateSpeed(t2bSide.SidePower);
    }
}

public class SideInfo
{
    public List<AnimalEntity> SideAnimals = new List<AnimalEntity>();
    public float SideSpeed = 0;
    public float SidePower = 0;

    public void AddAnimal(AnimalEntity entity)
    {
        SideAnimals.Add(entity);
    }

    public void UpdateMove(float deltaTime, AnimalEntity diffSideFirstAnim, float roadLength)
    {
        float normalDeltaMove = deltaTime * ConstValue.DefaultSpeed;
        float connectDeltaMove = deltaTime * SideSpeed;
        if (SideAnimals.Count > 0)
        {
            if (SideAnimals[0].MoveDistance >= roadLength)
            {
                var entity = SideAnimals[0];
                entity.SetState(AnimalState.Finish);
                SideAnimals.RemoveAt(0);
                SceneManager.Instance.AddToCollectList(entity);
                if (SideAnimals.Count>0)
                SideAnimals[0].SetState(AnimalState.Run);
            }
        }
        for (int i = 0; i < SideAnimals.Count; i++)
        {
            if (i == 0 && SideAnimals[0].CurState != AnimalState.Connect)
            {
                bool canConnect = false;
                if (diffSideFirstAnim != null)
                {
                    canConnect = SideAnimals[0].CanDiffSideConnect(diffSideFirstAnim.MoveDistance, diffSideFirstAnim.BodyLength);
                }
                if (canConnect)
                {
                    SideAnimals[0].SetState(AnimalState.Connect);
                    SidePower += SideAnimals[0].CurPower;
                }
                else
                {
                    SideAnimals[0].Move(normalDeltaMove);
                }
            }
            else 
            {
                if (SideAnimals[i].CurState == AnimalState.Connect)
                {
                    SideAnimals[i].Move(connectDeltaMove);
                }
                else if (SideAnimals[i].CurState == AnimalState.Run)
                {
                    SideAnimals[i].Move(normalDeltaMove);
                    bool canConnect = false;
                    if (i > 0)
                    {
                        canConnect = SideAnimals[i].CanSameSideConnect(SideAnimals[i - 1].MoveDistance, SideAnimals[i - 1].BodyLength);
                    }
                    if (canConnect)
                    {
                        SideAnimals[i].SetState(AnimalState.Connect);
                    }
                }
            }
        }

    }

    public void UpdateSpeed(float otherPower)
    {
        if (otherPower == 0)
            SideSpeed = 0;
        else 
        {
            //(SidePower -otherPower)/SidePower
        }
        //(SidePower -otherPower)/SidePower
    }

    public AnimalEntity GetFirst()
    {
        if (SideAnimals.Count > 0)
            return SideAnimals[0];

        return null;
    }
}
