using Akka.Actor;
using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class AddSensorResponse {
      public CorrelationId CorrelationId { get; }
      public IActorRef TemperatureSensor { get; }

      public AddSensorResponse( CorrelationId correlationId, IActorRef temperatureSensor ) {
         CorrelationId = correlationId;
         TemperatureSensor = temperatureSensor;
      }
   }
}