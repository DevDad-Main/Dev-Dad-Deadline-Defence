using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BuildArea : MonoBehaviour
{
    private BoxCollider2D buildZone;

    private void Awake()
    {
        buildZone = GetComponent<BoxCollider2D>();
        buildZone.isTrigger = true; // Make sure it's not a physics collider
    }

    public bool IsWithinBounds(Vector2 position)
    {
        return buildZone.OverlapPoint(position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
