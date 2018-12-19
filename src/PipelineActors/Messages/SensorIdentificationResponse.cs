using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class SensorIdentificationResponse {
      public CorrelationId CorrelationId { get; }
      public SensorIdentifier SensorIdentifier { get; }

      public SensorIdentificationResponse( CorrelationId correlationId, SensorIdentifier sensorIdentifier ) {
         CorrelationId = correlationId;
         SensorIdentifier = sensorIdentifier;
      }
   }
}