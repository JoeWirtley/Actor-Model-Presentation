using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class SubscribeToNotificationsRequest {
      public CorrelationId CorrelationId { get; }

      public SubscribeToNotificationsRequest( CorrelationId correlationId ) {
         CorrelationId = correlationId;
      }
   }
}