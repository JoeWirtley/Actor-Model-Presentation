namespace PipelineActors.Messages {
   public class SafetyNotification {
      public string Message { get; }

      public SafetyNotification( string message ) {
         Message = message;
      }
   }
}