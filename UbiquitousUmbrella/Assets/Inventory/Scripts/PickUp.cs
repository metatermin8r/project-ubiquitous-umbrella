using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public HeldItem item = new HeldItem("Item Name", 1, 1);
    public GameObject pickUpUI;
    public Transform InteractorSource; //this is a reference to the player 
    public float InteractRange; //the distance from which you can pick up an object

    private void Start()
    {
        pickUpUI.SetActive(false);
    }

    private void Update()
    {
        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange))
        {
            pickUpUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                InventoryController.instance.AddNewItem(item);
                Destroy(gameObject);
                pickUpUI.SetActive(false);
            }
        }
        else
        {
            pickUpUI.SetActive(false);
        }
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    pickUpUI.SetActive(true);

    //   if (Input.GetKey(KeyCode.E))
    //    {
    //        if (other.CompareTag("Player"))
    //        {
    //            InventoryController.instance.AddNewItem(item);
    //            Destroy(gameObject);
    //        }
    //    }
    //}

    void OnTriggerExit(Collider other)
    {
        pickUpUI.SetActive(false);
    }
}
