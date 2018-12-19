using System;
using System.Collections.Generic;
using Akka.Actor;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineActors.Actors {
   public class TemperatureSensorActor: UntypedActor {
      private readonly SensorIdentifier _id;
      private double _temperature;
      private DateTime _updated;
      private readonly HashSet<IActorRef> _subscribers;

      public TemperatureSensorActor( SensorIdentifier id ) {
         _id = id;
         _subscribers = new HashSet<IActorRef>();
      }

      protected override void OnReceive( object message ) {
         switch ( message ) {
            case SensorIdentificationRequest m:
               Sender.Tell( new SensorIdentificationResponse(  m.CorrelationId, _id ) );
               break;

            case UpdateTemperatureRequest m:
               _temperature = m.Temperature;
               _updated = m.Updated;
               Sender.Tell( new UpdateTemperatureResponse( m.CorrelationId ) );
               UpdateSubscribers();
               break;

            case QueryTemperatureRequest m:
               Sender.Tell( new QueryTemperatureResponse( m.CorrelationId, _id, _temperature, _updated ) );
               break;

            case SubscribeToUpdatesRequest m:
               _subscribers.Add( Sender );
               Sender.Tell( new SubscribeToUpdatesResponse( m.CorrelationId ) );
               break;

            default:
               Unhandled( message );
               break;
         }
      }

      private void UpdateSubscribers() {
         TemperatureUpdated updateMessage = new TemperatureUpdated( _id, _temperature, _updated );
         foreach ( var subscriber in _subscribers ) {
            subscriber.Tell( updateMessage  );
         }
      }

      public static Props Props( SensorIdentifier id ) =>
         Akka.Actor.Props.Create( () => new TemperatureSensorActor( id ) );
   }
}