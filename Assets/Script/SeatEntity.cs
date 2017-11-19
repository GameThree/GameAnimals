using UnityEngine;
using System.Collections;

public class SeatEntity : MonoBehaviour 
{
    [Header("座位编号")]
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
                value.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
        }
    }

}
