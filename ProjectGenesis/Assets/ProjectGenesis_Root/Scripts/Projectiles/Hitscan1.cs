using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Hitscan1 : MonoBehaviour
{
    [SerializeField] private float trailDuration;
    [SerializeField] private float trailSize;
    [SerializeField] private AnimationCurve trailCurve;
    [SerializeField] private LayerMask groundLayer;

    public IEnumerator Hit(Vector2 direction, float range, LayerMask collisionLayers, int pierce, float dmg, float knockback)
    {
        Vector3 endPos = Vector3.zero;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, range, collisionLayers + groundLayer);
        Debug.DrawRay(transform.position, direction, Color.blue, 2f);
        for (int i = 0; hits.Length > i && pierce > i && hits[i].collider.contactCaptureLayers != groundLayer; i++)
        {
            hits[i].collider.GetComponent<Health>().ChangeHealth(dmg);
            endPos = hits[i].point;
        }

        //transform.position += (transform.position - endPos).normalized * range / 2;
        transform.localScale = new Vector2(Vector3.Distance(endPos, transform.position), 1);
        Debug.DrawLine(transform.position, endPos, Color.cyan);
        Debug.Log(transform.position + "trPos" + endPos + "endPos");
        float elapsedTime = 0;
        while (elapsedTime < trailDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = new Vector2(transform.localScale.x, trailSize * trailCurve.Evaluate(elapsedTime/trailDuration));
            yield return null;
        }

        Destroy(gameObject);
        yield return null;
    }
}
