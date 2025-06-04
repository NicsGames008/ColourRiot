Shader "Custom/OutlineGlow"
{
    Properties
    {
        [Header(Main Settings)]
        _MainTex ("Main Texture", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (1,1,0,1)
        
        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (1,0.5,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
        
        [Header(Glow Settings)]
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1
        _MinGlow ("Min Glow", Range(0, 1)) = 0.2
        _MaxGlow ("Max Glow", Range(0, 1)) = 0.8
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        // Outline pass
        Pass
        {
            Name "OUTLINE"
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;
            float _GlowIntensity;
            float _PulseSpeed;
            float _MinGlow;
            float _MaxGlow;

            v2f vert(appdata v)
            {
                v2f o;
                
                // Calculate pulse value
                float pulse = (_MinGlow + (_MaxGlow - _MinGlow) * (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5));
                
                // Expand along normals with pulse effect
                float3 outlineOffset = normalize(v.normal) * _OutlineWidth * pulse * _GlowIntensity;
                v.vertex.xyz += outlineOffset;
                
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor * _GlowIntensity;
            }
            ENDCG
        }

        // Main pass with glow
        Pass
        {
            Name "MAIN"
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainColor;
            float4 _GlowColor;
            float _GlowIntensity;
            float _PulseSpeed;
            float _MinGlow;
            float _MaxGlow;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate pulse value
                float pulse = (_MinGlow + (_MaxGlow - _MinGlow) * (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5));
                
                // Sample texture and combine with glow
                fixed4 texColor = tex2D(_MainTex, i.uv) * _MainColor;
                fixed4 glowColor = _GlowColor * pulse * _GlowIntensity;
                
                fixed4 finalColor = texColor + glowColor;
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}