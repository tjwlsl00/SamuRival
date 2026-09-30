using UnityEngine;

public class EntryLine : MonoBehaviour
{
    // 변수 관련 
    [SerializeField] float targetSpeed;
    // bool 
    private bool isRaised = false;

    void OnTriggerEnter(Collider other)
    {
        if (isRaised) return;

        if (other.CompareTag("Red") || other.CompareTag("Blue"))
        {
            isRaised = true;
            FindSnowball();
        }
    }

    private void FindSnowball()
    {
        GameObject snowball = GameObject.FindGameObjectWithTag("Snowball");

        if (snowball != null)
        {
            ChangeSnowballSpeed(snowball);
        }
    }

    private void ChangeSnowballSpeed(GameObject snowball)
    {
        SnowEscape_Snowball snowEscape_Snowball = snowball.GetComponent<SnowEscape_Snowball>();

        if (snowEscape_Snowball != null)
        {
            Debug.Log("스노우볼 속도 변환");

            snowEscape_Snowball.moveSpeed = targetSpeed;
        }
        Destroy(this);
    }
}