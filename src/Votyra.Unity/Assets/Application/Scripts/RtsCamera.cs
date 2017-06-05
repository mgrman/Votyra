using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RtsCamera : MonoBehaviour
{

    // public Vector3 Position;
    // public Vector3 Rotation;

    public Vector3 Speed;

    // Use this for initialization
    void Start()
    {
        // Position = transform.localPosition;
        // Rotation = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        var moveX = Input.GetAxis("Horizontal1") * Speed.x;
        var moveY = Input.GetAxis("Vertical") * Speed.y;
        var moveZ = Input.GetAxis("Horizontal2") * Speed.z;
        // Position = new Vector3(Position.x + moveX * Speed.x, Position.y + moveY * Speed.y, Position.z + moveZ * Speed.z);

        transform.Translate(moveX, moveY, moveZ, Space.World);

        // transform.localPosition = Position;
        // transform.localRotation = Quaternion.Euler(Rotation);
    }
}
