using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientConsciousnessLayer : MonoBehaviour
{
    [Header("Wave Settings")]
    public int waveCount = 20;
    public float waveAmplitude = 0.5f;
    public float waveFrequency = 1.0f;
    public float waveSpeed = 0.5f;
    public Material waveMaterial;
    public Color waveColor = new Color(0.2f, 0.3f, 0.5f, 0.3f);
    
    [Header("Particle Settings")]
    public int particleCount = 1000;
    public float particleSize = 0.05f;
    public float particleSpeed = 0.2f;
    public float spawnRadius = 50f;
    public Material particleMaterial;
    public Color particleColor = new Color(0.4f, 0.5f, 0.8f, 0.4f);
    
    private GameObject[] waves;
    private GameObject particleSystem;
    private List<Transform> particles = new List<Transform>();
    
    private void Start()
    {
        CreateWaves();
        CreateParticles();
    }
    
    private void Update()
    {
        AnimateWaves();
        AnimateParticles();
    }
    
    private void CreateWaves()
    {
        waves = new GameObject[waveCount];
        
        for (int i = 0; i < waveCount; i++)
        {
            GameObject wave = new GameObject("Wave_" + i);
            wave.transform.SetParent(transform);
            
            MeshFilter meshFilter = wave.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = wave.AddComponent<MeshRenderer>();
            
            // Create a plane mesh for the wave
            meshFilter.mesh = CreateWaveMesh(20, 20, 100f, 100f);
            
            // Set material and position
            meshRenderer.material = waveMaterial != null ? waveMaterial : new Material(Shader.Find("Standard"));
            meshRenderer.material.color = waveColor;
            
            // Position waves at different heights with slight random offset
            float heightOffset = (i / (float)waveCount) * 50f - 25f;
            wave.transform.position = new Vector3(0f, heightOffset, 0f);
            wave.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            
            // Enable transparency
            meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            meshRenderer.material.SetInt("_ZWrite", 0);
            meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
            meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            meshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            meshRenderer.material.renderQueue = 3000;
            
            waves[i] = wave;
        }
    }
    
    private Mesh CreateWaveMesh(int xSegments, int zSegments, float width, float depth)
    {
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[(xSegments + 1) * (zSegments + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[xSegments * zSegments * 6];
        
        // Create vertices
        for (int z = 0; z <= zSegments; z++)
        {
            float zPos = ((float)z / zSegments - 0.5f) * depth;
            for (int x = 0; x <= xSegments; x++)
            {
                float xPos = ((float)x / xSegments - 0.5f) * width;
                vertices[z * (xSegments + 1) + x] = new Vector3(xPos, 0f, zPos);
                uv[z * (xSegments + 1) + x] = new Vector2((float)x / xSegments, (float)z / zSegments);
            }
        }
        
        // Create triangles
        int index = 0;
        for (int z = 0; z < zSegments; z++)
        {
            for (int x = 0; x < xSegments; x++)
            {
                triangles[index++] = z * (xSegments + 1) + x;
                triangles[index++] = z * (xSegments + 1) + x + 1;
                triangles[index++] = (z + 1) * (xSegments + 1) + x;
                
                triangles[index++] = (z + 1) * (xSegments + 1) + x;
                triangles[index++] = z * (xSegments + 1) + x + 1;
                triangles[index++] = (z + 1) * (xSegments + 1) + x + 1;
            }
        }
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    private void AnimateWaves()
    {
        for (int w = 0; w < waves.Length; w++)
        {
            Mesh mesh = waves[w].GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            
            for (int i = 0; i < vertices.Length; i++)
            {
                // Create wave based on sine waves
                float x = vertices[i].x;
                float z = vertices[i].z;
                float y = 0f;
                
                // Add multiple sine waves with different frequencies and phases
                float phase = Time.time * waveSpeed;
                y += Mathf.Sin(x * waveFrequency * 0.5f + phase) * waveAmplitude;
                y += Mathf.Sin(z * waveFrequency * 0.3f + phase * 1.2f) * waveAmplitude * 0.8f;
                y += Mathf.Sin((x + z) * waveFrequency * 0.2f + phase * 0.8f) * waveAmplitude * 0.4f;
                
                // Add slight variation based on wave index
                y *= 1f + 0.2f * Mathf.Sin(w * 0.5f);
                
                vertices[i] = new Vector3(vertices[i].x, y, vertices[i].z);
            }
            
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }
    }
    
    private void CreateParticles()
    {
        particleSystem = new GameObject("AmbientParticles");
        particleSystem.transform.SetParent(transform);
        
        for (int i = 0; i < particleCount; i++)
        {
            // Create particle
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            particle.name = "Particle_" + i;
            particle.transform.SetParent(particleSystem.transform);
            
            // Random position within sphere
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            particle.transform.position = randomPos;
            
            // Random scale
            float scale = Random.Range(particleSize * 0.5f, particleSize * 1.5f);
            particle.transform.localScale = new Vector3(scale, scale, scale);
            
            // Material
            Renderer renderer = particle.GetComponent<Renderer>();
            renderer.material = particleMaterial != null ? particleMaterial : new Material(Shader.Find("Standard"));
            renderer.material.color = particleColor;
            
            // Enable transparency
            renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            renderer.material.SetInt("_ZWrite", 0);
            renderer.material.DisableKeyword("_ALPHATEST_ON");
            renderer.material.EnableKeyword("_ALPHABLEND_ON");
            renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            renderer.material.renderQueue = 3000;
            
            // Remove collider to improve performance
            Destroy(particle.GetComponent<Collider>());
            
            particles.Add(particle.transform);
        }
    }
    
    private void AnimateParticles()
    {
        foreach (Transform particle in particles)
        {
            // Slow drift movement
            Vector3 moveDir = new Vector3(
                Mathf.Sin(Time.time * 0.1f + particle.position.x * 0.1f),
                Mathf.Sin(Time.time * 0.15f + particle.position.y * 0.1f),
                Mathf.Sin(Time.time * 0.12f + particle.position.z * 0.1f)
            ).normalized;
            
            particle.position += moveDir * particleSpeed * Time.deltaTime;
            
            // If particle drifts too far, bring it back into the sphere
            if (particle.position.magnitude > spawnRadius)
            {
                particle.position = particle.position.normalized * spawnRadius;
            }
        }
    }
    
    // Used by the InfoPanel system
    public string GetLayerDescription()
    {
        return "Ambient Consciousness: The lowest level of consciousness - present everywhere as a fundamental aspect of information.";
    }
} 