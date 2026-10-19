using UnityEngine;

public class SnowManHead : MonoBehaviour
{
    [SerializeField] GameObject SMHead;

    public void ToggleSnowHead()
    {
        SMHead.SetActive(!SMHead.activeSelf);
    }
}
