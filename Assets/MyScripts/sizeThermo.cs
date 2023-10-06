using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class sizeThermo : MonoBehaviour
{
    public string nameController = "Controller 1";
    public string tagOfTheMQTTReceiver = "Receiver";
    public mqttReceiver_test _eventSender;

    private TextMeshProUGUI fieldOfText;

    private Transform thermometer_model;
    void Start()
    {
        //thermometer_model = this.transform.GetChild(0);
        thermometer_model = this.transform;
        fieldOfText = GetComponent<TextMeshProUGUI>();

        _eventSender = GameObject.FindGameObjectsWithTag(tagOfTheMQTTReceiver)[0].gameObject.GetComponent<mqttReceiver_test>();
        if (_eventSender != null)
        {
            Debug.Log("Sender found!");
            _eventSender.onMessageReceived.AddListener(HandleMessageChanged);
            // _eventSender.OnMessageArrived += OnMessageArrivedHandler;

        }
        _eventSender.OnMessageChanged += HandleMessageChanged;
    }

    void OnDestroy()
    {
        _eventSender.onMessageReceived.RemoveListener(HandleMessageChanged);
    }

    private void OnMessageArrivedHandler(string newMsg)
    {
        Debug.Log("Event Fired. The message, from Object " + nameController + " is = " + newMsg);
    }

    public void HandleMessageChanged(string newMessage)
    {
        string currentTemperature = "No Temperature yet";
        string newTemperature = newMessage;

        if (currentTemperature != newTemperature)
        {

        float temperature_float = float.Parse(newTemperature);
            //newTemperature = newTemperature.ToFloat();
            currentTemperature = newTemperature;

            //  if(thermometer_obj != null)
            // {
            float z_scale = (temperature_float / 10000);
        //Vector3 newScale = new Vector3(1, 1, z_scale);
        //thermometer_model.localScale = new Vector3(thermometer_model.localScale.x, thermometer_model.localScale.y, z_scale);

        }
    }

}