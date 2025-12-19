using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Vector3 rotationAxis = Vector3.right;
    [SerializeField] private float rotationSpeed = 90f; // degrees per second

    private void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
