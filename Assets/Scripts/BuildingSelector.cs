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
    private Material originalMaterial;
    private Color originalColor;

    void Update()
    {
        // Building selector
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
        // Building move mechanic
        if (isMoving && selectedBuilding != null)
        {
            // Move building to mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            Vector3 mouseWorldPos = selectedBuilding.transform.position;

            if (groundPlane.Raycast(ray, out float distance))
            {
                mouseWorldPos = ray.GetPoint(distance);
                selectedBuilding.transform.position = mouseWorldPos;
            }

            // Check if placement is valid at this position
            bool canPlace = CanPlaceBuilding(mouseWorldPos);

            // Colour the building green or red
            var rend = selectedBuilding.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = (canPlace ? Color.green : Color.red);
            }

            // Attempt to finalize placement
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (!canPlace)
                {
                    selectedBuilding.transform.position = originalPosition;
                } 
                selectedBuilding.layer = originalLayerMask;
                isMoving = false;
                Deselect();
            }
        }
    }

    void SelectBuilding(GameObject building)
    {
        // Prevent buildings not restoring colour when selecting a building while another one is already selected
        Deselect();

        selectedBuilding = building;
        originalPosition = building.transform.position;
        originalLayerMask = building.layer;

        // Change colour slightly so as to provide a sort of visual indicator that the building is selected
        Renderer rend = building.GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterial = rend.material;
            originalColor = rend.material.color;
            rend.material.color = originalColor * 0.7f; // darken it a bit
        }

        buildingOptionsPanel.SetActive(true);
    }

    public void Deselect()
    {
        if (selectedBuilding != null)
        {
            Renderer rend = selectedBuilding.GetComponent<Renderer>();
            if (rend != null && originalMaterial != null)
            {
                rend.material.color = originalColor; // restore original colour
            }
        }
        
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
