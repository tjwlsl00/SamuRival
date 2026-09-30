using UnityEngine;
using System.Collections;

public class StunStar : MonoBehaviour
{
    [SerializeField] ParticleSystem StunStarPrefab;
    
    public void PlayStunEffect()
    {
        StunStarPrefab.gameObject.SetActive(true);
        StunStarPrefab.Play();

        StartCoroutine(StopStunEffect(3.5f));
    }

    IEnumerator StopStunEffect(float time)
    {
        yield return new WaitForSeconds(time);
        StunStarPrefab.gameObject.SetActive(false);
        StunStarPrefab.Stop();
    }
}
