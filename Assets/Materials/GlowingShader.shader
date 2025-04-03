Shader "Custom/GlowingShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionIntensity ("Emission Intensity", Range(0,10)) = 1.0
        _FresnelPower ("Fresnel Power", Range(0.1, 5.0)) = 1.0
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1.0
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.2
        _AlphaFalloff ("Alpha Falloff", Range(0.1, 10.0)) = 1.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 200
        
        // Enable transparency
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        CGPROGRAM
        // Physically based Standard lighting model
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0
        
        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
            float3 worldNormal;
        };
        
        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _EmissionColor;
        float _EmissionIntensity;
        float _FresnelPower;
        float _PulseSpeed;
        float _PulseAmount;
        float _AlphaFalloff;
        
        // Add instancing support for this shader
        #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Base color with transparency
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            
            // Calculate fresnel effect (stronger glow at glancing angles)
            float fresnel = pow(1.0 - saturate(dot(normalize(IN.viewDir), normalize(IN.worldNormal))), _FresnelPower);
            
            // Pulsating effect
            float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;
            
            // Apply fresnel to the alpha for a ethereal edge glow
            c.a *= fresnel * _Color.a * pulse;
            
            // Modify alpha falloff to create softer edges
            c.a = pow(c.a, _AlphaFalloff);
            
            // Set albedo value
            o.Albedo = c.rgb;
            
            // Set alpha value
            o.Alpha = c.a;
            
            // Set emission for glow effect
            o.Emission = _EmissionColor.rgb * _EmissionIntensity * fresnel * pulse;
            
            // Set metallic and smoothness for light interaction
            o.Metallic = 0.1;
            o.Smoothness = 0.8;
        }
        ENDCG
    }
    FallBack "Transparent/VertexLit"
} 