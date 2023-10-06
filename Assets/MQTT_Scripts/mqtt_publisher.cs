using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class mqtt_publisher : MonoBehaviour
{
    public string nameController = "Controller 1";
    public string tagOfTheMQTTReceiver = "Receiver_temperature";
    public mqttReceiver_test _eventSender;
    string topic = "MQTT_Unity/temperature";
  
    
    void Start()
    {
        

        _eventSender = GameObject.FindGameObjectsWithTag(tagOfTheMQTTReceiver)[0].gameObject.GetComponent<mqttReceiver_test>();

       
    }

    public void SendIt(string messageToSend)
    {
        _eventSender.messagePublish = messageToSend;
        //_eventSender.TestPublish(messageToSend.ToString());
        //_eventSender.TestPublish2();
        _eventSender.TestPublish4(topic);
    }

}