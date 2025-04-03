using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CognitiveConsciousnessLayer : MonoBehaviour
{
    [Header("Cognitive Boundary")]
    public float boundaryRadius = 5f;
    public Material boundaryMaterial;
    public Color boundaryColor = new Color(0.7f, 0.8f, 1.0f, 0.5f);
    
    [Header("Core Illumination")]
    public float coreRadius = 3f;
    public Material coreMaterial;
    public Color coreColor = new Color(1.0f, 1.0f, 1.0f, 0.4f);
    public float coreEmissionIntensity = 1.5f;
    
    [Header("Standing Wave Patterns")]
    public int wavePatternCount = 3;
    public int pointsPerPattern = 20;
    public float waveAmplitude = 0.5f;
    public float waveFrequency = 1.5f;
    public float waveWidth = 0.05f;
    public Material wavePatternMaterial;
    public Color wavePatternColor = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    
    [Header("Concept Network")]
    public int conceptNodeCount = 8;
    public float conceptNodeSize = 0.25f;
    public Material conceptNodeMaterial;
    public Color conceptNodeColor = new Color(1.0f, 1.0f, 1.0f, 0.9f);
    
    [Header("Self Node")]
    public float selfNodeSize = 0.5f;
    public Material selfNodeMaterial;
    public Color selfNodeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    
    [Header("Connections")]
    public Material connectionMaterial;
    public Color connectionColor = new Color(1.0f, 1.0f, 1.0f, 0.6f);
    
    [Header("Animation")]
    public float rotationSpeed = 10f;
    public float pulsateSpeed = 2f;
    public float pulsateAmount = 0.1f;
    
    private GameObject boundary;
    private GameObject core;
    private GameObject[] wavePatterns;
    private GameObject[] conceptNodes;
    private GameObject selfNode;
    private GameObject[] connections;
    
    private void Start()
    {
        CreateBoundary();
        CreateCore();
        CreateWavePatterns();
        CreateConceptNetwork();
        CreateConnections();
    }
    
    private void Update()
    {
        AnimateBoundary();
        AnimateCore();
        AnimateWavePatterns();
        AnimateConceptNetwork();
        AnimateConnections();
    }
    
    private void CreateBoundary()
    {
        boundary = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        boundary.name = "CognitiveBoundary";
        boundary.transform.SetParent(transform);
        boundary.transform.localScale = new Vector3(boundaryRadius * 2, boundaryRadius * 2, boundaryRadius * 2);
        
        Renderer renderer = boundary.GetComponent<Renderer>();
        renderer.material = boundaryMaterial != null ? boundaryMaterial : new Material(Shader.Find("Standard"));
        renderer.material.color = boundaryColor;
        
        // Set material properties for transparency and glow
        renderer.material.SetFloat("_Mode", 3); // Transparent mode
        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        renderer.material.SetInt("_ZWrite", 0);
        renderer.material.DisableKeyword("_ALPHATEST_ON");
        renderer.material.EnableKeyword("_ALPHABLEND_ON");
        renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        renderer.material.renderQueue = 3000;
        
        // Add emission for glow effect
        renderer.material.EnableKeyword("_EMISSION");
        renderer.material.SetColor("_EmissionColor", boundaryColor * 0.3f);
        
        // Remove collider for performance
        Destroy(boundary.GetComponent<Collider>());
    }
    
    private void CreateCore()
    {
        core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        core.name = "CognitiveCore";
        core.transform.SetParent(transform);
        core.transform.localScale = new Vector3(coreRadius * 2, coreRadius * 2, coreRadius * 2);
        
        Renderer renderer = core.GetComponent<Renderer>();
        renderer.material = coreMaterial != null ? coreMaterial : new Material(Shader.Find("Standard"));
        renderer.material.color = coreColor;
        
        // Set material properties for glow
        renderer.material.SetFloat("_Mode", 3); // Transparent mode
        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        renderer.material.SetInt("_ZWrite", 0);
        renderer.material.DisableKeyword("_ALPHATEST_ON");
        renderer.material.EnableKeyword("_ALPHABLEND_ON");
        renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        renderer.material.renderQueue = 3000;
        
        // Strong emission for core glow
        renderer.material.EnableKeyword("_EMISSION");
        renderer.material.SetColor("_EmissionColor", coreColor * coreEmissionIntensity);
        
        // Remove collider for performance
        Destroy(core.GetComponent<Collider>());
    }
    
    private void CreateWavePatterns()
    {
        wavePatterns = new GameObject[wavePatternCount];
        
        for (int i = 0; i < wavePatternCount; i++)
        {
            GameObject pattern = new GameObject("WavePattern_" + i);
            pattern.transform.SetParent(transform);
            
            // Random rotation for wave pattern
            pattern.transform.rotation = Quaternion.Euler(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );
            
            LineRenderer lineRenderer = pattern.AddComponent<LineRenderer>();
            
            // Set up line renderer
            lineRenderer.positionCount = pointsPerPattern;
            lineRenderer.startWidth = waveWidth;
            lineRenderer.endWidth = waveWidth;
            lineRenderer.material = wavePatternMaterial != null ? wavePatternMaterial : new Material(Shader.Find("Standard"));
            lineRenderer.material.color = wavePatternColor;
            
            // Make the line glow
            lineRenderer.material.EnableKeyword("_EMISSION");
            lineRenderer.material.SetColor("_EmissionColor", wavePatternColor * 0.8f);
            
            // Generate initial wave points
            Vector3[] points = new Vector3[pointsPerPattern];
            for (int j = 0; j < pointsPerPattern; j++)
            {
                float t = j / (float)(pointsPerPattern - 1);
                float x = (t * 2f - 1f) * boundaryRadius * 0.8f; // Range from -R to R
                float y = Mathf.Sin(t * waveFrequency * Mathf.PI * 2f) * waveAmplitude;
                float z = 0f;
                
                points[j] = new Vector3(x, y, z);
            }
            
            lineRenderer.SetPositions(points);
            
            wavePatterns[i] = pattern;
        }
    }
    
    private void CreateConceptNetwork()
    {
        // Create the central self node
        selfNode = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        selfNode.name = "SelfNode";
        selfNode.transform.SetParent(transform);
        selfNode.transform.localPosition = Vector3.zero;
        selfNode.transform.localScale = new Vector3(selfNodeSize, selfNodeSize, selfNodeSize);
        
        Renderer selfRenderer = selfNode.GetComponent<Renderer>();
        selfRenderer.material = selfNodeMaterial != null ? selfNodeMaterial : new Material(Shader.Find("Standard"));
        selfRenderer.material.color = selfNodeColor;
        
        // Make it glow strongly
        selfRenderer.material.EnableKeyword("_EMISSION");
        selfRenderer.material.SetColor("_EmissionColor", selfNodeColor * 2f);
        
        // Remove collider for performance
        Destroy(selfNode.GetComponent<Collider>());
        
        // Create concept nodes
        conceptNodes = new GameObject[conceptNodeCount];
        
        for (int i = 0; i < conceptNodeCount; i++)
        {
            GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            node.name = "ConceptNode_" + i;
            node.transform.SetParent(transform);
            
            // Position on a smaller sphere around the self node
            float angle = (i / (float)conceptNodeCount) * Mathf.PI * 2;
            float radius = boundaryRadius * 0.4f + Random.Range(-0.1f, 0.1f) * boundaryRadius;
            
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            float z = Random.Range(-0.2f, 0.2f) * boundaryRadius;
            
            node.transform.localPosition = new Vector3(x, y, z);
            
            // Random scale variation
            float scale = conceptNodeSize * (0.8f + Random.value * 0.7f);
            node.transform.localScale = new Vector3(scale, scale, scale);
            
            // Material
            Renderer renderer = node.GetComponent<Renderer>();
            renderer.material = conceptNodeMaterial != null ? conceptNodeMaterial : new Material(Shader.Find("Standard"));
            renderer.material.color = conceptNodeColor;
            
            // Make it glow
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", conceptNodeColor * 0.8f);
            
            // Remove collider for performance
            Destroy(node.GetComponent<Collider>());
            
            conceptNodes[i] = node;
        }
    }
    
    private void CreateConnections()
    {
        int totalConnections = conceptNodeCount * 2; // Each node connects to self and some other nodes
        connections = new GameObject[totalConnections];
        
        int connectionIndex = 0;
        
        // Create connections to self node
        for (int i = 0; i < conceptNodeCount; i++)
        {
            GameObject connection = new GameObject("Connection_Self_" + i);
            connection.transform.SetParent(transform);
            
            LineRenderer lineRenderer = connection.AddComponent<LineRenderer>();
            
            // Set up line renderer
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.03f;
            lineRenderer.endWidth = 0.03f;
            lineRenderer.material = connectionMaterial != null ? connectionMaterial : new Material(Shader.Find("Standard"));
            lineRenderer.material.color = connectionColor;
            
            // Make it glow slightly
            lineRenderer.material.EnableKeyword("_EMISSION");
            lineRenderer.material.SetColor("_EmissionColor", connectionColor * 0.5f);
            
            // Initial positions
            lineRenderer.SetPosition(0, selfNode.transform.position);
            lineRenderer.SetPosition(1, conceptNodes[i].transform.position);
            
            connections[connectionIndex++] = connection;
        }
        
        // Create connections between nodes (not all nodes connect)
        for (int i = 0; i < conceptNodeCount; i++)
        {
            // Connect to a random node that's not too far
            int targetIndex = (i + 1 + Mathf.FloorToInt(Random.value * (conceptNodeCount - 2))) % conceptNodeCount;
            
            GameObject connection = new GameObject("Connection_" + i + "_" + targetIndex);
            connection.transform.SetParent(transform);
            
            LineRenderer lineRenderer = connection.AddComponent<LineRenderer>();
            
            // Set up line renderer
            lineRenderer.positionCount = 3; // Use a curved line with control point
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.material = connectionMaterial != null ? connectionMaterial : new Material(Shader.Find("Standard"));
            lineRenderer.material.color = connectionColor;
            
            // Make it glow slightly
            lineRenderer.material.EnableKeyword("_EMISSION");
            lineRenderer.material.SetColor("_EmissionColor", connectionColor * 0.3f);
            
            // Calculate a curved path through self node
            Vector3 startPos = conceptNodes[i].transform.position;
            Vector3 endPos = conceptNodes[targetIndex].transform.position;
            Vector3 controlPoint = Vector3.Lerp(startPos, endPos, 0.5f) * 0.3f; // Pull toward center
            
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, controlPoint);
            lineRenderer.SetPosition(2, endPos);
            
            connections[connectionIndex++] = connection;
        }
    }
    
    private void AnimateBoundary()
    {
        if (boundary == null) return;
        
        // Gentle pulsation
        float pulse = 1f + Mathf.Sin(Time.time * pulsateSpeed * 0.5f) * pulsateAmount * 0.5f;
        boundary.transform.localScale = new Vector3(
            boundaryRadius * 2 * pulse,
            boundaryRadius * 2 * pulse,
            boundaryRadius * 2 * pulse
        );
    }
    
    private void AnimateCore()
    {
        if (core == null) return;
        
        // More pronounced pulsation for the core
        float pulse = 1f + Mathf.Sin(Time.time * pulsateSpeed) * pulsateAmount;
        core.transform.localScale = new Vector3(
            coreRadius * 2 * pulse,
            coreRadius * 2 * pulse,
            coreRadius * 2 * pulse
        );
        
        // Modulate the emission intensity
        Renderer renderer = core.GetComponent<Renderer>();
        float emissionPulse = coreEmissionIntensity * (1f + Mathf.Sin(Time.time * pulsateSpeed * 1.5f) * 0.3f);
        renderer.material.SetColor("_EmissionColor", coreColor * emissionPulse);
    }
    
    private void AnimateWavePatterns()
    {
        for (int i = 0; i < wavePatterns.Length; i++)
        {
            // Slowly rotate the wave pattern
            wavePatterns[i].transform.Rotate(
                rotationSpeed * 0.01f * (i % 2 == 0 ? 1 : -1),
                rotationSpeed * 0.02f,
                rotationSpeed * 0.015f * (i % 2 == 0 ? -1 : 1)
            );
            
            // Animate the wave itself
            LineRenderer lineRenderer = wavePatterns[i].GetComponent<LineRenderer>();
            Vector3[] points = new Vector3[pointsPerPattern];
            
            for (int j = 0; j < pointsPerPattern; j++)
            {
                float t = j / (float)(pointsPerPattern - 1);
                float phase = Time.time * 2f * (1f + i * 0.2f);
                
                float x = (t * 2f - 1f) * boundaryRadius * 0.8f;
                float y = Mathf.Sin(t * waveFrequency * Mathf.PI * 2f + phase) * waveAmplitude;
                float z = 0f;
                
                // Transform the point by the object's rotation
                points[j] = new Vector3(x, y, z);
            }
            
            lineRenderer.SetPositions(points);
            
            // Pulse the width
            float widthPulse = waveWidth * (1f + Mathf.Sin(Time.time * 3f + i) * 0.3f);
            lineRenderer.startWidth = widthPulse;
            lineRenderer.endWidth = widthPulse;
        }
    }
    
    private void AnimateConceptNetwork()
    {
        // Animate the self node
        float selfPulse = 1f + Mathf.Sin(Time.time * pulsateSpeed * 1.2f) * pulsateAmount;
        selfNode.transform.localScale = new Vector3(
            selfNodeSize * selfPulse,
            selfNodeSize * selfPulse,
            selfNodeSize * selfPulse
        );
        
        // Animate concept nodes
        for (int i = 0; i < conceptNodes.Length; i++)
        {
            // Orbit slightly around their positions
            float angle = (i / (float)conceptNodeCount) * Mathf.PI * 2;
            float timeOffset = Time.time * rotationSpeed * 0.05f;
            float radiusVariation = Mathf.Sin(Time.time * 0.8f + i * 0.7f) * 0.05f;
            
            float radius = boundaryRadius * (0.4f + radiusVariation) + Random.Range(-0.1f, 0.1f) * boundaryRadius;
            
            float x = Mathf.Cos(angle + timeOffset) * radius;
            float y = Mathf.Sin(angle + timeOffset) * radius;
            float z = conceptNodes[i].transform.localPosition.z + Mathf.Sin(Time.time + i) * 0.01f;
            
            // Smooth movement
            conceptNodes[i].transform.localPosition = Vector3.Lerp(
                conceptNodes[i].transform.localPosition,
                new Vector3(x, y, z),
                Time.deltaTime * 1.5f
            );
            
            // Pulse size
            float pulse = 1f + Mathf.Sin(Time.time * 2f + i * 0.8f) * 0.15f;
            float baseScale = conceptNodeSize * (0.8f + Random.value * 0.7f);
            conceptNodes[i].transform.localScale = new Vector3(
                baseScale * pulse,
                baseScale * pulse,
                baseScale * pulse
            );
        }
    }
    
    private void AnimateConnections()
    {
        int connectionIndex = 0;
        
        // Update self connections
        for (int i = 0; i < conceptNodeCount; i++)
        {
            LineRenderer lineRenderer = connections[connectionIndex++].GetComponent<LineRenderer>();
            
            // Update start and end positions
            lineRenderer.SetPosition(0, selfNode.transform.position);
            lineRenderer.SetPosition(1, conceptNodes[i].transform.position);
            
            // Pulse width based on "thought activity"
            float widthPulse = 0.03f * (1f + Mathf.Sin(Time.time * 5f + i * 0.7f) * 0.5f);
            lineRenderer.startWidth = widthPulse;
            lineRenderer.endWidth = widthPulse;
        }
        
        // Update node-to-node connections
        for (int i = 0; i < conceptNodeCount; i++)
        {
            if (connectionIndex >= connections.Length) break;
            
            LineRenderer lineRenderer = connections[connectionIndex++].GetComponent<LineRenderer>();
            
            // Calculate target node
            int targetIndex = (i + 1 + Mathf.FloorToInt(Random.value * (conceptNodeCount - 2))) % conceptNodeCount;
            
            Vector3 startPos = conceptNodes[i].transform.position;
            Vector3 endPos = conceptNodes[targetIndex].transform.position;
            
            // Curved control point that moves slightly for animation
            float pullFactor = 0.3f + Mathf.Sin(Time.time * 0.7f + i * 0.5f) * 0.1f;
            Vector3 controlPoint = Vector3.Lerp(startPos, endPos, 0.5f) * pullFactor;
            
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, controlPoint);
            lineRenderer.SetPosition(2, endPos);
            
            // Subtle width variation
            float widthPulse = 0.02f * (1f + Mathf.Sin(Time.time * 3f + i * 0.5f) * 0.3f);
            lineRenderer.startWidth = widthPulse;
            lineRenderer.endWidth = widthPulse;
        }
    }
    
    // Used by the InfoPanel system
    public string GetLayerDescription()
    {
        return "Cognitive Consciousness: A second level of amplification through the development of integrated worldviews.";
    }
} 