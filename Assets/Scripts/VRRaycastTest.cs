using UnityEngine;
using UnityEngine.InputSystem;

public class VRRaycastTest : MonoBehaviour
{
    public float rayLength = 10f;
    private GameObject currentHit;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            currentHit = hit.collider.gameObject;
            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

            // Nuevo Input System (ejemplo: clic izquierdo del mouse o trigger del XR Controller)
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                VRButton3D button = currentHit.GetComponent<VRButton3D>();
                if (button != null)
                {
                    button.OnPressed();
                }
            }
        }
        else
        {
            currentHit = null;
        }
    }
}
