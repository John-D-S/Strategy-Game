Shader "Custom/Blink" 
{
Properties 
    {
        _MainTex ("Particle Texture", 2D) = "red" {}
        _Blink ( "Blink", Float ) = 0
        _Color("Color", Color) = (1,1,1)
    }

SubShader 
    {
    Tags {"RenderType"="Transparent"}
        ZWrite On
        
        Blend SrcAlpha One
        
        

    

    CGPROGRAM
    #pragma surface surf Lambert
    
    
    sampler2D _MainTex;
    float _Blink;
    float3 _Color;

    struct Input
    {
        float2 uv_MainTex;
    };

    void surf (Input IN, inout SurfaceOutput o)
    {
        half4 c = tex2D (_MainTex, IN.uv_MainTex );
        if( _Blink == 1.0f ) 
            c *=  ( 0.2f + abs( sin( _Time.w ) ) );
        o.Albedo    = c.rgb*_Color;
        o.Emission = c.rgb*_Color.rgb;
        o.Alpha     = c.a;
    }
    fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
         return fixed4(0,0,0,0);//half4(s.Albedo, s.Alpha);
     }
    
    ENDCG
} 
//Fallback "Diffuse"
}