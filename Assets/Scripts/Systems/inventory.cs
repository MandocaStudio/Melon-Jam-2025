using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

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

        public TextMeshProUGUI AmountBig;

        public GameObject parent;
        [SerializeField] private GameObject[] children;

        public void InitializeChildren()
        {
            int childCount = parent.transform.childCount;

            children = new GameObject[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = parent.transform.GetChild(i).gameObject;

            }
        }

        public void ShowOnlyChildren(int amount)
        {
            if (children == null || children.Length == 0)
                return;

            for (int i = 0; i < children.Length; i++)
            {
                children[i].SetActive(i < amount);
            }
        }


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

                AmountBig.text = "x" + bigCount;

                Debug.Log("Tenemos un:" + type);
            }

            ShowOnlyChildren(smallCount);
        }

        public void AddBig()
        {
            Debug.Log("alo?");

            if (bigCount >= 3)
            {
                return;
            }
            bigCount++;

            AmountBig.text = "x" + bigCount;

            Debug.Log("Tenemos un: grandote" + type);
        }

        public void RemoveBig()
        {
            if (bigCount == 0)
            {
                return;
            }
            bigCount--;

            AmountBig.text = "x" + bigCount;

            Debug.Log("Tenemos un:" + type);
        }
    }

    public InventoryItem[] inventory = new InventoryItem[3];

    void Start()
    {

        foreach (var item in inventory)
        {
            item.InitializeChildren();
        }
        // Inicializamos los 3 tipos
        // inventory[0] = new InventoryItem { type = ItemType.Wind };
        // inventory[1] = new InventoryItem { type = ItemType.Ice };
        // inventory[2] = new InventoryItem { type = ItemType.Ray };
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
        Debug.Log("Tenemos un:" + (int)type);

        InventoryItem item = inventory[(int)type];

        item.AddBig();
    }

    // Método para combinar objetos grandes
    public void CombineObjects(ItemType type1, ItemType type2)
    {
        Debug.Log("combinando.....");

        if (inventory[(int)type1].bigCount <= 0 || inventory[(int)type2].bigCount <= 0)
        {

            Debug.Log("No hay suficientes objetos grandes para combinar.");
            return;
        }

        InventoryItem item1 = inventory[(int)type1];
        InventoryItem item2 = inventory[(int)type2];

<<<<<<< Updated upstream
        item1.RemoveBig();
        item2.RemoveBig();


        // Disminuye uno de cada uno
        // inventory[(int)type1].bigCount--;
        // inventory[(int)type2].bigCount--;
=======
>>>>>>> Stashed changes
    }


}
