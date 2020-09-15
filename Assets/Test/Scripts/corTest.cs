using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corTest : MonoBehaviour
{
    public float timer = 0;
    public float speed = 5f;
    public Transform cube;

    // Start is called before the first frame update
    void Start()
    {
        IEnumerator t = Test();
        StartCoroutine(t);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.DrawLine(cube.position, cube.position + cube.forward ,Color.black, 5f);
            timer = 0;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(h, 0,v)*Time.deltaTime*speed, Space.Self);
    }

    IEnumerator Test()
    {
        
        while (true)
        {
            if (timer < 3f)
                timer += Time.deltaTime;
            else if (timer < 5f)
            {
                Debug.DrawLine(cube.position, cube.position + cube.forward * 10, Color.black, 5f);
                timer = 6f;
            }

            yield return null;
        }
    }
}
