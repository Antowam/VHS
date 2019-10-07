using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomShaker : MonoBehaviour
{
    public float shakeSpeed;
    public float shakeAmount;


    Vector3 initPos;
    Vector2 newVec;

    float initRot;

    public Vector3 euler;
    Vector3 initEuler;

    public float maxRotation;
    public float speed;

    void Start()
    {
        initEuler = transform.eulerAngles;
        initPos = transform.position;
        newVec = (Random.insideUnitSphere * shakeAmount) + initPos;
        euler.z = Random.Range(initEuler.z - 20f, initEuler.z + 20f);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, newVec, shakeSpeed * Time.deltaTime);
        if ((newVec - (Vector2)transform.position).magnitude < 2f)
        {
            newVec = (Random.insideUnitSphere * shakeAmount) + initPos;
        }

        transform.rotation = Quaternion.Euler(maxRotation * Mathf.Sin(Time.time * speed), -maxRotation * Mathf.Sin(Time.time * speed), maxRotation * Mathf.Sin(Time.time * speed * 2));

    }
}
