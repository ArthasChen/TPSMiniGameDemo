using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public Transform player;
    private Vector3 offset;
    private float length;
    private float height;
    public float width;

    public float rotateSpeed = 15;
    public float moveSpeed = 20;

    private void Awake()
    {
        height = player.GetComponent<CapsuleCollider>().height;
        offset = player.position - transform.position;
        length = offset.magnitude;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.position - offset, moveSpeed * Time.deltaTime);
        transform.LookAt(player.position + new Vector3(0,height*0.5f,0) + transform.right * width);

        float h = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        float v = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        transform.RotateAround(player.position, Vector3.up, h);
        transform.RotateAround(player.position, transform.right, -v);

        offset = (player.position - transform.position).normalized * length;

        float ph = Input.GetAxis("Horizontal");
        float pv = Input.GetAxis("Vertical");
        player.Translate(new Vector3(ph, 0, pv)*2*Time.deltaTime);
        Vector3 dir = transform.forward;
        dir.y = 0;
        player.rotation = Quaternion.Lerp(player.rotation, Quaternion.LookRotation(dir), 15 * Time.deltaTime);
    }
}
