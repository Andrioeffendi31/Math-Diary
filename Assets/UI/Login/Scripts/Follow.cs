using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Follow : MonoBehaviour
{
    private ParticleSystem particle;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.Stop();
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            particle.Play();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            particle.Stop();
        }
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        transform.position = mousePosition;
    }
}
