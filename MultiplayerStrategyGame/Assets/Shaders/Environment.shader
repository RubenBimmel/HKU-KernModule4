Shader "Custom/Environment" {
	Properties {
		_TextureMask ("Texture mask (Greyscale)", 2D) = "white" {}
		//_MaskSize ("Mask size", Float) = 1.0
		_HeightMap ("Height map (Greyscale)", 2D) = "white" {}
		_HeightStrength ("Height map strength", Float) = 1.0
		_Texture1 ("Albedo 1 (RGB)", 2D) = "white" {}
		_Normal1 ("Normal 1 (RGB)", 2D) = "white" {}
		_Texture2 ("Albedo 2 (RGB)", 2D) = "white" {}
		_Normal2 ("Normal 2 (RGB)", 2D) = "white" {}
		[PerRendererData]_Color ("Color", Color) = (1,1,1,1)
		[PerRendererData]_EmissionStrength ("Emission strength", Float) = 0
		[PerRendererData]_Saturation ("Saturation", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _TextureMask;
		sampler2D _HeightMap;
		sampler2D _Texture1;
		sampler2D _Texture2;
		sampler2D _Normal1;
		sampler2D _Normal2;

		float4 _TextureMask_ST;

		struct Input {
			float2 uv_TextureMask;
			float2 uv_Texture1;
			float2 uv_Texture2;
			float3 worldPos;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _HeightStrength;
		float _EmissionStrength;
		float _Saturation;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_full v) {
			
			// Get worldPos
			float4 worldPos = mul(unity_ObjectToWorld, v.vertex);	//volgorde hiervan is belangrijk: eerst matrix, dan positie!

			//Object-space displacement map op basis van een grijs-waarde heightmap (vandaar dat we alleen het rood kanaal gebruiken)
			half height = tex2Dlod(_HeightMap, float4(worldPos.xz * _TextureMask_ST.xy + _TextureMask_ST.zw, 0, 0)).r * _HeightStrength;

			worldPos.y += height;
			v.vertex = mul(unity_WorldToObject, worldPos);

			//v.vertex.y += sin( _Time.z + v.vertex.x * 10 ) * 0.1;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = _Color;
			if (_Color.r == 1 && _Color.g == 1 && _Color.b == 1) {
				c = lerp(tex2D (_Texture1, IN.uv_Texture1), tex2D (_Texture2, IN.uv_Texture2), tex2D (_TextureMask, IN.worldPos.xz * _TextureMask_ST.xy + _TextureMask_ST.zw));
			}
			fixed4 n = lerp(tex2D (_Normal1, IN.uv_Texture1), tex2D (_Normal2, IN.uv_Texture2), tex2D (_TextureMask, IN.worldPos.xz * _TextureMask_ST.xy + _TextureMask_ST.zw));
			
			// Albedo comes from a texture tinted by color
			if (_Saturation < 1) {
				fixed4 s = (c.r + c.g + c.b) / 3;
				c = lerp (s, c, _Saturation);
			}
			o.Albedo = c.rgb;
			o.Normal = n;
			// Metallic and smoothness are 0 by default
			o.Metallic = 0;
			o.Smoothness = 0;
			o.Alpha = c.a;

			o.Emission = _EmissionStrength;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
