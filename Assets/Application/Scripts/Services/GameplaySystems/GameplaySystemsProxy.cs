using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplaySystemsProxy : IGameplaySystem
{
    private List<IGameplaySystem> systems;

    public GameplaySystemsProxy()
    {
        systems = new List<IGameplaySystem>();
    }

    public void Add(params IGameplaySystem[] systems)
    {
        this.systems.AddRange(systems);
    }

    public void OnGameplayStarted()
    {
        foreach (var system in systems)
        {
            system.OnGameplayStarted();
        }
    }

    public void OnGameplayStopped()
    {
        foreach (var system in systems)
        {
            system.OnGameplayStopped();
        }
    }
}
