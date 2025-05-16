using UnityEngine;

public class PlayerFixer : MonoBehaviour
{
    public bool hasScrewdriver { get; private set; } = false;

    public void GiveScrewdriver()
    {
        hasScrewdriver = true;
        Debug.Log("[Player] Picked up screwdriver");
    }
}
