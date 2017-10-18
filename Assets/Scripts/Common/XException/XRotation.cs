using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class XRotation : MonoBehaviour
{
    [SerializeField]
    private float speed = 1;

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(new Vector3(0, 12 + speed, 0));
        speed++;
    }

}
