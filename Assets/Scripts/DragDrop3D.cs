using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop3D : MonoBehaviour
{
    public GameObject handPoint;
    private GameObject selectedObject = null;
    void Update()
    {
        if (selectedObject != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Objeto soltado");
                selectedObject.GetComponent<Rigidbody>().useGravity = true;
                selectedObject.GetComponent<Rigidbody>().isKinematic = false;
                selectedObject.transform.SetParent(null);
                selectedObject = null;
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Objeto"))
        {
            Debug.Log("Objeto en rango");
            if (Input.GetMouseButton(0) && selectedObject == null)
            {
                Debug.Log("Objeto agarrado");
                other.GetComponent<Rigidbody>().useGravity = false;
                other.GetComponent<Rigidbody>().isKinematic = true;
                other.transform.position = handPoint.transform.position;
                other.gameObject.transform.SetParent(handPoint.gameObject.transform);
                selectedObject = other.gameObject;
            }
        }
    }
}


