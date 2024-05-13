using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosCadrs : MonoBehaviour
{
    [SerializeField]
    private Vector3 GizScale = new Vector3(15, 16,1);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, GizScale);
    }
}
