using System.Collections.Immutable;
using Akka.Actor;
using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class GetAllSensorsResponse {
      public CorrelationId CorrelationId { get; }
      public IImmutableDictionary<SensorIdentifier, IActorRef> Sensors { get; }

      public GetAllSensorsResponse( CorrelationId correlationId, 
         IImmutableDictionary<SensorIdentifier, IActorRef> sensors ) {
         CorrelationId = correlationId;
         Sensors = sensors;
      }
   }
}