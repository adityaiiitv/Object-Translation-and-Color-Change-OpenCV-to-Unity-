using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PositionAtFaceScreenSpace : MonoBehaviour
{
    private float _camDistance;

    void Start()
    {
        _camDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
        //_camDistance = transform.position.x;
    }

    void Update()
    {
        if (OpenCVFaceDetection.NormalizedFacePositions.Count == 0)
            return;

        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(OpenCVFaceDetection.NormalizedFacePositions[0].x, OpenCVFaceDetection.NormalizedFacePositions[0].y, _camDistance));
        //transform.position = transform.position + new Vector3(1.0f, 0.0f, 0.0f);
    }
}
