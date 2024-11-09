using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    [SerializeField] public float ParallaxX;
    [SerializeField] public float ParallaxY;

    private Vector3 _initialPosition;
    private Camera _camera;

    void Start()
    {
        _initialPosition = transform.position;
        _camera = Camera.main;
    }


    void Update()
    {
        Vector3 parallax = _camera.transform.position;
        parallax.x *= ParallaxX;
        parallax.y *= ParallaxY;
        parallax.z = 0;
        transform.position = _initialPosition + parallax;
    }
}
