using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public GameObject Target { get; set; }

    private readonly Vector3 c_PositionOffset = new Vector3(0f, -2f, 8f);

    void Update()
    {
        if (!Target)
            return;

        Quaternion rotation = Quaternion.Euler(0, Target.transform.eulerAngles.y, 0);
        transform.position = Target.transform.position - (rotation * c_PositionOffset);

        transform.LookAt(Target.transform.position);
    }
}
