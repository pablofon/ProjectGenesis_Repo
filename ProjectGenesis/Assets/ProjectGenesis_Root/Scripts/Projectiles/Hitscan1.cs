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
        float distance = range;
        Vector3 endPos = transform.position + (Vector3)direction * range;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, range, collisionLayers + groundLayer);
        Debug.DrawRay(transform.position, direction, Color.blue, 2f);
        for (int i = 0; hits.Length > i && pierce > i && hits[i].collider.contactCaptureLayers != groundLayer; i++)
        {
            Health healthScript = hits[i].collider.GetComponent<Health>();
            if (healthScript != null) healthScript.Damage(-dmg);
            endPos = hits[i].point;
            distance = hits[i].distance;
        }

        transform.position += (endPos - transform.position).normalized * distance / 2;
        transform.localScale = new Vector2(distance, 1);
        Debug.DrawLine(transform.position, endPos, Color.cyan);
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
