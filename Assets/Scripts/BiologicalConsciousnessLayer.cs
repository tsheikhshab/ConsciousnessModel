using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiologicalConsciousnessLayer : MonoBehaviour
{
    [Header("Biological Boundary")]
    public float boundaryRadius = 10f;
    public Material boundaryMaterial;
    public Color boundaryColor = new Color(0.2f, 0.4f, 0.8f, 0.3f);
    
    [Header("Resonance Rings")]
    public int ringCount = 3;
    public float ringThickness = 0.1f;
    public Material ringMaterial;
    public Color ringColor = new Color(0.3f, 0.5f, 0.9f, 0.4f);
    
    [Header("Biological Nodes")]
    public int nodeCount = 24;
    public float nodeSize = 0.2f;
    public Material nodeMaterial;
    public Color nodeColor = new Color(0.7f, 0.8f, 1.0f, 0.8f);
    
    [Header("Self-Reinforcing Loops")]
    public int loopCount = 3;
    public float loopRadius = 2.5f;
    public int loopSegments = 32;
    public Material loopMaterial;
    public Color loopColor = new Color(0.4f, 0.6f, 0.9f, 0.5f);
    
    [Header("Connections")]
    public int connectionCount = 8;
    public Material connectionMaterial;
    public Color connectionColor = new Color(0.4f, 0.6f, 0.9f, 0.5f);
    
    [Header("Animation")]
    public float rotationSpeed = 5f;
    public float pulsateSpeed = 1f;
    public float pulsateAmount = 0.1f;
    
    private GameObject boundary;
    private GameObject[] rings;
    private GameObject[] nodes;
    private GameObject[] loops;
    private GameObject[] connections;
    
    private void Start()
    {
        CreateBoundary();
        CreateRings();
        CreateNodes();
        CreateLoops();
        CreateConnections();
    }
    
    private void Update()
    {
        AnimateBoundary();
        AnimateRings();
        AnimateNodes();
        AnimateLoops();
        AnimateConnections();
    }
    
    private void CreateBoundary()
    {
        boundary = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        boundary.name = "BiologicalBoundary";
        boundary.transform.SetParent(transform);
        boundary.transform.localScale = new Vector3(boundaryRadius * 2, boundaryRadius * 2, boundaryRadius * 2);
        
        Renderer renderer = boundary.GetComponent<Renderer>();
        renderer.material = boundaryMaterial != null ? boundaryMaterial : new Material(Shader.Find("Standard"));
        renderer.material.color = boundaryColor;
        
        // Make it wireframe-like by using a shader or setting the material properties
        renderer.material.SetFloat("_Mode", 3); // Transparent mode
        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        renderer.material.SetInt("_ZWrite", 0);
        renderer.material.DisableKeyword("_ALPHATEST_ON");
        renderer.material.EnableKeyword("_ALPHABLEND_ON");
        renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        renderer.material.renderQueue = 3000;
        
        // Remove collider for performance
        Destroy(boundary.GetComponent<Collider>());
    }
    
    private void CreateRings()
    {
        rings = new GameObject[ringCount];
        
        for (int i = 0; i < ringCount; i++)
        {
            GameObject ring = new GameObject("Ring_" + i);
            ring.transform.SetParent(transform);
            
            // Calculate radius based on the index
            float radius = boundaryRadius * (0.8f - i * 0.15f);
            
            // Create a torus for the ring
            MeshFilter meshFilter = ring.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = ring.AddComponent<MeshRenderer>();
            
            meshFilter.mesh = CreateTorusMesh(radius, ringThickness, 32, 12);
            
            meshRenderer.material = ringMaterial != null ? ringMaterial : new Material(Shader.Find("Standard"));
            meshRenderer.material.color = ringColor;
            
            // Make it transparent
            meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            meshRenderer.material.SetInt("_ZWrite", 0);
            meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
            meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            meshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            meshRenderer.material.renderQueue = 3000;
            
            rings[i] = ring;
        }
    }
    
    private void CreateNodes()
    {
        nodes = new GameObject[nodeCount];
        
        for (int i = 0; i < nodeCount; i++)
        {
            GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            node.name = "Node_" + i;
            node.transform.SetParent(transform);
            
            // Calculate position on a sphere
            float angle = (i / (float)nodeCount) * Mathf.PI * 2;
            float radius = boundaryRadius * 0.7f + Mathf.Sin(i * 2) * boundaryRadius * 0.1f;
            
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            float z = Mathf.Sin(i * 0.5f) * boundaryRadius * 0.3f;
            
            node.transform.localPosition = new Vector3(x, y, z);
            
            // Random scale variation
            float scale = nodeSize * (0.8f + Random.value * 0.4f);
            node.transform.localScale = new Vector3(scale, scale, scale);
            
            // Material
            Renderer renderer = node.GetComponent<Renderer>();
            renderer.material = nodeMaterial != null ? nodeMaterial : new Material(Shader.Find("Standard"));
            renderer.material.color = nodeColor;
            
            // Make it glow or use emission
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", nodeColor * 0.5f);
            
            // Remove collider for performance
            Destroy(node.GetComponent<Collider>());
            
            nodes[i] = node;
        }
    }
    
    private void CreateLoops()
    {
        loops = new GameObject[loopCount];
        
        for (int i = 0; i < loopCount; i++)
        {
            GameObject loop = new GameObject("Loop_" + i);
            loop.transform.SetParent(transform);
            
            // Position around the sphere
            float angle = (i / (float)loopCount) * Mathf.PI * 2;
            float x = Mathf.Cos(angle) * boundaryRadius * 0.5f;
            float y = Mathf.Sin(angle) * boundaryRadius * 0.5f;
            float z = 0f;
            
            loop.transform.localPosition = new Vector3(x, y, z);
            
            // Create loop mesh
            MeshFilter meshFilter = loop.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = loop.AddComponent<MeshRenderer>();
            
            meshFilter.mesh = CreateCircleMesh(loopRadius, loopSegments);
            
            meshRenderer.material = loopMaterial != null ? loopMaterial : new Material(Shader.Find("Standard"));
            meshRenderer.material.color = loopColor;
            
            // Make it transparent and wireframe-like
            meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            meshRenderer.material.SetInt("_ZWrite", 0);
            meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
            meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            meshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            meshRenderer.material.renderQueue = 3000;
            
            // Random rotation
            loop.transform.rotation = Random.rotation;
            
            loops[i] = loop;
        }
    }
    
    private void CreateConnections()
    {
        connections = new GameObject[connectionCount];
        
        for (int i = 0; i < connectionCount; i++)
        {
            GameObject connection = new GameObject("Connection_" + i);
            connection.transform.SetParent(transform);
            
            LineRenderer lineRenderer = connection.AddComponent<LineRenderer>();
            
            // Set up line renderer properties
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = connectionMaterial != null ? connectionMaterial : new Material(Shader.Find("Standard"));
            lineRenderer.material.color = connectionColor;
            
            // Make the line dashed with texture
            lineRenderer.textureMode = LineTextureMode.Tile;
            
            // Create points for a curve
            int startIdx = i % nodes.Length;
            int endIdx = (i + 2) % nodes.Length;
            
            Vector3 startPos = nodes[startIdx].transform.position;
            Vector3 endPos = nodes[endIdx].transform.position;
            
            // Add curve control point
            Vector3 midPoint = Vector3.Lerp(startPos, endPos, 0.5f);
            Vector3 dirToCenterFromMid = (Vector3.zero - midPoint).normalized;
            midPoint += dirToCenterFromMid * boundaryRadius * 0.3f;
            
            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, midPoint);
            lineRenderer.SetPosition(2, endPos);
            
            connections[i] = connection;
        }
    }
    
    private void AnimateBoundary()
    {
        if (boundary == null) return;
        
        // Gentle pulsation
        float pulse = 1f + Mathf.Sin(Time.time * pulsateSpeed) * pulsateAmount;
        boundary.transform.localScale = new Vector3(
            boundaryRadius * 2 * pulse,
            boundaryRadius * 2 * pulse,
            boundaryRadius * 2 * pulse
        );
    }
    
    private void AnimateRings()
    {
        for (int i = 0; i < rings.Length; i++)
        {
            // Rotate rings at different speeds and directions
            rings[i].transform.Rotate(
                rotationSpeed * (i % 2 == 0 ? 1 : -1) * 0.01f,
                rotationSpeed * 0.02f,
                rotationSpeed * (i % 2 == 0 ? -1 : 1) * 0.015f
            );
            
            // Pulse rings
            float pulse = 1f + Mathf.Sin(Time.time * pulsateSpeed + i * 0.5f) * pulsateAmount * 0.5f;
            float radius = boundaryRadius * (0.8f - i * 0.15f);
            rings[i].transform.localScale = new Vector3(pulse, pulse, pulse);
        }
    }
    
    private void AnimateNodes()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            // Gently move nodes around their positions
            float angleOffset = Time.time * 0.2f + i * 0.1f;
            float radiusOffset = Mathf.Sin(Time.time + i) * 0.1f;
            
            float angle = (i / (float)nodeCount) * Mathf.PI * 2 + angleOffset;
            float radius = boundaryRadius * (0.7f + radiusOffset) + Mathf.Sin(i * 2) * boundaryRadius * 0.1f;
            
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            float z = Mathf.Sin(i * 0.5f + Time.time * 0.3f) * boundaryRadius * 0.3f;
            
            // Smooth movement
            nodes[i].transform.localPosition = Vector3.Lerp(
                nodes[i].transform.localPosition,
                new Vector3(x, y, z),
                Time.deltaTime * 2f
            );
            
            // Pulse size
            float pulse = 1f + Mathf.Sin(Time.time * 3f + i * 0.5f) * 0.2f;
            float scale = nodeSize * (0.8f + Random.value * 0.4f) * pulse;
            nodes[i].transform.localScale = new Vector3(scale, scale, scale);
        }
    }
    
    private void AnimateLoops()
    {
        for (int i = 0; i < loops.Length; i++)
        {
            // Rotate loops to simulate flow
            loops[i].transform.Rotate(Vector3.forward, rotationSpeed * (i % 2 == 0 ? 1 : -1) * 0.5f);
            
            // Slight bobbing and breathing
            float pulse = 1f + Mathf.Sin(Time.time * pulsateSpeed + i * 0.7f) * pulsateAmount;
            loops[i].transform.localScale = new Vector3(pulse, pulse, pulse);
        }
    }
    
    private void AnimateConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            LineRenderer lineRenderer = connections[i].GetComponent<LineRenderer>();
            
            // Update connection endpoints based on node positions
            int startIdx = i % nodes.Length;
            int endIdx = (i + 2) % nodes.Length;
            
            Vector3 startPos = nodes[startIdx].transform.position;
            Vector3 endPos = nodes[endIdx].transform.position;
            
            // Update curve control point
            Vector3 midPoint = Vector3.Lerp(startPos, endPos, 0.5f);
            Vector3 dirToCenterFromMid = (Vector3.zero - midPoint).normalized;
            
            // Add some variation to the curve
            float curveAmount = boundaryRadius * 0.3f * (1f + Mathf.Sin(Time.time + i) * 0.2f);
            midPoint += dirToCenterFromMid * curveAmount;
            
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, midPoint);
            lineRenderer.SetPosition(2, endPos);
            
            // Pulse width
            float pulse = 0.05f + Mathf.Sin(Time.time * 2f + i * 0.3f) * 0.02f;
            lineRenderer.startWidth = pulse;
            lineRenderer.endWidth = pulse;
        }
    }
    
    private Mesh CreateTorusMesh(float radius, float thickness, int radialSegments, int tubularSegments)
    {
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[(radialSegments + 1) * (tubularSegments + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[radialSegments * tubularSegments * 6];
        
        // Generate vertices
        for (int i = 0; i <= radialSegments; i++)
        {
            float u = (float)i / radialSegments * 2f * Mathf.PI;
            
            for (int j = 0; j <= tubularSegments; j++)
            {
                float v = (float)j / tubularSegments * 2f * Mathf.PI;
                
                float x = (radius + thickness * Mathf.Cos(v)) * Mathf.Cos(u);
                float y = (radius + thickness * Mathf.Cos(v)) * Mathf.Sin(u);
                float z = thickness * Mathf.Sin(v);
                
                vertices[i * (tubularSegments + 1) + j] = new Vector3(x, y, z);
                uv[i * (tubularSegments + 1) + j] = new Vector2((float)i / radialSegments, (float)j / tubularSegments);
            }
        }
        
        // Generate triangles
        int index = 0;
        for (int i = 0; i < radialSegments; i++)
        {
            for (int j = 0; j < tubularSegments; j++)
            {
                int a = i * (tubularSegments + 1) + j;
                int b = i * (tubularSegments + 1) + j + 1;
                int c = (i + 1) * (tubularSegments + 1) + j;
                int d = (i + 1) * (tubularSegments + 1) + j + 1;
                
                triangles[index++] = a;
                triangles[index++] = d;
                triangles[index++] = b;
                
                triangles[index++] = a;
                triangles[index++] = c;
                triangles[index++] = d;
            }
        }
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    private Mesh CreateCircleMesh(float radius, int segments)
    {
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[segments + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[segments * 3];
        
        // Center vertex
        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);
        
        // Edge vertices
        float angleStep = 2f * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            
            vertices[i + 1] = new Vector3(x, y, 0f);
            uv[i + 1] = new Vector2((x / radius + 1f) * 0.5f, (y / radius + 1f) * 0.5f);
        }
        
        // Triangles
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0; // Center
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2 > segments ? 1 : i + 2;
        }
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    // Used by the InfoPanel system
    public string GetLayerDescription()
    {
        return "Biological Consciousness: The first threshold of consciousness occurs in living systems through autocatalytic closure.";
    }
} 