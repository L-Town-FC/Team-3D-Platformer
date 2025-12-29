using UnityEngine;

public class IndicatorBelowObject : MonoBehaviour
{
    [SerializeField]
    LayerMask playerMask;
    [SerializeField]
    Transform indicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 100f, playerMask))
        {
            print("here");
            indicator.position = hitInfo.point + Vector3.up * 0.01f;
        }
    }
}
