using extOSC;
using UnityEngine;

public class TestOSC : MonoBehaviour
{
    OSCReceiver receiver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        receiver = gameObject.GetComponent<OSCReceiver>();
        receiver.LocalPort = 7001;
        receiver.Bind("/mousex", (message) =>  {
            Debug.Log("Received OSC message: " + $"x: {message.Values[0].FloatValue}" );
        });
        receiver.Bind("/mousey", (message) =>
        {
            message.ToFloat(out var value);
            Debug.Log("Received OSC message: " + $"y: {value}" );
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
