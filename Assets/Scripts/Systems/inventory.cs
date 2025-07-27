using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private void Awake()
    {
        Instance = this;
    }

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
                Debug.Log("Tenemos un:" + type);
            }
        }

        public void AddBig()
        {
            if (bigCount == 3)
            {
                return;
            }
            bigCount++;
            Debug.Log("Tenemos un:" + type);
        }

        public void RemoveBig()
        {
            if (bigCount == 0)
            {
                return;
            }
            bigCount--;
            Debug.Log("Tenemos un:" + type);
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

    // Método para simular recolección de objeto pequeño
    public void CollectSmall(ItemType type)
    {
        InventoryItem item = inventory[(int)type];
        item.AddSmall();
    }

    // Método para simular recolección de objeto grande
    public void CollectBig(ItemType type)
    {
        InventoryItem item = inventory[(int)type];
        item.AddBig();
    }

    // Método para combinar objetos grandes
    public void CombineObjects(ItemType type1, ItemType type2)
    {
        if (inventory[(int)type1].bigCount <= 0 || inventory[(int)type2].bigCount <= 0)
        {
            Debug.Log("No hay suficientes objetos grandes para combinar.");
            return;
        }

        // Disminuye uno de cada uno
        inventory[(int)type1].bigCount--;
        inventory[(int)type2].bigCount--;
    }
}
