using System;
using UnityEngine;

public interface EnemyController
{
    public Vector2 position();

    public float knockbackSpeed();
}
