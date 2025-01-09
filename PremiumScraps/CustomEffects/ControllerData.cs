using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    // https://discussions.unity.com/t/how-can-i-send-a-render-texture-over-the-network/32327/2
    internal class ControllerData
    {
        public static readonly int dataWidth = 80;
        public static readonly int dataHeight = 40;

        [Serializable]
        public class SerializableColor
        {
            readonly byte r;
            readonly byte g;
            readonly byte b;
            readonly byte a;

            public SerializableColor(Color32 source)
            {
                r = source.r;
                g = source.g;
                b = source.b;
                a = source.a;
            }

            public Color32 ToColor()
            {
                return new Color32(r, g, b, a);
            }
        }

        public static Color[] GetPixels(RenderTexture texture, bool resized = true)
        {
            RenderTexture.active = texture;
            Texture2D tempTex = new Texture2D(texture.width, texture.height);
            tempTex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            tempTex.Apply();
            if (resized)
            {
                RenderTexture resizedTex = new RenderTexture(dataWidth, dataHeight, 24);
                RenderTexture.active = resizedTex;
                Graphics.Blit(tempTex, resizedTex);
                tempTex = new Texture2D(dataWidth, dataHeight);
                tempTex.ReadPixels(new Rect(0, 0, dataWidth, dataHeight), 0, 0);
                tempTex.Apply();
            }
            return tempTex.GetPixels();
        }

        public static SerializableColor[] Encode(Color[] data)
        {
            var result = new SerializableColor[data.Length];
            for (int i = 0; i < data.Length; i++)
                result[i] = new SerializableColor(data[i]);
            return result;
        }

        public static Color[] Decode(SerializableColor[] data)
        {
            var result = new Color[data.Length];
            for (int i = 0; i < data.Length; i++)
                result[i] = data[i].ToColor();
            return result;
        }

        public static byte[] SerializeObject<T>(T objectToSerialize)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            bf.Serialize(stream, objectToSerialize);
            stream.Position = 0;
            return stream.ToArray();
        }

        public static T DeserializeObject<T>(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data) { Position = 0 };
            BinaryFormatter bf = new BinaryFormatter { Binder = new VersionFixer() };
            var result = (T)bf.Deserialize(stream);
            return result;
        }

        sealed class VersionFixer : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type? typeToDeserialize;
                var assemVer1 = Assembly.GetExecutingAssembly().FullName;
                if (assemblyName != assemVer1)
                    assemblyName = assemVer1;
                typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
                return typeToDeserialize;
            }
        }

        /*[ServerRpc(RequireOwnership = false)]
private void UpdateScreenRequestServerRpc(ulong playerAskingClientId)
{
    if (!targetIsValid)
        return;
    var clientRpcParams = new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { targetClientId } } };
    UpdateScreenRequestClientRpc(playerAskingClientId, clientRpcParams);
}

[ClientRpc]
private void UpdateScreenRequestClientRpc(ulong playerAskingClientId, ClientRpcParams clientRpcParams = default)
{
    StartCoroutine(ScreenRequest(playerAskingClientId));
}

private IEnumerator ScreenRequest(ulong playerAskingClientId)
{
    yield return null;
    var texture = GameNetworkManager.Instance.localPlayerController.gameplayCamera.targetTexture;
    byte[] data = ControllerData.SerializeObject(ControllerData.Encode(ControllerData.GetPixels(texture)));
    UpdateScreenResultServerRpc(playerAskingClientId, data, ControllerData.dataWidth, ControllerData.dataHeight);
}

[ServerRpc(RequireOwnership = false)]
private void UpdateScreenResultServerRpc(ulong playerAskingClientId, byte[] data, int width, int height)
{
    var clientRpcParams = new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { playerAskingClientId } } };
    UpdateScreenResultClientRpc(data, width, height, clientRpcParams);
}

[ClientRpc]
private void UpdateScreenResultClientRpc(byte[] data, int width, int height, ClientRpcParams clientRpcParams = default)
{
    StartCoroutine(ScreenResult(data, width, height));
}

private IEnumerator ScreenResult(byte[] data, int width, int height)
{
    yield return null;
    var colors = ControllerData.Decode(ControllerData.DeserializeObject<ControllerData.SerializableColor[]>(data));
    var texture = new Texture2D(width, height);
    texture.SetPixels(colors);
    texture.Apply();
    renderer?.materials[3].SetTexture("_ScreenTexture", texture);
    readyToDisplay = true;
}*/
    }
}
