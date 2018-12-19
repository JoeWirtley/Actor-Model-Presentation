using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class GetAllSensorsRequest {
      public CorrelationId CorrelationId { get; }

      public GetAllSensorsRequest( CorrelationId correlationId = null ) {
         if ( correlationId == null ) {
            correlationId = new CorrelationId();
         }
         CorrelationId = correlationId;
      }
   }
}