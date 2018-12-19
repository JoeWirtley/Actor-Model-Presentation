namespace PipelineActors.Support {
   public class SensorIdentifier {
      public int Value { get; }

      public SensorIdentifier( int value ) {
         Value = value;
      }

      public static implicit operator int( SensorIdentifier id ) {
         return id.Value;
      }

      public static implicit operator SensorIdentifier( int value ) {
         return new SensorIdentifier( value );
      }
   }
}