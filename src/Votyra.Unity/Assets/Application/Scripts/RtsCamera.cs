using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RtsCamera : MonoBehaviour
{
    public Vector3 SpeedTranslation = Vector3.one;

    public Vector3 SpeedRotation = Vector3.one;

    void Update()
    {
        var moveX = Input.GetAxis("Horizontal1") * Time.deltaTime * SpeedTranslation.x;
        var moveY = Input.GetAxis("Vertical") * Time.deltaTime * SpeedTranslation.y;
        var moveZ = Input.GetAxis("Horizontal2") * Time.deltaTime * SpeedTranslation.z;
        var move = new Vector3(moveX, moveY, moveZ);

        var currentRotationXZ = transform.rotation.eulerAngles.y;
        var currentRotationPlane = Quaternion.Euler(0, currentRotationXZ, 0);

        move = currentRotationPlane * move;

        var rotationXZ = Input.GetAxis("RotationHorizontal") * Time.deltaTime * SpeedRotation.y;

        transform.Translate(move, Space.World);
        transform.Rotate(Vector3.up, rotationXZ, Space.World);
    }
}
