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
            using (Packet _packet = new Packet())
            {
                _packet.Write("Your mum gay");
                PlatformPacketManager.SendPacket(new System.Net.IPEndPoint(IPAddress.Parse("25.18.58.72"), 6242), _packet);

            }
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            PlatformPacketManager.OpenServer(1, 6942);
        }
    }
}
