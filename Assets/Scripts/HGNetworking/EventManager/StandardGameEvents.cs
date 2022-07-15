using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class Event_SEND_USERNAME : Event
    {
        public string Username { get; set; }
    }

    public class Event_TEST_EVENT : Event
    {
        public string Username { get; set; }
        public int Number { get; set; }
        public Vector3 Vec { get; set; }
        public Quaternion Quat { get; set; }
        public string Test { get; set; }
    }

    public class Event_GIVE_CONTROL : Event
    {
        public int ghostId { get; set; }
        public int moveId { get; set; }
    }
    public class Event_KICK_BALL : Event
    {
        public Vector3 kickVector { get; set; }

        public int ghostId { get; set; }
    }
}
