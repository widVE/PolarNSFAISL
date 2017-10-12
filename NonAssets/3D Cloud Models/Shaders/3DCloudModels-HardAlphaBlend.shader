// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:Transparent/Diffuse,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:False,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.6941177,fgcg:0.6039216,fgcb:0.4862745,fgca:1,fgde:0.01162162,fgrn:183.7838,fgrf:956.7568,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32589,y:32639|emission-3-OUT,alpha-591-OUT;n:type:ShaderForge.SFN_Lerp,id:3,x:32934,y:32656|A-4-RGB,B-419-RGB,T-440-OUT;n:type:ShaderForge.SFN_Color,id:4,x:33161,y:32420,ptlb:Ambient Color,ptin:_AmbientColor,glob:False,c1:0.2078431,c2:0.2588235,c3:0.2980392,c4:1;n:type:ShaderForge.SFN_Color,id:419,x:33252,y:32592,ptlb:Sun Color,ptin:_SunColor,glob:False,c1:0.972549,c2:0.9215686,c3:0.8784314,c4:1;n:type:ShaderForge.SFN_Tex2d,id:420,x:33451,y:32792,ptlb:Cloud Texture,ptin:_CloudTexture,tex:98e6ba859b87c8e44b7dc9d790f85d13,ntxv:0,isnm:False|UVIN-421-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:421,x:33632,y:32782,uv:0;n:type:ShaderForge.SFN_Slider,id:424,x:33349,y:33037,ptlb:Cloud Growth,ptin:_CloudGrowth,min:1,cur:0.2620265,max:-1;n:type:ShaderForge.SFN_Subtract,id:425,x:33107,y:33042|A-466-OUT,B-424-OUT;n:type:ShaderForge.SFN_Subtract,id:428,x:33513,y:33380|A-420-R,B-429-OUT;n:type:ShaderForge.SFN_Vector1,id:429,x:33865,y:33465,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:430,x:33355,y:33380|A-428-OUT,B-431-OUT;n:type:ShaderForge.SFN_Slider,id:431,x:33501,y:33615,ptlb:Cloud Contrast,ptin:_CloudContrast,min:0.1,cur:3,max:3;n:type:ShaderForge.SFN_Add,id:432,x:33188,y:33380|A-430-OUT,B-429-OUT;n:type:ShaderForge.SFN_Log,id:433,x:34042,y:32998,lt:0|IN-435-OUT;n:type:ShaderForge.SFN_Slider,id:435,x:34253,y:33013,ptlb:Bias,ptin:_Bias,min:0.25,cur:0.75,max:0.95;n:type:ShaderForge.SFN_Divide,id:436,x:33933,y:33197|A-439-OUT,B-433-OUT;n:type:ShaderForge.SFN_Vector1,id:439,x:34223,y:33217,v1:-0.3;n:type:ShaderForge.SFN_Power,id:440,x:33428,y:33206|VAL-444-OUT,EXP-436-OUT;n:type:ShaderForge.SFN_Clamp01,id:444,x:32984,y:33421|IN-432-OUT;n:type:ShaderForge.SFN_Multiply,id:465,x:32962,y:32862|A-469-OUT,B-420-G;n:type:ShaderForge.SFN_OneMinus,id:466,x:33219,y:32755|IN-420-R;n:type:ShaderForge.SFN_Power,id:469,x:32949,y:33111|VAL-425-OUT,EXP-480-OUT;n:type:ShaderForge.SFN_Multiply,id:474,x:32632,y:33124|A-465-OUT,B-478-OUT;n:type:ShaderForge.SFN_Vector1,id:478,x:32661,y:33276,v1:2;n:type:ShaderForge.SFN_Vector1,id:480,x:32909,y:33262,v1:0.4;n:type:ShaderForge.SFN_Clamp01,id:591,x:32453,y:33136|IN-474-OUT;proporder:420-4-419-424-431-435;pass:END;sub:END;*/

Shader "QuantumTheory/3DCloudModels-HardAlphaBlend" {
    Properties {
        _CloudTexture ("Cloud Texture", 2D) = "white" {}
        _AmbientColor ("Ambient Color", Color) = (0.2078431,0.2588235,0.2980392,1)
        _SunColor ("Sun Color", Color) = (0.972549,0.9215686,0.8784314,1)
        _CloudGrowth ("Cloud Growth", Range(1, -1)) = 0.2620265
        _CloudContrast ("Cloud Contrast", Range(0.1, 3)) = 3
        _Bias ("Bias", Range(0.25, 0.95)) = 0.75
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _AmbientColor;
            uniform float4 _SunColor;
            uniform sampler2D _CloudTexture; uniform float4 _CloudTexture_ST;
            uniform float _CloudGrowth;
            uniform float _CloudContrast;
            uniform float _Bias;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_421 = i.uv0;
                float4 node_420 = tex2D(_CloudTexture,TRANSFORM_TEX(node_421.rg, _CloudTexture));
                float node_429 = 0.5;
                float3 emissive = lerp(_AmbientColor.rgb,_SunColor.rgb,pow(saturate((((node_420.r-node_429)*_CloudContrast)+node_429)),((-0.3)/log(_Bias))));
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,saturate(((pow(((1.0 - node_420.r)-_CloudGrowth),0.4)*node_420.g)*2.0)));
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
//    CustomEditor "ShaderForgeMaterialInspector"
}
