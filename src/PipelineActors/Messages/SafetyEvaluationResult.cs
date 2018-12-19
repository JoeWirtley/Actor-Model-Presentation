namespace PipelineActors.Messages {
   public class SafetyEvaluationResult {
      public bool RequiresNotification => !string.IsNullOrEmpty( Message );
      public string Message { get; }

      public SafetyEvaluationResult( string message = "" ) {
         Message = message;
      }
   }
}