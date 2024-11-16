using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject overlayPanel;     // Assign the Overlay Panel
    public GameObject inventoryPanel;  // Assign the Inventory Panel

    private bool isInventoryOpen = false;

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        // Show or hide the overlay and inventory panels
        overlayPanel.SetActive(isInventoryOpen);
        inventoryPanel.SetActive(isInventoryOpen);
    }
}
