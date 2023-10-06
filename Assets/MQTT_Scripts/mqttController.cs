using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class mqttController : MonoBehaviour
{
    public string nameController = "Controller 1";
    public string tagOfTheMQTTReceiver = "Receiver";
    public mqttReceiver_test _eventSender;

    private TextMeshProUGUI fieldOfText;

   

    void Start()
    {
        fieldOfText = GetComponent<TextMeshProUGUI>();
        //rawImage = GetComponent<RawImage>();
        

        _eventSender = GameObject.FindGameObjectsWithTag(tagOfTheMQTTReceiver)[0].gameObject.GetComponent<mqttReceiver_test>();
        if(_eventSender != null)
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
        if (fieldOfText != null)
        {
            Debug.Log("Neue Nachricht: " + newMessage);
            fieldOfText.text = newMessage;
        }
      

    }

    public void HandleButtonPressed(string buttonPressed)
    {

        GameObject plane = GameObject.FindWithTag("plane_for_color");
        if (buttonPressed == "btn_1") 
        {
            
            plane.GetComponent<rawImage_functions>().ChangeColor();

            Debug.Log("Neue Nachricht: " + buttonPressed);
        }

        if (buttonPressed == "btn_2")
        {

            plane.GetComponent<rawImage_functions>().ChangeScale();

            Debug.Log("Neue Nachricht: " + buttonPressed);
        }
    }

    

}