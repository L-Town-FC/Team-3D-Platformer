using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(CapsuleCollider))]
public class HitboxGizmo : MonoBehaviour
{
    [Header("Visibility")]
    public bool show = true;
    public Color color = Color.yellow;

    [Header("Detail")]
    [Range(8, 64)] public int segments = 24;     // circle smoothness
    [Range(2, 16)] public int rings = 6;         // rings along the cylinder
    [Range(2, 16)] public int meridians = 6;     // vertical arcs around the capsule

    private CapsuleCollider cap;

    private void OnEnable() => cap = GetComponent<CapsuleCollider>();
    private void OnValidate() => cap = GetComponent<CapsuleCollider>();

    private void OnDrawGizmos()
    {
        if (!show)
            return;

        if (cap == null)
            cap = GetComponent<CapsuleCollider>();

        if (cap == null)
            return;

        Gizmos.color = color;

        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        DrawWireCapsuleDetailed(cap.center, cap.radius, cap.height, cap.direction, segments, rings, meridians);

        Gizmos.matrix = old;
    }

    private static void DrawWireCapsuleDetailed(
        Vector3 center,
        float radius,
        float height,
        int direction,
        int segments,
        int rings,
        int meridians
    )
    {
        // Axis the capsule is aligned to
        Vector3 axis;
        switch (direction)
        {
            case 0: axis = Vector3.right; break;
            case 1: axis = Vector3.up; break;
            default: axis = Vector3.forward; break;
        }

        // Two perpendicular axes to form circles
        OrthogonalBasis(axis, out Vector3 a, out Vector3 b);

        float cylinder = Mathf.Max(0f, height - 2f * radius);
        Vector3 top = center + axis * (cylinder * 0.5f);
        Vector3 bot = center - axis * (cylinder * 0.5f);

        // Rings around the cylinder (horizontal circles)
        for (int i = 0; i <= rings; i++)
        {
            float t = rings == 0 ? 0f : (float)i / rings;
            Vector3 ringCenter = Vector3.Lerp(bot, top, t);
            DrawCircle(ringCenter, a, b, radius, segments);
        }

        // Equator circles on each endcap (helps define the shape)
        DrawCircle(top, a, b, radius, segments);
        DrawCircle(bot, a, b, radius, segments);

        // Meridians: arcs running from bottom to top around the capsule
        for (int m = 0; m < meridians; m++)
        {
            float angle = (Mathf.PI * 2f) * (m / (float)meridians);
            Vector3 dir = (Mathf.Cos(angle) * a + Mathf.Sin(angle) * b);

            // Side lines along the cylinder
            Gizmos.DrawLine(bot + dir * radius, top + dir * radius);

            // Top hemisphere arc
            DrawHemisphereArc(top, axis, dir, radius, segments, true);

            // Bottom hemisphere arc
            DrawHemisphereArc(bot, axis, dir, radius, segments, false);
        }
    }

    private static void DrawCircle(Vector3 center, Vector3 a, Vector3 b, float radius, int segments)
    {
        Vector3 prev = center + a * radius;
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            float ang = t * Mathf.PI * 2f;
            Vector3 next = center + (Mathf.Cos(ang) * a + Mathf.Sin(ang) * b) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    // Draws an arc of a hemisphere in the plane spanned by axis & dir
    private static void DrawHemisphereArc(Vector3 capCenter, Vector3 axis, Vector3 dir, float radius, int segments, bool top)
    {
        // For the top hemisphere, we go from side up to pole; for bottom, side down to pole
        float start = 0f;
        float end = Mathf.PI * 0.5f;

        Vector3 prev = capCenter + dir * radius;

        for (int i = 1; i <= segments / 2; i++)
        {
            float t = i / (float)(segments / 2);
            float ang = Mathf.Lerp(start, end, t);

            Vector3 offset = dir * (Mathf.Cos(ang) * radius) + (top ? axis : -axis) * (Mathf.Sin(ang) * radius);
            Vector3 next = capCenter + offset;

            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    private static void OrthogonalBasis(Vector3 axis, out Vector3 a, out Vector3 b)
    {
        Vector3 temp = Mathf.Abs(axis.y) < 0.99f ? Vector3.up : Vector3.right;
        a = Vector3.Cross(axis, temp).normalized;
        b = Vector3.Cross(axis, a).normalized;
    }
}
