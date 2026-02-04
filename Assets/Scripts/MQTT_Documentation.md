# Documentation MQTT Unity

Ce document explique le fonctionnement des deux scripts MQTT utilisés dans ce projet Unity.

---

## Prérequis - Installation de l'Asset M2MqttUnity

Avant d'utiliser les scripts MQTT, vous devez installer la bibliothèque **M2MqttUnity**.

### Téléchargement

L'asset est disponible sur GitHub:

> **Repository**: [M2MqttUnity](https://github.com/gpvigano/M2MqttUnity)

### Installation

1. Télécharger le repository (bouton "Code" > "Download ZIP") ou cloner avec git:
   ```bash
   git clone https://github.com/gpvigano/M2MqttUnity.git
   ```

2. Copier les dossiers suivants dans votre projet Unity (`Assets/`):
   - `M2Mqtt/` - Le client MQTT .NET (basé sur Eclipse Paho)
   - `M2MqttUnity/` - L'adaptation pour Unity

3. Votre structure de dossiers devrait ressembler à:
   ```
   Assets/
   ├── M2Mqtt/
   │   ├── Messages/
   │   ├── Net/
   │   ├── MqttClient.cs
   │   └── ...
   ├── M2MqttUnity/
   │   ├── Scripts/
   │   │   ├── M2MqttUnityClient.cs
   │   │   └── BrokerSettings.cs
   │   └── Examples/
   └── Scripts/
       ├── MQTTSubscribe.cs
       └── TriggerPublisher.cs
   ```

### Informations sur l'asset

| Info | Détail |
|------|--------|
| Auteur | Giovanni Paolo Vigano |
| Licence | MIT (M2MqttUnity) / EPL (M2Mqtt) |
| Basé sur | Eclipse Paho M2Mqtt |
| Compatible | Unity 2018+, UWP, HoloLens |

---

## Architecture globale

```
┌─────────────────────┐         ┌─────────────────────┐
│  TriggerPublisher   │────────▶│    MQTTSubscribe    │
│  (Détection collision)        │  (Client MQTT)      │
└─────────────────────┘         └──────────┬──────────┘
                                           │
                                           ▼
                                ┌─────────────────────┐
                                │   Broker MQTT       │
                                │   (ex: Mosquitto)   │
                                └─────────────────────┘
```

---

## 1. MQTTSubscribe.cs

### Description
Ce script est le **client MQTT principal**. Il hérite de `M2MqttUnityClient` (bibliothèque M2Mqtt pour Unity) et gère la connexion au broker, l'abonnement aux topics et le traitement des messages.

### Propriétés configurables (Inspector)

| Propriété | Type | Description |
|-----------|------|-------------|
| `topic` | string | Le topic MQTT auquel s'abonner (défaut: `"unity/cube/color"`) |
| `cubeRenderer` | Renderer | Référence au Renderer du cube dont on change la couleur |

### Fonctionnement

#### 1. Abonnement au topic
```csharp
protected override void SubscribeTopics() {
    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
}
```
- S'abonne au topic configuré avec le niveau de QoS 2 (Exactly Once)
- QoS 2 garantit que chaque message est reçu exactement une fois

#### 2. Réception et décodage des messages
```csharp
protected override void DecodeMessage(string topic, byte[] message) {
    string payload = Encoding.UTF8.GetString(message).Trim();
    if (int.TryParse(payload, out int value)) {
        cubeRenderer.material.color = value switch {
            1 => Color.red,
            2 => Color.green,
            3 => Color.yellow,
            _ => Color.white
        };
    }
}
```
- Convertit le message reçu (bytes) en string
- Parse la valeur en entier
- Change la couleur du cube selon la valeur:

| Valeur | Couleur |
|--------|---------|
| 1 | Rouge |
| 2 | Vert |
| 3 | Jaune |
| autre | Blanc |

#### 3. Publication de messages
```csharp
public void PublishTrigger(int value) {
    if (client.IsConnected) {
        client.Publish(topic, Encoding.UTF8.GetBytes(msg), 1, false);
    }
}
```
- Publie une valeur sur le topic configuré
- QoS 1 (At Least Once) pour la publication
- Vérifie que le client est connecté avant d'envoyer

---

## 2. TriggerPublisher.cs

### Description
Ce script détecte les **collisions de type Trigger** dans Unity et envoie un message MQTT via `MQTTSubscribe`.

### Propriétés configurables (Inspector)

| Propriété | Type | Description |
|-----------|------|-------------|
| `mqtt` | MQTTSubscribe | Référence au script MQTTSubscribe (glisser-déposer le GameObject) |
| `sendValue` | int | Valeur à envoyer (1, 2 ou 3) |

### Fonctionnement

```csharp
void OnTriggerEnter(Collider other) {
    mqtt.PublishTrigger(sendValue);
}
```

- `OnTriggerEnter` est appelé automatiquement par Unity quand un objet avec un Rigidbody entre dans le Collider (marqué comme Trigger)
- Appelle la méthode `PublishTrigger` de `MQTTSubscribe` avec la valeur configurée

### Prérequis
- Le GameObject doit avoir un **Collider** avec `Is Trigger = true`
- L'objet qui entre dans le trigger doit avoir un **Rigidbody**

---

## Configuration dans Unity

### Étape 1: Configurer MQTTSubscribe
1. Créer un GameObject vide (ex: "MQTTManager")
2. Attacher le script `MQTTSubscribe`
3. Dans l'Inspector, configurer:
   - **Broker Address**: Adresse du serveur MQTT (ex: `localhost`, `test.mosquitto.org`)
   - **Broker Port**: Port (défaut: `1883`)
   - **Topic**: Topic MQTT (ex: `unity/cube/color`)
   - **Cube Renderer**: Glisser le cube qui changera de couleur

### Étape 2: Configurer TriggerPublisher
1. Créer un GameObject avec un Collider (ex: BoxCollider)
2. Cocher **Is Trigger** sur le Collider
3. Attacher le script `TriggerPublisher`
4. Dans l'Inspector:
   - **Mqtt**: Glisser le GameObject "MQTTManager"
   - **Send Value**: Choisir 1, 2 ou 3

---

## Exemple d'utilisation

### Scénario
Un joueur traverse différentes zones qui changent la couleur d'un cube.

```
Zone Rouge (sendValue=1)  ──┐
                            │
Zone Verte (sendValue=2)  ──┼──▶ MQTTSubscribe ──▶ Cube change de couleur
                            │
Zone Jaune (sendValue=3)  ──┘
```

### Test manuel avec MQTT
Vous pouvez aussi tester en envoyant des messages depuis un terminal:

```bash
# Avec mosquitto_pub
mosquitto_pub -h localhost -t "unity/cube/color" -m "1"  # Rouge
mosquitto_pub -h localhost -t "unity/cube/color" -m "2"  # Vert
mosquitto_pub -h localhost -t "unity/cube/color" -m "3"  # Jaune
```

---

## Niveaux de QoS MQTT

| Niveau | Nom | Description |
|--------|-----|-------------|
| 0 | At Most Once | Le message peut être perdu |
| 1 | At Least Once | Le message arrive au moins une fois (peut être dupliqué) |
| 2 | Exactly Once | Le message arrive exactement une fois (utilisé pour Subscribe) |

---

## Dépendances

- **M2MqttUnity**: Bibliothèque MQTT pour Unity (incluse dans `Assets/M2MqttUnity/`)
- **M2Mqtt**: Client MQTT .NET (inclus dans `Assets/M2Mqtt/`)
