using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private GameObject MapAsset = null;
    private GameObject MapRef = null;
    private Map MapScript = null;

    private GameObject GameModeAsset = null;
    private GameObject GameModeRef = null;
    private GameMode GameModeScript = null;

    private GameObject HUDAsset = null;
    private GameObject HUDRef = null;
    private HUD HUDScript = null;

    private GameObject MainCameraAsset = null;
    private GameObject MainCameraRef = null;

    private GameObject PControllerAsset = null;
    private GameObject PControllerRef = null;
    private PlayersController PControllerScript = null;

    private GameObject EControllerAsset = null;
    private GameObject EControllerRef = null;
    private EnemiesController EControllerScript = null;


    private void LoadResources()
    {
        MapAsset = Resources.Load<GameObject>("Systems/Map");
        if (!MapAsset)
            Debug.LogError("Wrong asset path at SimulationManager - LoadResources - MapAsset");
        else
        {
            MapRef = Instantiate(MapAsset);
            MapScript = MapRef.GetComponent<Map>();
        }

        GameModeAsset = Resources.Load<GameObject>("Systems/GameMode");
        if (!GameModeAsset)
            Debug.LogError("Wrong asset path at SimulationManager - LoadResources - GameModeAsset");
        else
        {
            GameModeRef = Instantiate(GameModeAsset);
            GameModeScript = GameModeRef.GetComponent<GameMode>();
        }

        HUDAsset = Resources.Load<GameObject>("Systems/HUD");
        if (!HUDAsset)
            Debug.LogError("Wrong asset path at SimulationManager - LoadResources - HUDAsset");
        else
        {
            HUDRef = Instantiate(HUDAsset);
            HUDScript = HUDRef.GetComponent<HUD>();
        }

        MainCameraAsset = Resources.Load<GameObject>("Systems/MainCamera");
        if (!MainCameraAsset)
            Debug.LogError("Wrong asset path at SimulationManager - LoadResources - MainCameraAsset");
        else
        {
            MainCameraRef = Instantiate(MainCameraAsset);
        }

        PControllerAsset = Resources.Load<GameObject>("Systems/CharactersController");
        if (!PControllerAsset)
            Debug.LogError("Wrong asset path at SimulationManager - LoadResources - CharactersControllerAsset");
        else
        {
            PControllerRef = Instantiate(PControllerAsset);
            PControllerScript = PControllerRef.GetComponent<PlayersController>();
        }

        EControllerAsset = Resources.Load<GameObject>("Systems/EnemiesController");
        if (!EControllerAsset)
            Debug.LogError("Wrong asset path at SimulationManager - LoadResources - EnemiesControllerAsset");
        else
        {
            EControllerRef = Instantiate(EControllerAsset);
            EControllerScript = EControllerRef.GetComponent<EnemiesController>();
        }
    }
    private void InitializeSystems()
    {
        //Just to get it to look good. Maybe make proper solution later.
        MapRef.transform.position = new Vector3(-0.83f, 0.0f, 131.0f);
        MainCameraRef.transform.position = new Vector3(0.0f, 0.0f, 65.0f);

        GameModeScript.SetPCScriptReference(PControllerScript);
        GameModeScript.SetECScriptReference(EControllerScript);

        MapScript.Init();

        PControllerScript.SetGMScriptReference(GameModeScript);
        PControllerScript.SetMapScriptReference(MapScript);
        PControllerScript.SetHUDScriptReference(HUDScript);
        PControllerScript.SetECScriptReference(EControllerScript);
        PControllerScript.Init();

        EControllerScript.SetGMScriptReference(GameModeScript);
        EControllerScript.SetMapScriptReference(MapScript);
        EControllerScript.SetHUDScriptReference(HUDScript);
        EControllerScript.SetPCScriptReference(PControllerScript);
        EControllerScript.Init();
    }


    private void Awake()
    {
        LoadResources();
        InitializeSystems();
        GameModeScript.Activate(); //To start the simulation.
    }
    private void Update()
    {
        GameModeScript.UpdateTurns();
    }
}
