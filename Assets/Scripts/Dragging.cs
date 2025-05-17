using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragging : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject prefabSpawn;
    private GameObject dragIcon;
    private Image dragIconImage;
    private RectTransform dragTransform;
    public LayerMask noBuildLayer;

    //When first clicking the draggable icon
    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) {
            Debug.LogError("Canvas not found in parent!");
            return;
        }

        dragIcon = new GameObject("icon");
        dragIcon.transform.SetParent(canvas.transform, false);

        dragIconImage = dragIcon.AddComponent<Image>();
        dragIconImage.sprite = GetComponent<Image>().sprite;
        dragIconImage.SetNativeSize();

        var group = dragIcon.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        dragTransform = dragIcon.GetComponent<RectTransform>();
    }

    //While dragging the icon
    public void OnDrag(PointerEventData eventData)
    {
        dragTransform.position = eventData.position;
        //Check if placement is valid or invalid
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 spawnPosition = hit.point;
            spawnPosition.y = 0.001f;

            Vector3 prefabSize = prefabSpawn.transform.localScale;

            if (CanPlaceBuilding(spawnPosition, prefabSize))
            {
                dragIconImage.color = Color.green;  // Valid
            }
            else
            {
                dragIconImage.color = Color.red;    // Invalid
            }
        }
        else
        {
            // if there is no terrain hit = invalid placement
            dragIconImage.color = Color.red;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(dragIcon);

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 spawnPosition = hit.point;
                spawnPosition.y = 0.001f;

                //Get prefab size to check if it'll overlap with nobuild area
                Vector3 prefabSize = prefabSpawn.transform.localScale; 

                if (CanPlaceBuilding(spawnPosition, prefabSize))
                {
                    GameObject spawned = Instantiate(prefabSpawn, spawnPosition, Quaternion.identity);
                    //Give buildings the nobuild layer to prevent placing buildings inside of other buildings
                    spawned.layer = LayerMask.NameToLayer("NoBuild");
                    spawned.transform.SetParent(GameObject.Find("Buildings").transform);

                    // Transition effect (scale up)
                    spawned.transform.localScale = Vector3.zero;
                    LeanTween.scale(spawned, prefabSize, 0.3f).setEaseOutBack();
                }
                else
                {
                    Debug.Log("Cannot place building here - blocked by NoBuild area.");
                }
            }
        }
    }

    private bool CanPlaceBuilding(Vector3 position, Vector3 prefabSize)
    {
        // Instantiate prefab temporarily at target position to get its size
        GameObject temp = Instantiate(prefabSpawn, position, Quaternion.identity);

        Renderer tempRenderer = temp.GetComponentInChildren<Renderer>();
        if (tempRenderer == null)
        {
            Destroy(temp);
            return true; // No renderer found, assume placeable
        }

        Vector3 halfExtents = tempRenderer.bounds.extents;

        // Check if colliders on noBuildLayer overlap with prefab bounds
        Collider[] hits = Physics.OverlapBox(position, halfExtents, Quaternion.identity, noBuildLayer);

        Destroy(temp);

        return hits.Length == 0; // True if no blockers
    }
}
