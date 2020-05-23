using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "ade885fbcc22c31e81decfe5e8f1c0002335b35f")]
public class HealthComponent : Component
{
    private float maxHealth = 100.0f;
    private float health;

    private bool isAlive;

    private void Init()
    {
        // write here code to be called on component initialization

        maxHealth = PropLib.LoadValue(node, "HealthProp", "MaxHealth").Float;
        var shouldUseHealth = PropLib.LoadValue(node, "HealthProp", "ShouldUseHealth").Int;
        if (shouldUseHealth == 1)
        {
            health = PropLib.LoadValue(node, "HealthProp", "Health").Float;
        }
        else
        {
            health = maxHealth;
        }

        isAlive = true;
    }


    private void Update()
    {
        // write here code to be called before updating each render frame
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;

        health -= damage;
        if (!(health <= 0.0f)) return;

        Log.Message($"You are died\n");
        isAlive = false;
    }
}