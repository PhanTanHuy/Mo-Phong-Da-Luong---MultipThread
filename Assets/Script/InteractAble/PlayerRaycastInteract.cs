using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycastInteract : MonoBehaviour
{
    public Transform cameraTransform;
    public LayerMask layerMaskInteract;
    public float rayDistance = 5f;

    private Outline currentOutline; 

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, rayDistance, layerMaskInteract))
        {
            InteractAble interactAble = hit.collider.GetComponentInParent<InteractAble>();
            if (interactAble != null)
            {
                Outline outline = hit.collider.GetComponentInParent<Outline>();

                if (outline != currentOutline)
                {
                    DisableCurrentOutline();
                    if (outline != null)
                    {
                        outline.enabled = true;
                        currentOutline = outline;
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    interactAble.BaseInteract(transform);
                }
            }
            else
            {
                DisableCurrentOutline();
            }
        }
        else
        {
            DisableCurrentOutline();
        }
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * rayDistance, Color.yellow);

    }

    private void DisableCurrentOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }
}
