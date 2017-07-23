Shader "Toony Gooch/Toony Gooch RimOutline Hard"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		
        _RimPower ("Rim Power", Float) = 1.4
        _RimColor ("Rim Color (RGB) + Intensity (A)", Color) = (0.0,0.0,0.0,1.0)
        
        _SColor ("Shadow Color", Color) = (0.0,0.0,0.0,1)
		_LColor ("Highlight Color", Color) = (0.5,0.5,0.5,1)
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
CGPROGRAM
		#pragma surface surf ToonRamp
		
		sampler2D _MainTex;
		sampler2D _Ramp;
		float4 _Color;
		float4 _LColor;
		float4 _SColor;
		float _RimPower;
		float4 _RimColor;
		
		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
		#pragma lighting ToonRamp exclude_path:prepass
		inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
		{
			#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
			#endif
			
			half d = dot (s.Normal, lightDir)*0.5 + 0.5;
			half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;
			ramp = lerp(_SColor, _LColor, ramp);
			
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
			c.a = 0;
			
			return c;
		}
		
		
		struct Input
		{
			float2 uv_MainTex : TEXCOORD0;
			float3 pos : POSITION0;
			float3 viewDir;
		};
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = c.a;
			
			//Rim Outline
			half rim = saturate( dot(normalize(IN.viewDir), o.Normal) );
			o.Albedo *= 1.0f - (step(rim, _RimPower) * (1.0f-_RimColor) * _RimColor.a);
		}
ENDCG

	}
	
	Fallback "Toon/Lighted Outline"
}
