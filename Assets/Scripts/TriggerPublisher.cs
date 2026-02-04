using UnityEngine;
using M2MqttUnity;

public class TriggerPublisher : MonoBehaviour {
    public MQTTSubscribe mqtt;  // Drag ton MQTTSubscribe GameObject
    public int sendValue = 1;  // 1,2,3

    void OnTriggerEnter(Collider other) {
        mqtt.PublishTrigger(sendValue);  // Utilise méthode de MQTTSubscribe
    }
}
