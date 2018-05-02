using UnityEngine;
using System.Collections;

public class GameCore : MonoBehaviour
{
    private static GameCore instance = null;

    public static GameCore Instance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
	  
	}
}
