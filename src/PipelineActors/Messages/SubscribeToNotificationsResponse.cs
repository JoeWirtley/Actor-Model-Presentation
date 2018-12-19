using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class SubscribeToNotificationsResponse {
      public CorrelationId CorrelationId { get; }

      public SubscribeToNotificationsResponse( CorrelationId correlationId ) {
         CorrelationId = correlationId;
      }
   }
}