using UnityEngine;
using System.Collections;

public class AutoScale : MonoBehaviour
{
    [SerializeField]
    float min = 1.0f;
    [SerializeField]
    float max = 1.3f;

    [SerializeField]
    float period = 2f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float scale = min + (max - min) * (1 + Mathf.Sin(Time.time / period * 2 * Mathf.PI)) / 2;
        transform.localScale = new Vector3(scale, scale);
    }
}
