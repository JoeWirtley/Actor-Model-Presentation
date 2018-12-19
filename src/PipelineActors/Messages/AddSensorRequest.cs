using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class AddSensorRequest {
      public CorrelationId CorrelationId { get; }
      public SensorIdentifier SensorId { get; }

      public AddSensorRequest( CorrelationId correlationId, SensorIdentifier sensorId ) {
         CorrelationId = correlationId;
         SensorId = sensorId;
      }
   }
}