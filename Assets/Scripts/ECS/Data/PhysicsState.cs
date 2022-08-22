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
        rotation = rigidbody.rotation;
        position = rigidbody.position;
        velocity = rigidbody.velocity;
        angularVelocity = rigidbody.angularVelocity;
    }

    public override string ToString()
    {
        return $"pos: {position} vel: {velocity} angVel: {angularVelocity} rot: {rotation}";
    }
}
