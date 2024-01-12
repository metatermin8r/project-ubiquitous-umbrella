[System.Serializable]

public class HeldItem
{
    public string name;
    public int count;
    public int id;

    public HeldItem(string itemName, int itemCount, int itemId) 
    {
        name = itemName;
        count = itemCount;   
        id = itemId;
    }
}
