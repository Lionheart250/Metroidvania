using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public int length;
    public LineRenderer lineRend;
    public Vector3[] segmentPoses;
    private Vector3[] segmentV;
    public Transform targetDir;
    public float targetDist;
    public float smoothSpeed; 
    public float trailSpeed;

    public float wiggleSpeed;
    public float wiggleMagnitude;
    public Transform wiggleDir;
    //public Transform tailEnd;

    private void Start()
    {
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];

        // Set initial positions to targetDir position
        for (int i = 0; i < length; i++)
        {
            segmentPoses[i] = targetDir.position;
        }

        // Set positions in LineRenderer
        lineRend.SetPositions(segmentPoses);
    }

    private void Update()
    {
        wiggleDir.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time % wiggleSpeed) * wiggleMagnitude);

        segmentPoses[0] = targetDir.position;

        for(int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1] + targetDir.right * targetDist, ref segmentV[i], smoothSpeed + i / trailSpeed);
            
        }
        lineRend.SetPositions(segmentPoses);

        //tailEnd.position = segmentPoses[segmentPoses.Length - 1];
    }
}
