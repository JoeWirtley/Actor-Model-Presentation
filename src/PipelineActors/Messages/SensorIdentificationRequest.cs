using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class SensorIdentificationRequest {
      public CorrelationId CorrelationId { get; }

      public SensorIdentificationRequest( CorrelationId correlationId ) {
         CorrelationId = correlationId;
      }
   }
}