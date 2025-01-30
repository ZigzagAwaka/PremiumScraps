using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Rendering;

namespace PremiumScraps.CustomEffects
{
    internal class ControllerRender : MonoBehaviour
    {
        ShadowCastingMode previousTargetShadowCastingMode;
        ShadowCastingMode previousLocalShadowCastingMode;
        int previousLocalLayer;
        Light? nightvisionLight;
        PlayerControllerB? targetPlayer;
        PlayerControllerB? localPlayer;
        Camera? controllerCamera;

        public void Setup(Camera cam, PlayerControllerB player, PlayerControllerB target, Light light)
        {
            controllerCamera = cam;
            localPlayer = player;
            targetPlayer = target;
            nightvisionLight = light;
            RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
            RenderPipelineManager.endCameraRendering += EndCameraRendering;
        }

        void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (controllerCamera != null && controllerCamera == camera)
            {
                if (nightvisionLight != null)
                {
                    nightvisionLight.enabled = true;
                }
                if (localPlayer != null)
                {
                    previousLocalShadowCastingMode = localPlayer.thisPlayerModel.shadowCastingMode;
                    previousLocalLayer = localPlayer.thisPlayerModel.gameObject.layer;
                    localPlayer.thisPlayerModel.shadowCastingMode = ShadowCastingMode.On;
                    localPlayer.thisPlayerModel.gameObject.layer = 0;  // DEFAULT LAYER
                }
                if (targetPlayer != null)
                {
                    previousTargetShadowCastingMode = targetPlayer.thisPlayerModel.shadowCastingMode;
                    targetPlayer.thisPlayerModel.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }
            }
        }

        void EndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (controllerCamera != null && controllerCamera == camera)
            {
                if (nightvisionLight != null)
                    nightvisionLight.enabled = false;
                if (localPlayer != null)
                {
                    localPlayer.thisPlayerModel.shadowCastingMode = previousLocalShadowCastingMode;
                    localPlayer.thisPlayerModel.gameObject.layer = previousLocalLayer;
                }
                if (targetPlayer != null)
                    targetPlayer.thisPlayerModel.shadowCastingMode = previousTargetShadowCastingMode;
            }
        }

        public void Free()
        {
            RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= EndCameraRendering;
        }
    }
}
