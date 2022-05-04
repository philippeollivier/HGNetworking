using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Send");
            using (Packet _packet = new Packet())
            {
                _packet.Write("Your mum gay");
                PlatformPacketManager.SendPacket(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6943), _packet);

            }
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            PlatformPacketManager.OpenServer(1, 6942);
        }
    }
}
