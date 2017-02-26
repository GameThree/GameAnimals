using UnityEngine;
using System.Collections;

public class Boot : MonoBehaviour 
{
    private GameCore m_game;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (m_game == null)
        {
            m_game = gameObject.AddComponent<GameCore>();
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
