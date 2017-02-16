using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject m_Target;
    Vector3 m_Offset;

    void Start()
    {
        m_Offset = m_Target.transform.position - transform.position;
    }

    private void FixedUpdate()
    {
        float currentAngle = transform.eulerAngles.y;
        float desiredAngle = m_Target.transform.eulerAngles.y;

        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.position = m_Target.transform.position - (rotation * m_Offset);

        transform.LookAt(m_Target.transform);
    }
}
