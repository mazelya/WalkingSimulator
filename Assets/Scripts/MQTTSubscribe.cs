using UnityEngine;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;

public class MQTTSubscribe : M2MqttUnityClient {
    [Header("Subscribe")]
    public string topic = "unity/cube/color";
    public Renderer cubeRenderer;

    protected override void SubscribeTopics() {
        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
    }

    protected override void DecodeMessage(string topic, byte[] message) {
        string payload = Encoding.UTF8.GetString(message).Trim();
        if (int.TryParse(payload, out int value)) {
            if (cubeRenderer) {
                cubeRenderer.material.color = value switch {
                    1 => Color.red,
                    2 => Color.green,
                    3 => Color.yellow,
                    _ => Color.white
                };
                Debug.Log("MQTT reçu: " + value);
            }
        }
    }

    // AJOUTÉ ICI DANS LA CLASSE
    public void PublishTrigger(int value) {
        string msg = value.ToString();
        if (client.IsConnected) {
            client.Publish(topic, Encoding.UTF8.GetBytes(msg), 1, false);
            Debug.Log("Trigger envoyé: " + msg);
        }
    }
}
