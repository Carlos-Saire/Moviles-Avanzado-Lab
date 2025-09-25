using UnityEngine;

public class PlayerCustomizer : MonoBehaviour
{
    public void Update()
    {
        transform.GetChild(1);
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
