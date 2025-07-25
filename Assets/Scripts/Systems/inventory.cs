using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    // public InputAction windAction;
    // public InputAction rayAction;
    // public InputAction iceAction;
    public enum ItemType { Wind, Ice, Ray }

    // Estructura de un objeto del inventario
    [System.Serializable]
    public class InventoryItem
    {
        public ItemType type;
        public int smallCount = 0;  // Cantidad de objetos pequeños
        public int bigCount = 0;    // Cantidad de objetos grandes

        public void AddSmall()
        {
            if (bigCount == 3 && smallCount == 5)
            {
                return;
            }
            smallCount++;
            if (smallCount >= 5)
            {
                smallCount -= 5;
                bigCount++;
                Debug.Log("¡Se creó un:" + type);
            }

        }
    }

    public InventoryItem[] inventory = new InventoryItem[3];

    void Start()
    {
        // Inicializamos los 3 tipos
        inventory[0] = new InventoryItem { type = ItemType.Wind };
        inventory[1] = new InventoryItem { type = ItemType.Ice };
        inventory[2] = new InventoryItem { type = ItemType.Ray };
    }

    //Parallel depurar
    // void OnEnable()
    // {
    //     windAction.Enable();
    //     rayAction.Enable();
    //     iceAction.Enable();

    //     windAction.performed += ctx => CollectSmall(ItemType.Wind);
    //     rayAction.performed += ctx => CollectSmall(ItemType.Ray);
    //     iceAction.performed += ctx => CollectSmall(ItemType.Ice);
    // }

    // void OnDisable()
    // {
    //     windAction.Disable();
    //     rayAction.Disable();
    //     iceAction.Disable();
    // }

    // Método para simular recolección de objeto pequeño
    public void CollectSmall(ItemType type)
    {
        InventoryItem item = inventory[(int)type];
        item.AddSmall();
        Debug.Log($"Recolectaste un objeto pequeño de tipo {type}. Total pequeños: {item.smallCount}, grandes: {item.bigCount}");
    }


}
