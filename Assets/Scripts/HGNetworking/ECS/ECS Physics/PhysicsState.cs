using UnityEngine;
public class PhysicsState
{
    public Vector3 position, velocity, angularVelocity;
    public Quaternion rotation;

    public PhysicsState(Quaternion rotation, Vector3 position, Vector3 velocity, Vector3 angularVelocity)
    {
        this.rotation = rotation;
        this.position = position;
        this.velocity = velocity;
        this.angularVelocity = angularVelocity;
    }
    
    public PhysicsState(Rigidbody rigidbody)
    {
        this.rotation = rigidbody.rotation;
        this.position = rigidbody.position;
        this.velocity = rigidbody.velocity;
        this.angularVelocity = rigidbody.angularVelocity;
    }
}
