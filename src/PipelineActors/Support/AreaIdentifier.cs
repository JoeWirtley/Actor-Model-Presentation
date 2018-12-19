namespace PipelineActors.Support {
   public class AreaIdentifier {
      public string Value { get; }

      public AreaIdentifier( string value ) {
         Value = value;
      }

      public static implicit operator string( AreaIdentifier id ) {
         return id.Value;
      }

      public static implicit operator AreaIdentifier( string value ) {
         return new AreaIdentifier( value );
      }
   }
}