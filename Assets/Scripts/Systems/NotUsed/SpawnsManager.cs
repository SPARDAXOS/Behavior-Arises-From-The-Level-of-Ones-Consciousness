using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnsManager : MonoBehaviour
{
    private int MonstersPoolSize = 1;
    private int MonstersFlockSize = 3;

    private PlayersController CControllerScript;
    private EnemiesController EControllerScript;

    private GameObject DamageDealerAsset;
    private GameObject DamageDealerRef;
    private DamageDealer DamageDealerScript;

    private GameObject TankAsset;
    private GameObject TankRef;
    private Tank TankScript;

    private GameObject HealerAsset;
    private GameObject HealerRef;
    private Healer HealerScript;

    private GameObject MonsterAsset;

    private int MonstersEntityNumber = 0; //Increment on every dps creation.

    private int DPSIDNum = 0; //Increment on every dps creation.

    public List<GameObject> MonstersPool = new List<GameObject>();


    private void LoadResources()
    {
        DamageDealerAsset = Resources.Load<GameObject>("Entities/DPS");
        if (!DamageDealerAsset)
            Debug.LogError("Wrong asset path at SpawnsManager - LoadResources - DPSAsset");
        else
        {
            DamageDealerRef = Instantiate(DamageDealerAsset);
            DamageDealerScript = DamageDealerRef.GetComponent<DamageDealer>();
            //Set Specifics
            DamageDealerRef.SetActive(false);
        }

        TankAsset = Resources.Load<GameObject>("Entities/Tank");
        if (!TankAsset)
            Debug.LogError("Wrong asset path at SpawnsManager - LoadResources - TankAsset");
        else
        {
            TankRef = Instantiate(TankAsset);
            TankScript = TankRef.GetComponent<Tank>();
            //Set Specifics
            TankRef.SetActive(false);
        }

        HealerAsset = Resources.Load<GameObject>("Entities/Healer");
        if (!HealerAsset)
            Debug.LogError("Wrong asset path at SpawnsManager - LoadResources - HealerAsset");
        else
        {
            HealerRef = Instantiate(HealerAsset);
            HealerScript = HealerRef.GetComponent<Healer>();
            //Set Specifics
            HealerRef.SetActive(false);
        }

        MonsterAsset = Resources.Load<GameObject>("Entities/Monster");
        if (!MonsterAsset)
            Debug.LogError("Wrong asset path at SpawnsManager - LoadResources - MonsterAsset");
    }
    private void CreateMonstersPool()
    {
        if (!MonsterAsset)
        {
            Debug.LogError("Null MonsterAsset reference at CreateMonstersPool");
            return;
        }

        for(int i = 0; i < MonstersPoolSize; i++)
        {
            GameObject temp = Instantiate(MonsterAsset);

            //temp.GetComponent<Monster>().Init(MonstersEntityNumber);
            MonstersEntityNumber++;


            //temp.SetActive(false);
            MonstersPool.Add(temp);
        }
    }

    private void SetupCharacters()
    {
        
    }

    public void AssignCController(PlayersController script)
    {
        if (script)
            CControllerScript = script;
        else
            Debug.LogError("Null reference at SpawnsManager - AssignCController");
    }
    public void AssignEController(EnemiesController script)
    {
        if (script)
            EControllerScript = script;
        else
            Debug.LogError("Null reference at SpawnsManager - AssignEController");
    }



    void Start()
    {
        //LoadResources();
        //CreateMonstersPool();
    }
    void Update()
    {
        
    }
}
