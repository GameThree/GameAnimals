using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalProbability
{
    [Header("动物的预制")]
    public GameObject Animal;
    [Header("动物的生成概率数")]
    public int Weight;
    /// <summary>
    /// 用来计算随机数的范围
    /// </summary>
    [NonSerialized]
    public int MaxNumber;
}

public class AnimalFactory : MonoBehaviour 
{
    [Header("动物工厂可生产动物数组")]
    public AnimalProbability[] Animals;
    private bool m_init = false;
    private int m_randomMaxValue = 0;
    private static GameObject recycleObj = null;
    private  static List<GameObject> m_collectAnimalList = new List<GameObject>();
	// Use this for initialization
	void Start () {
		
	}
    /// <summary>
    /// 初始化随机数据
    /// </summary>
    private void Init()
    {
        if(!m_init)
        {
            m_init =true;
            for(int i = 0; i < Animals.Length; i++)
            {
                m_randomMaxValue += Animals[i].Weight;
                Animals[i].MaxNumber = m_randomMaxValue - 1;
            }

        }
    }
    /// <summary>
    /// 根据配置的随机数据随机生产动物
    /// </summary>
    /// <returns></returns>
    public AnimalEntity CreateAnimals()
    {
        Init();
        int randomValue = UnityEngine.Random.Range(0, m_randomMaxValue);
        Debug.Log(randomValue);
        for (int i = 0;i< Animals.Length;i++)
        {
            if (randomValue < Animals[i].MaxNumber)
            {
                return GetAnimalEntity(Animals[i].Animal);
            }
        }
        if (Animals.Length > 0)
        {
            return GetAnimalEntity(Animals[1].Animal);
        }
        return null;
    }
    
    /// <summary>
    /// 回收动物
    /// </summary>
    /// <param name="entity"></param>
    public static void AddToCollectList(AnimalEntity entity)
    {
        if (recycleObj == null)
        {
            recycleObj = new GameObject("Recycle");
        }
        entity.gameObject.SetActive(false);
        entity.Clear();
        entity.transform.parent = recycleObj.transform;
        m_collectAnimalList.Add(entity.gameObject);
    }
    /// <summary>
    /// 优先从回收列表中找动物，找不到再生产新的动物
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public AnimalEntity GetAnimalEntity(GameObject prefab)
    {
        GameObject result = null;
        for (int i = 0; i < m_collectAnimalList.Count; i++)
        {
            if (m_collectAnimalList[i].name == prefab.name)
            {
                result = m_collectAnimalList[i];
                m_collectAnimalList.RemoveAt(i);
                break;
            }
        }
        if(result==null)
        {
            result = GameObject.Instantiate(prefab);
            result.name = prefab.name;
        }
        return result.GetComponent<AnimalEntity>();
    }
}
