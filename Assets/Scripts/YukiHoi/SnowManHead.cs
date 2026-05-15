using UnityEngine;

public class SnowManHead : MonoBehaviour
{
    [SerializeField] GameObject SMHead;

    public void OnOffSnowManHead(bool isVisible)
    {
        SMHead.SetActive(isVisible);
    }
}
