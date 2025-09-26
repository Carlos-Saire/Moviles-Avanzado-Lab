using UnityEngine;
using Unity.Netcode;

public class PlayerCustomizer : NetworkBehaviour
{

    public void Update()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }
}

public class PlayerCustomizationData
{
    public int bodyIndex;
    public int eyesIndex;
    public int glovesIndex;
    public int headIndex;
    public int mouthIndex;
    public int tailIndex;
} 
