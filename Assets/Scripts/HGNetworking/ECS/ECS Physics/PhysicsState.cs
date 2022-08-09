using UnityEngine;
public class PhysicsState
{
    public Vector3 localPosition, velocity, angularVelocity;
    public Quaternion rotation;

    public PhysicsState(Quaternion rotation, Vector3 localPosition, Vector3 velocity, Vector3 angularVelocity)
    {
        this.rotation = rotation;
        this.localPosition = localPosition;
        this.velocity = velocity;
        this.angularVelocity = angularVelocity;
    }
    
    public PhysicsState(Rigidbody rigidbody)
    {
        this.rotation = rigidbody.rotation;
        this.localPosition = rigidbody.position;
        this.velocity = rigidbody.velocity;
        this.angularVelocity = rigidbody.angularVelocity;
    }
}
