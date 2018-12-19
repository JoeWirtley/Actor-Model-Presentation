using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class SubscribeToUpdatesResponse {
      public CorrelationId CorrelationId { get; }

      public SubscribeToUpdatesResponse( CorrelationId correlationId ) {
         CorrelationId = correlationId;
      }
   }
}