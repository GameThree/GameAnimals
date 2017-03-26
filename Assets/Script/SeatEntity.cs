using UnityEngine;
using System.Collections;

public class SeatEntity : MonoBehaviour 
{
    public int SeatIndex = 0;

    private AnimalEntity animal;

    public AnimalEntity Animal
    {
        get{return animal;}
        set
        {
            animal =value;
            if (value != null)
            {
                value.Index = SeatIndex;
                value.transform.parent = transform;
                value.transform.localPosition = Vector3.zero;
                value.transform.localScale = Vector3.one;
            }
        }
    }

}
