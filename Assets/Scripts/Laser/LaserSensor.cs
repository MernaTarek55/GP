using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class LaserSensor : MonoBehaviour,IInput
{
    public event Action<Laser> onLaserAdded;
    public event Action<Laser> onLaserRemoved;

    public event Action<IInput> onTriggered;
    public event Action<IInput> onUntriggered;

    bool _isTriggered = false;
    public bool IsTriggered
    {
        get => _isTriggered;
        protected set
        {
            if (_isTriggered == value) return;
            if(value)
            {
                onTriggered?.Invoke(this);
            }
            else
            {
                onUntriggered?.Invoke(this);
            }
            _isTriggered = value;
        }
    }
    List<Laser> strikingLasers;

    private void Awake()
    {
        strikingLasers = new List<Laser>();
    }
    void Start()
    {
        IsTriggered = false;
    }
    public static void HandleLaser(Laser laser, LaserSensor prev, LaserSensor current)
    {
        if(prev == current)
        {
            return; // No change in sensor, do nothing
        }
        if (prev != null)
        {
            prev.RemoveLaser(laser);
        }
        if (current != null)
        {
            current.AddLaser(laser);
        }
    }
    void AddLaser(Laser strikingLaser)
    {
        if (strikingLasers.Contains(strikingLaser)) return;
        strikingLasers.Add(strikingLaser);
        onLaserAdded?.Invoke(strikingLaser);
        if (strikingLasers.Count == 1)
        {
            IsTriggered = true;
        }
    }
    void RemoveLaser(Laser strikingLaser)
    {
        if (!strikingLasers.Contains(strikingLaser)) return;
        strikingLasers.Remove(strikingLaser);
        onLaserRemoved?.Invoke(strikingLaser);
        if (strikingLasers.Count == 0)
        {
            IsTriggered = false;
        }
    }
}
