/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;

public class mqttReceiver_test : M2MqttUnityClient
{

    [Header("MQTT topics")]
    [Tooltip("Set the topic to subscribe. !!!ATTENTION!!! multi-level wildcard # subscribes to all topics")]
    public string topicSubscribe = "#"; // topic to subscribe. !!! The multi-level wildcard # is used to subscribe to all the topics. Attention i if #, subscribe to all topics. Attention if MQTT is on data plan
    [Tooltip("Set the topic to publish (optional)")]
    public string topicPublish = "M2MQTT_Unity/licht"; // topic to publish
    //public string topicPublish = "test"; // topic to publish
    public string messagePublish = ""; // message to publish
    public string broker = "";
    bool updateUI;
    private List<string> eventMessages = new List<string>();
    private string testMessage = "This is a test";
    private char[] messageChar= { 'a', 'b', 'c'};

     [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
    public bool autoTest = false;

    private string m_msg;

    public string msg
    {
        get
        {
            return m_msg;
        }
        set
        {
            if (m_msg == value) return;
            m_msg = value;

            OnMessageChanged?.Invoke(msg);
            if (OnMessageArrived != null)
            {
                OnMessageArrived(m_msg);
            }
        }
    }


    public delegate void MessageChangedEventHandler(string newMessage);

    public event MessageChangedEventHandler OnMessageChanged;

    [System.Serializable]
    public class MQTTMessageEvent : UnityEvent<string> { }
    public MQTTMessageEvent onMessageReceived;

    void OnMessageReceived(string newMessage)
    {
        onMessageReceived.Invoke(newMessage);
    }

    private bool m_isConnected;

    public bool isConnected
    {
        get
        {
            return m_isConnected;
            Debug.Log("Is Connected");
        }
        set
        {
            if (m_isConnected == value) return;
            m_isConnected = value;
            if (OnConnectionSucceeded != null)
            {
                OnConnectionSucceeded(isConnected);
            }
        }
    }

    
    

    public event OnMessageArrivedDelegate OnMessageArrived;
    public delegate void OnMessageArrivedDelegate(string newMsg);
    public event OnConnectionSucceededDelegate OnConnectionSucceeded;
    public delegate void OnConnectionSucceededDelegate(bool isConnected);

    /*

    public void TestPublish(string messageToPublish)
    {
        //messageChar = messageToPublish.ToCharArray();
        Debug.Log("Topic: " + topicPublish);
        Debug.Log("Message: " + messageToPublish);
        client.Publish(topicPublish, System.Text.Encoding.UTF8.GetBytes(messageToPublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        Debug.Log("Test message published: " + messageToPublish);
      
    }

    public void TestPublish2()
    {
        client.Publish("M2MQTT_Unity/licht", System.Text.Encoding.UTF8.GetBytes("Test message"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        Debug.Log("Test message published");
       // AddUiMessage("Test message published.");
    }


    public void TestPublish3()
    {
        topicPublish = "M2MQTT_Unity/licht";
        //messagePublish = "hoi";
        client.Publish(topicPublish, System.Text.Encoding.UTF8.GetBytes(messagePublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        Debug.Log("Test message published");
        // AddUiMessage("Test message published.");

    }
    */
        public void TestPublish4(string topic)
        {
            topicPublish = topic;

            client.Publish(topicPublish, System.Text.Encoding.UTF8.GetBytes(messagePublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("Test message published");
        
        }

        public void SetBrokerAddress(string brokerAddress)
    {

            this.brokerAddress = brokerAddress;
    }

    public void SetBrokerPort(string brokerPort)
    {

            int.TryParse(brokerPort, out this.brokerPort);

    }

    public void SetEncrypted(bool isEncrypted)
    {
        this.isEncrypted = isEncrypted;
    }
/*

    public void SetUiMessage(string msg)
    {
            updateUI = true;

    }

    public void AddUiMessage(string msg)
    {

            updateUI = true;
        
    }

    protected override void OnConnecting()
    {
        base.OnConnecting();
      // Log messa
        SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
    }

    protected override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("Connected to broker on " + brokerAddress + "\n");

        if (autoTest)
        {
            TestPublish(testMessage);
        }
    }

    */

    protected override void SubscribeTopics()
    {
        client.Subscribe(new string[] { topicSubscribe }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
    }

    protected override void UnsubscribeTopics()
    {
        client.Unsubscribe(new string[] { topicSubscribe });
    }
    /*
    protected override void OnConnectionFailed(string errorMessage)
    {
        AddUiMessage("CONNECTION FAILED! " + errorMessage);
    }

    protected override void OnDisconnected()
    {
        AddUiMessage("Disconnected.");
    }

    protected override void OnConnectionLost()
    {
        AddUiMessage("CONNECTION LOST!");
    }

    protected override void Start()
    {
        SetUiMessage("Ready.");
        updateUI = true;
        base.Start();
    }

    protected override void DecodeMessage(string topic, byte[] message)
    {
        string msg = System.Text.Encoding.UTF8.GetString(message);
        onMessageReceived.Invoke(msg);
        Debug.Log("Received: " + msg);
        StoreMessage(msg);
        if (topic == topicSubscribe)
        {
            if (autoTest)
            {
                autoTest = false;
                Disconnect();
            }
        }
    }

    private void StoreMessage(string eventMsg)
    {
        eventMessages.Add(eventMsg);
    }

    private void ProcessMessage(string msg)
    {
        AddUiMessage("Received: " + msg);
    }



    protected override void Update()
    {
        base.Update(); // call ProcessMqttEvents()

        if (eventMessages.Count > 0)
        {
            foreach (string msg in eventMessages)
            {
                ProcessMessage(msg);
            }
            eventMessages.Clear();
        }
        if (updateUI)
        {
            //UpdateUI();
            Debug.Log("UI could be updated");
        }
    }

     */

    private void OnDestroy()
    {
        Disconnect();
    }

    private void OnValidate()
    {
        if (autoTest)
        {
            autoConnect = true;
        }
    }

}
