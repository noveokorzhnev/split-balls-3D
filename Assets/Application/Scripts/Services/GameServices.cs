using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServices : MonoBehaviour
{
    private List<object> services;

    private void Awake()
    {
        services = new List<object>();
    }

    public void Add(params object[] services)
    {
        this.services.AddRange(services);
    }
}
