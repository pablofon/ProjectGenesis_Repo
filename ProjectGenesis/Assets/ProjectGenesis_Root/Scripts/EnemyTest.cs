using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    private Coroutine flash;

    void Awake()
    {
        sr.material = new Material(sr.material);
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashColor(Color.red, 2f, .5f));
    }

    private IEnumerator FlashColor(Color color, float duration, float amount)
    {
        sr.material.color = color;
        float elapsedTime = 0f;
        while (elapsedTime < duration + .1f)
        {
            elapsedTime += Time.deltaTime;
            float mult = Mathf.Lerp(amount, 0, elapsedTime / .2f);

            mult = Mathf.Abs(mult);

            // Set the multiplier in the shader
            sr.material.SetFloat("_Multiplier", mult);
            yield return null; // Wait for the next frame
        }
        sr.material.SetFloat("_Multiplier", 0);
        Debug.Log("coloreset");
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
