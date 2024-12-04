using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttributes : MonoBehaviour
{
    public static EnemyAttributes Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        int floorLevel = TileGenerator.Instance.floorLevel;
        Attack = 1 + floorLevel * 2;
        AttackRange = 1 + floorLevel / 10;
        CriticalDamage = 0.01f + floorLevel * 0.05f;
        CriticalRate = 0.03f + floorLevel * 0.01f;
        Defense = 1 + floorLevel * 2;
        Health = 15 + floorLevel * 10;
    }

    public int AttackRange { get; set; }
    public int Defense { get; set; }
    public float Attack { get; set; }
    public float CriticalRate { get; set; }
    public float CriticalDamage { get; set; }
    public float Health { get; set; }
}