using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingSelector : MonoBehaviour
{
    public GameObject buildingOptionsPanel;
    private GameObject selectedBuilding;
    private Vector3 originalPosition;
    private LayerMask originalLayerMask;
    private bool isMoving = false;
    public Material validMat, invalidMat; // materials for feedback

    void Update()
    {
        //Building Selector
        if (!isMoving && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Building"))
                {
                    SelectBuilding(hit.collider.gameObject);
                }
                else
                {
                    Deselect();
                }
            }
        }
        //Building move mechanic
        if (isMoving && selectedBuilding != null)
        {
            // Move building to mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Ground at y = 0

            Vector3 mouseWorldPos = selectedBuilding.transform.position;

            if (groundPlane.Raycast(ray, out float distance))
            {
                mouseWorldPos = ray.GetPoint(distance);
                selectedBuilding.transform.position = mouseWorldPos;
            }

            // Check if placement is valid at this position
            bool canPlace = CanPlaceBuilding(mouseWorldPos);

            // Visual feedback: color the building green or red
            var rend = selectedBuilding.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = (canPlace ? Color.green : Color.red);
            }

            // If left-click, attempt to finalize placement
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (canPlace)
                {
                    // Place building: restore NoBuild layer and exit move mode
                    selectedBuilding.layer = originalLayerMask;
                    isMoving = false;
                }
                else
                {
                    // Invalid position: snap back to original
                    selectedBuilding.transform.position = originalPosition;
                    selectedBuilding.layer = originalLayerMask;
                    isMoving = false;
                }
                // Show the UI panel again (or clear selection)
                buildingOptionsPanel.SetActive(true);
            }
        }
    }

    void SelectBuilding(GameObject building)
    {
        selectedBuilding = building;
        originalPosition = building.transform.position;
        originalLayerMask = building.layer;
        buildingOptionsPanel.SetActive(true);
    }

    public void Deselect()
    {
        selectedBuilding = null;
        buildingOptionsPanel.SetActive(false);
    }

    public void OnRotateButtonClicked()
    {
        if (selectedBuilding != null)
        {
            selectedBuilding.transform.Rotate(0f, 90f, 0f, Space.World);
        }
    }

    public void OnMoveButtonClicked()
    {
        if (selectedBuilding == null) { 
            return; 
        }
        isMoving = true;
        buildingOptionsPanel.SetActive(false);
        // Prevent building from blocking its own placement with nobuild
        selectedBuilding.layer = LayerMask.NameToLayer("Default");
    }

    public void OnDeleteButtonClicked()
    {
        if (selectedBuilding != null)
        {
            Destroy(selectedBuilding);
            Deselect();
        }
    }

    public bool CanPlaceBuilding(Vector3 position)
    {
        // Instantiate temporarily at target position to get its size
        GameObject temp = Instantiate(selectedBuilding, position, Quaternion.identity);

        Vector3 halfExtents = temp.GetComponentInChildren<Renderer>().bounds.extents;

        // Check if colliders on no build layer overlap with the selected building
        Collider[] hits = Physics.OverlapBox(position, halfExtents, Quaternion.identity, LayerMask.GetMask("NoBuild"));

        Destroy(temp);

        return hits.Length == 0; // True if no blockers
    }

}
