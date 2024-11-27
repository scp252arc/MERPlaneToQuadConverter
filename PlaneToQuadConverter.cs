using UnityEngine;

public class PlaneToQuadConverter : MonoBehaviour
{
    [Header("Prefab Settings")]
    [Tooltip("Select Prefab to replace Plane.")]
    public GameObject quadPrefab;

    [ContextMenu("Convert Planes to Quads")]
    public void ConvertPlanesToQuads()
    {
        if (quadPrefab == null)
        {
            Debug.LogError("Quad Prefab not set! Set it in inspector.");
            return;
        }

        GameObject[] planes = GameObject.FindGameObjectsWithTag("Plane");

        foreach (var plane in planes)
        {
            MeshFilter meshFilter = plane.GetComponent<MeshFilter>();

            if (meshFilter != null && meshFilter.sharedMesh.name == "Plane")
            {
                GameObject originalPlane = plane;

                Transform parent = originalPlane.transform.parent;
                Vector3 position = originalPlane.transform.position;
                Quaternion rotation = originalPlane.transform.rotation;
                Vector3 scale = originalPlane.transform.localScale;

                GameObject quad = Instantiate(quadPrefab, position, rotation);

                quad.transform.rotation = rotation * Quaternion.Euler(90, 0, 0);
                quad.transform.localScale = new Vector3(scale.x * 10, scale.z * 10, 1);
                quad.transform.SetParent(parent);

                Component[] quadComponents = quad.GetComponents<Component>();
                foreach (var component in quadComponents)
                {
                    if (component.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        DestroyImmediate(component);
                    }
                }

                Component[] planeComponents = originalPlane.GetComponents<Component>();
                foreach (var component in planeComponents)
                {
                    if (component is Transform)
                        continue;

                    Component newComponent = quad.AddComponent(component.GetType());
                    System.Reflection.FieldInfo[] fields = component.GetType().GetFields(
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

                    foreach (var field in fields)
                    {
                        field.SetValue(newComponent, field.GetValue(component));
                    }
                }

                DestroyImmediate(originalPlane);
            }
        }
    }
}
