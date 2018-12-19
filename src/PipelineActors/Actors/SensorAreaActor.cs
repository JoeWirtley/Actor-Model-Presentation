using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineActors.Actors {
   public class SensorAreaActor: UntypedActor {
      private readonly AreaIdentifier _id;
      private readonly Dictionary<SensorIdentifier, IActorRef> _sensors;
      private readonly HashSet<IActorRef> _subscribers;

      public SensorAreaActor( AreaIdentifier id ) {
         _id = id;
         _sensors = new Dictionary<SensorIdentifier, IActorRef>();
         _subscribers = new HashSet<IActorRef>();
      }

      protected override void OnReceive( object message ) {
         switch ( message ) {
            case AddSensorRequest m:
               if ( !_sensors.TryGetValue( m.SensorId, out var sensor ) ) {
                  sensor = Context.ActorOf( TemperatureSensorActor.Props( m.SensorId ) );
                  _sensors.Add( m.SensorId, sensor );
                  sensor.Tell( new SubscribeToUpdatesRequest( new CorrelationId() ) );
               }

               Sender.Tell( new AddSensorResponse( m.CorrelationId, sensor ) );
               break;

            case SubscribeToUpdatesRequest m:
               _subscribers.Add( Sender );
               Sender.Tell( new SubscribeToUpdatesResponse( m.CorrelationId ) );
               break;

            case TemperatureUpdated m:
               foreach ( var subscriber in _subscribers ) {
                  subscriber.Forward( m );
               }
               break;

            case GetAllSensorsRequest m:
               Sender.Tell( new GetAllSensorsResponse( m.CorrelationId, _sensors.ToImmutableDictionary() )  );
               break;
         }
      }

      public static Props Props( AreaIdentifier id ) =>
         Akka.Actor.Props.Create( () => new SensorAreaActor( id ) );
   }
}