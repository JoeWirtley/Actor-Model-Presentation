using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class QueryTemperatureRequest {
      public CorrelationId CorrelationId { get; }

      public QueryTemperatureRequest( CorrelationId correlationId = null ) {
         if ( correlationId == null ) {
            correlationId = new CorrelationId();
         }
         CorrelationId = correlationId;
      }
   }
}