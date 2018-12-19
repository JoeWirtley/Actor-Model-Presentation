using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class QueryTemperatureRequest {
      public CorrelationId CorrelationId { get; }

      public QueryTemperatureRequest( CorrelationId correlationId ) {
         CorrelationId = correlationId;
      }
   }
}