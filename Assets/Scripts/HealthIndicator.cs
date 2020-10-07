using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicator : MonoBehaviour
{
    TextMesh textMesh;
    Health health;
    float displayedHealth;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        health = GetComponentInParent<Health>();
        displayedHealth = health.current - 1.0f;
    }

    void Update()
    {
        float value = health.current;
        if (!Mathf.Approximately(displayedHealth, value)) { // !=
            displayedHealth = value;
            textMesh.text = $"{value}";
        }
    }
}
