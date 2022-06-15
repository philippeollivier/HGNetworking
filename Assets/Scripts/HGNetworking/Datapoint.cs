public class Datapoint
{
    public float Time { get; set; }
    public float Value { get; set; }

    public Datapoint(float Time, float Value){
        this.Time = Time;
        this.Value = Value;
    }
}
