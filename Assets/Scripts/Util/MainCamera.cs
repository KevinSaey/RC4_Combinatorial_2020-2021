using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Vector3 _target;
    private float _rotateSpeed = 3.0f;
    private float _panSpeed = 0.3f;
    private float _minXRotation = -10f, _maxXRotation = 90f;
    private Pose _startTransform;
    private float _height = 15f;

    private void Awake()
    {
        _startTransform = new Pose(transform.position, transform.rotation);
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android) return;
        bool pan = Input.GetAxis("Pan") == 1.0f;
        bool rotate = Input.GetAxis("Rotate") == 1.0f;

        if (pan)
        {
            float right = -Input.GetAxis("Mouse X") * _panSpeed;
            float up = -Input.GetAxis("Mouse Y") * _panSpeed;

            Pan(right, up);
        }
        else if (rotate)
        {
            float yaw = Input.GetAxis("Mouse X");
            float pitch = -Input.GetAxis("Mouse Y");

            Rotate(yaw, pitch);
        }

        float zoomFactor = Input.GetAxis("Mouse ScrollWheel");
        if (zoomFactor != 0)
        {
            Zoom(zoomFactor);
        }
    }


    public void Pan(Vector2 motion) => Pan(motion.x, motion.y);
    public void Pan(float x, float y)
    {
        Vector3 vector = transform.rotation * new Vector3(x, y, 0);
        transform.position += vector;
        //if (transform.position.y < 0) transform.position = new Vector3(transform.position.x,0,transform.position.z);
        //_target += vector;//Enable is you don't want a fixed target
    }

    public void Rotate(Vector2 motion) => Rotate(motion.x, motion.y);
    public void Rotate(float yaw, float pitch)
    {
        yaw *= _rotateSpeed;
        pitch *= _rotateSpeed;

        float xAngle = FormatAngle(transform.rotation.eulerAngles.x);
        //Clamp rotation around x
        if (xAngle + pitch > _maxXRotation && pitch > 0)
            pitch = 0;
        if (xAngle + pitch < _minXRotation && pitch < 0)
            pitch = 0;
        transform.RotateAround(_target, Vector3.up, yaw);
        transform.RotateAround(_target, transform.rotation * Vector3.right, pitch);
        if (transform.position.y < 0) transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }


    public void Zoom(float zoomFactor)
    {
        float distance = (_target - transform.position).magnitude * zoomFactor;
        transform.Translate(Vector3.forward * distance, Space.Self);
    }
    //Put angles in a format from -180 to 180 (instead of unity's 0 - 360)
    public float FormatAngle(float angle) => angle < 180 ? angle : angle - 360;

    public void SetTarget(Vector3 position) => _target = position;

    public void ResetPosition() => transform.SetPositionAndRotation(_startTransform.position, _startTransform.rotation);


}