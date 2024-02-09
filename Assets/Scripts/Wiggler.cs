using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggler : MonoBehaviour
{
    Vector2 initPos;
    Vector2 currentPos;
    Vector2 targetPos;

    float animationTime;
    float timeElapsed;
    [SerializeField] Vector3[] targetPoints;
    [SerializeField] float[] animationTimes;
    int targetIndex;

    bool wiggling;
    bool moving;
    void Start()
    {
        initPos = new Vector2(transform.localPosition.x, transform.localPosition.y);
        currentPos = initPos;
    }

    public void AnimatedMove(Vector3 targetPoint)
    {
        initPos = transform.position;
        //targetPoint.y = 1;
        SetDestination(targetPoint, .5f, true);

    }

    public void Wiggle()
    {
        if (wiggling)
        {
            return;
        }
        initPos = transform.localPosition;
        targetIndex = 0;
        SetDestination(targetPoints[targetIndex], animationTimes[targetIndex], false);
        wiggling = true;
    }

    void SetDestination(Vector2 destination, float time, bool useWorldPosition)
    {
        timeElapsed = 0;
        animationTime = time;
        if (useWorldPosition)
        {
            currentPos = transform.position;
            targetPos = destination;
            moving = true;
        }
        else
        {
            currentPos = transform.localPosition;
            targetPos = initPos + destination;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (wiggling)
        {
            timeElapsed += Time.deltaTime / animationTime;
            transform.localPosition = Vector3.Lerp(currentPos, targetPos, timeElapsed);

            if (Vector3.Distance(transform.localPosition, targetPos) <= 0.001f)
            {
                if (targetIndex + 1 >= targetPoints.Length)
                {
                    wiggling = false;
                }
                else
                {
                    targetIndex++;
                    SetDestination(targetPoints[targetIndex], animationTimes[targetIndex], false);
                }

            }
        }

        if (moving)
        {
            timeElapsed += Time.deltaTime / animationTime;
            transform.position = Vector3.Lerp(currentPos, targetPos, timeElapsed);

            if (Vector3.Distance(transform.position, targetPos) <= 0.001f)
            {
                moving = false;
                initPos = transform.localPosition;
            }
        }
    }
}
