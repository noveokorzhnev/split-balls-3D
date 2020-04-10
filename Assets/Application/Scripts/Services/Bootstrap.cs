using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap
{
    private class GameResourcesData
    {
        public GameplaySettings gameplaySettings;
        public MonetizationSettings monetizationSettings;
        public BallPrefabs ballPrefabs;
        public StagePrefabs stagePrefabs;
        public ShopModel shopModel;
    }
    private static GameResourcesData resourcesData;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void BeforeSceneLoad()
    {
        // Load all game resources data.
        resourcesData = new GameResourcesData()
        {
            gameplaySettings = Resources.Load<GameplaySettings>("GameplaySettings"),
            monetizationSettings = Resources.Load<MonetizationSettings>("MonetizationSettings"),
            ballPrefabs = Resources.Load<BallPrefabs>("BallPrefabs"),
            stagePrefabs = Resources.Load<StagePrefabs>("StagePrefabs"),
            shopModel = Resources.Load<ShopModel>("ShopModel")
        };        
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AfterSceneLoad()
    {
        // Initialize basic game systems.
        var userInterfaceSystem = new UserInterfaceSystem();
        var controlSystem = new ControlSystem(resourcesData.gameplaySettings);
        var keysSystem = new KeysSystem(resourcesData.stagePrefabs);
        var stageSystem = new StageSystem(resourcesData.stagePrefabs, keysSystem);
        var ballSystem = new BallSystem(resourcesData.ballPrefabs, stageSystem);
        var gameCurrencySystem = new GameCurrencySystem(userInterfaceSystem);
        var monetizationSystem = new MonetizationSystem(resourcesData.monetizationSettings, gameCurrencySystem);
        var shopSystem = new ShopSystem(resourcesData.shopModel, ballSystem, gameCurrencySystem, monetizationSystem);

        // Initialize gameplay systems.
        var gameInteractionsSystem = new GameInteractionsSystem(ballSystem, keysSystem, stageSystem);
        var gameInterfaceSystem = new GameInterfaceSystem(stageSystem, gameInteractionsSystem);

        // Register a proxy service for gameplay systems.
        var gameplaySystemsProxy = new GameplaySystemsProxy();
        gameplaySystemsProxy.Add(
            gameInteractionsSystem,
            gameInterfaceSystem);

        // Initialize the state machine system with all dependencies.
        var gameStateMachine = new GameStateMachine(
            gameplaySystemsProxy, 
            userInterfaceSystem, 
            controlSystem,
            stageSystem,
            ballSystem,
            keysSystem,
            gameCurrencySystem,
            monetizationSystem);

        // Preserve links to game services.
        var storage = new GameObject("[Game Services]").AddComponent<GameServices>();
        storage.Add(
            userInterfaceSystem,
            controlSystem,
            keysSystem,
            stageSystem,
            ballSystem,
            gameCurrencySystem,
            monetizationSystem,
            shopSystem);
        storage.Add(
            gameplaySystemsProxy,
            gameStateMachine);

        // Free the static reference to resources data - it's held by services from now.
        resourcesData = null;

        // Since there is only one scene in the project, we just link this very bootstrap script to the event of reloading scene.
        SceneManager.sceneLoaded += RebootOnNextSceneLoaded;
    }

    private static void RebootOnNextSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= RebootOnNextSceneLoaded;

        BeforeSceneLoad();
        AfterSceneLoad();
    }
}

