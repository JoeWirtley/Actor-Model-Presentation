using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class SubscribeToUpdatesRequest {
      public CorrelationId CorrelationId { get; }

      public SubscribeToUpdatesRequest( CorrelationId correlationId ) {
         CorrelationId = correlationId;
      }
   }
}