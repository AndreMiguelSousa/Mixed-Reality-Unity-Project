using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragging : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject prefabSpawn;
    private GameObject dragIcon;
    private RectTransform dragTransform;

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

        var image = dragIcon.AddComponent<Image>();
        image.sprite = GetComponent<Image>().sprite;
        image.SetNativeSize();

        var group = dragIcon.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        dragTransform = dragIcon.GetComponent<RectTransform>();
    }

    //Follow the cursor with the icon
    public void OnDrag(PointerEventData eventData)
    {
        dragTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(dragIcon);

        // Place prefab into 3D world
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 spawnPosition = hit.point;
                spawnPosition.y = 0.001f;
                GameObject spawned = Instantiate(prefabSpawn, spawnPosition, Quaternion.identity);

                // Transition effect (scale up)
                Vector3 targetScale = prefabSpawn.transform.localScale;
                spawned.transform.localScale = Vector3.zero;
                LeanTween.scale(spawned, targetScale, 0.3f).setEaseOutBack();
            }
        }
    }
}
