using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int rotateSpeed;
    Transform transform;
    void Start()
    {
        transform = this.GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotateSpeed));
    }
}
