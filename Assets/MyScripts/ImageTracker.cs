using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

public class ImageTracker : MonoBehaviour
{
    public mqttReceiver_test _eventSender;

    public ARTrackedImageManager aRTrackedImageManager;

    public TextMeshProUGUI textField;
    public TextMeshProUGUI textFieldLost;
    public TextMeshProUGUI textFieldTest;

    private GameObject thermometer_obj;

    public List<ImageAction> imageActions;
    private Dictionary<string, UnityEvent> imageActionMap;

    public List<string> imageNames = new List<string>();
    public Button licht;
    private bool lichtStatus = false;

    void Start()
    {
        //_eventSender.onMessageReceived.AddListener(HandleTemperatureChanged);
        licht.onClick.AddListener(OnLichtschalterClick);
        licht.gameObject.SetActive(false);

    }
        private void Awake()
    {
        aRTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        imageActionMap = new Dictionary<string, UnityEvent>();
        foreach (var imageAction in imageActions)
        {
            imageActionMap.Add(imageAction.imageName, imageAction.action);
        }
    }

    private void OnEnable()
    {
        //aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
        aRTrackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }

    private void OnDisable()
    {
        //aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
        aRTrackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
    }

    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        //textField.text = "Method called";
        // textFieldLost.text = "Method called";
        //textFieldTest.text = args.addedtrackedImage.referenceImage.name;

       

        thermometer_obj = GameObject.FindWithTag("Thermometer");
        //thermometer_obj.SetActive(true);
      //  licht.gameObject.SetActive(false);
        foreach (var trackedImage in args.added)
         {
            textFieldTest.text = "Added";
             //if(imageActionMap.TryGetValue(trackedImage.referenceImage.name, out UnityEvent action))
             //{
            string addedName = trackedImage.referenceImage.name;
                 textField.text = addedName;

            string lastTrackedImage = "";

            // Inside your loop for added or updated images


            
             if (addedName == imageNames[0] && trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
             {
                 thermometer_obj.SetActive(true);
                licht.gameObject.SetActive(false);
                textField.text = trackedImage.trackingState.ToString();
                textFieldLost.text = addedName;

            }

             if (addedName == imageNames[1] && trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                     {
                 licht.gameObject.SetActive(true);
                //  thermometer_obj.SetActive(false);
                disableObjects("Thermometer");
                 textField.text = trackedImage.trackingState.ToString();
                textFieldLost.text = addedName;

            }
             


            // action.Invoke();
            //}
        }
        

  
        foreach (var trackedImage in args.updated)
        {
            textFieldTest.text = "Updated";
            //if(imageActionMap.TryGetValue(trackedImage.referenceImage.name, out UnityEvent action))
            //{
            string updatedName = trackedImage.referenceImage.name;
            textField.text = updatedName;

            if (updatedName == imageNames[0] && trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
               thermometer_obj.SetActive(true);
               licht.gameObject.SetActive(false);
            }


            if (updatedName == imageNames[0] && trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
            {
                //disableObjects("Thermometer");
            }

            if (updatedName == imageNames[1] && trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                licht.gameObject.SetActive(true);
                disableObjects("Thermometer");
                // thermometer_obj.SetActive(false);

            }


            if (updatedName == imageNames[1] && trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
            {
                licht.gameObject.SetActive(false);
            }
            else
            {
               //licht.gameObject.SetActive(false);
              // thermometer_obj.SetActive(false);
            }
            /*if (updatedName == imageNames[1])
            {
                while (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    licht.gameObject.SetActive(true);
                }
                licht.gameObject.SetActive(false);
            }
     */


            textField.text = trackedImage.trackingState.ToString();
            //}
        }

        foreach (var trackedImage in args.removed)
        {
            textFieldTest.text = "Removed";
            string removedName = trackedImage.referenceImage.name;
            textFieldLost.text = "Removed: " + removedName;

            if (removedName == imageNames[0])
            {
                thermometer_obj.SetActive(false);
                textFieldLost.text = removedName;
                textField.text = trackedImage.trackingState.ToString();
            }

            if (removedName == imageNames[1])
            {
                licht.gameObject.SetActive(false);
                textField.text = trackedImage.trackingState.ToString();
                textFieldLost.text = removedName;

            }


        }

        //thermometer_obj = trackedImage.transform.GetChild(0).gameObject;
        /* fieldOfText.text = trackedImage.trackingState.ToString();
      //  if (trackedImage.trackingState == TrackingState.Tracking || trackedImage.trackingState == TrackingState.Limited)
       // {
             string imageName = trackedImage.referenceImage.name;
             fieldOfText.text = imageName;
         if (imageName == "thermometer")
         {

         }
         //}*/
    }

    public void HandleTemperatureChanged(string newTemperature)
    {
        //   Debug.Log("Neue Nachricht: " + newMessage);
        //textField.text = newTemperature;
        textField.text = thermometer_obj.name;
        float temperature_float = float.Parse(newTemperature);
        //newTemperature = newTemperature.ToFloat();
      
       
      //  if(thermometer_obj != null)
       // {
            float y_scale = (temperature_float / 100) * 10;
            Vector3 newScale = new Vector3 (100, y_scale, 100);
            thermometer_obj.transform.localScale = newScale;
        //}


    }

    [System.Serializable]
    public class ImageAction
    {
        public string imageName;
        public UnityEvent action;
    }

    private void OnLichtschalterClick()
    {
        string lichtAn = "on";
        string lichtAus = "off";
        string topic = "M2MQTT_Unity/licht";

        lichtStatus = !lichtStatus;
        if (lichtStatus)
        {
            SendIt(lichtAn, topic);
        }
        if (!lichtStatus)
        {
            SendIt(lichtAus, topic);
        }
    }

    public void SendIt(string messageToSend, string topic)
    {
        _eventSender.messagePublish = messageToSend;
        //_eventSender.TestPublish(messageToSend.ToString());
        //_eventSender.TestPublish2();
        _eventSender.TestPublish4(topic);
    }

    public void SendIt(string messageToSend)
    {
        string topic = "M2MQTT_Unity/licht";
        _eventSender.messagePublish = messageToSend;
        //_eventSender.TestPublish(messageToSend.ToString());
        //_eventSender.TestPublish2();
        _eventSender.TestPublish4(topic);
    }

    public void disableObjects(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);

        // Loop through the array and set each GameObject inactive
        foreach (GameObject obj in taggedObjects)
        {
            obj.SetActive(false);
        }
    }

    public void enableObjects(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);

        // Loop through the array and set each GameObject inactive
        foreach (GameObject obj in taggedObjects)
        {
            obj.SetActive(true);
        }
    }
}
