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
    public bool TryOnRoad(Vector3 animalPos, bool isTop)
    {
        Vector3 roadPos =transform.position;
        return animalPos.z <= BottomOnRoadPoint && animalPos.x >= roadPos.x - 1 && animalPos.x <= roadPos.x + 1;
    }
    public void OnRoad(AnimalEntity animal, bool isTop)
    {
        animal.Index = RoadIndex;
        float zPos =TopMarkPos;
        if (isTop)
            t2bSide.AddAnimal(animal);
        else
        { 
            b2tSide.AddAnimal(animal);
            zPos =BottomMarkPos;
        }
        animal.SetParent(transform, new Vector3(0, 0, zPos));
        animal.SetState(AnimalState.Run);
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
                    if (i > 0 && SideAnimals[0].CurState== AnimalState.Connect)
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
            if (SideSpeed < 0 && SideAnimals[SideAnimals.Count-1].MoveDistance<=0)
            {
                SideAnimals.RemoveAt(SideAnimals.Count - 1);
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
