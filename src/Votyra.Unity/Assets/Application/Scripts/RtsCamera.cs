using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RtsCamera : MonoBehaviour
{

    // public Vector3 Position;
    // public Vector3 Rotation;

    public Vector3 SpeedTranslation = Vector3.one;

    public Vector3 SpeedRotation = Vector3.one;
    // Use this for initialization
    void Start()
    {
        // Position = transform.localPosition;
        // Rotation = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        var moveX = Input.GetAxis("Horizontal1") * SpeedTranslation.x;
        var moveY = Input.GetAxis("Vertical") * SpeedTranslation.y;
        var moveZ = Input.GetAxis("Horizontal2") * SpeedTranslation.z;
        var move = new Vector3(moveX, moveY, moveZ);

        var currentRotationXZ = transform.rotation.eulerAngles.y;
        var currentRotationPlane = Quaternion.Euler(0, currentRotationXZ, 0);

        move = currentRotationPlane * move;

        var rotationXZ = Input.GetAxis("RotationHorizontal") * SpeedRotation.y;

        // Position = new Vector3(Position.x + moveX * Speed.x, Position.y + moveY * Speed.y, Position.z + moveZ * Speed.z);

        transform.Translate(move, Space.World);

        transform.Rotate(Vector3.up, rotationXZ, Space.World);
        // transform.localPosition = Position;
        // transform.localRotation = Quaternion.Euler(Rotation);
    }
}
