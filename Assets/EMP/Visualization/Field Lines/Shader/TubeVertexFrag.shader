// Swarm - Special renderer that draws a swarm of swirling/crawling lines.
// https://github.com/keijiro/Swarm
Shader "Swarm/TubeVertexFrag"
{
    Properties
    {
        _Smoothness("Smoothness", Range(0, 1)) = 0
        _Metallic("Metallic", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass{
        
        CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members color)
#pragma exclude_renderers d3d11
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members color)
//#pragma exclude_renderers d3d11

        //#pragma surface surf Standard vertex:vert addshadow nolightmap
        //#pragma surface surf Standard vertex:vert nolightmap noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap
        //#pragma surface surf Lambert vertex:vert nolightmap noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap
        #pragma vertex vert
        #pragma fragment frag
        #pragma instancing_options procedural:setup
        #pragma target 4.5
        
        #include "UnityCG.cginc"
        
        struct Input
        {
            half param : COLOR;
        };

        half _Smoothness;
        half _Metallic;

        half3 _GradientA;
        half3 _GradientB;
        half3 _GradientC;
        half3 _GradientD;

        float _Radius;

        float4x4 _LocalToWorld;
        float4x4 _WorldToLocal;

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

        StructuredBuffer<float4> _PositionBuffer;
        StructuredBuffer<float4> _TangentBuffer;
        StructuredBuffer<float4> _NormalBuffer;

        uint _InstanceCount;
        uint _HistoryLength;
        uint _IndexOffset;
        uint _IndexLimit;

        #endif

        half3 CosineGradient(half param)
        {
            half3 c = _GradientB * cos(_GradientC * param + _GradientD);
            //return GammaToLinearSpace(saturate(c + _GradientA));
            return saturate(c + _GradientA);
        }
        
        struct v2f {
            half3 worldNormal : TEXCOORD0;
            float4 pos : SV_POSITION;
            float color : COLOR0;
        }
        
        //void vert(inout appdata_full v, out Input data)
        v2f vert(appdata_full v, uint instanceID : SV_InstanceID)
        {
            v2f o;
            
            //UNITY_INITIALIZE_OUTPUT(Input, data);

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

            float phi = v.vertex.x; // Angle in slice
            float cap = v.vertex.y; // -1:head, +1:tail
            float seg = v.vertex.z; // Segment index
//            float phi = vertex.x; // Angle in slice
//            float cap = vertex.y; // -1:head, +1:tail
//            float seg = vertex.z; // Segment index
            uint iseg = min((uint)seg, _IndexLimit);

            // Parameter along the curve (used for coloring).
            float param = seg / _HistoryLength;
            param += (float)instanceID / _InstanceCount; 

            // Index of the current slice in the buffers.
            uint idx = unity_InstanceID;
            idx += _InstanceCount * ((iseg + _IndexOffset) % _HistoryLength);

            float3 p = _PositionBuffer[idx].xyz; // Position
            float3 t = _TangentBuffer[idx].xyz;  // Curve-TNB: Tangent 
            float3 n = _NormalBuffer[idx].xyz;   // Curve-TNB: Normal
            float3 b = cross(t, n);              // Curve-TNB: Binormal

            float3 normal = n * cos(phi) + b * sin(phi); // Surface normal

            // Feedback the results.
            //v.vertex = float4(p + normal * _Radius * (1 - abs(cap)), 1);
            //v.normal = normal * (1 - abs(cap)) + n * cap;
            o.pos = float4(p + normal * _Radius * (1 - abs(cap)), 1);
            o.worldNormal = normal * (1 - abs(cap)) + n * cap;
            o.color = param;
            
            return o;
            
            //v.color = param;

            #endif
        }

        void setup()
        {
            unity_ObjectToWorld = _LocalToWorld;
            unity_WorldToObject = _WorldToLocal;
        }

        //void surf (Input IN, inout SurfaceOutputStandard o)
//        void surf (Input IN, inout SurfaceOutput o)
//        {
//            o.Albedo = CosineGradient(IN.param);
//            //o.Metallic = _Metallic;
//            //o.Smoothness = _Smoothness;
//        }
        
        //fixed4 frag (Input IN, v2f i) : SV_Target
        fixed4 frag (v2f i) : SV_Target
        {
            //return fixed4(CosineGradient(i.color), 0);            
            return fixed4(1, 0, 1, 0);            
        }

        ENDCG
    //Pass
    }
    //SubShader
    }
    FallBack "Diffuse"
}
