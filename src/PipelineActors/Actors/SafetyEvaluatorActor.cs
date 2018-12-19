using System.Collections.Generic;
using Akka.Actor;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineActors.Actors {
   public class SafetyEvaluatorActor: UntypedActor {
      private readonly IActorRef _areaActor;
      private readonly HashSet<IActorRef> _subscribers;

      public SafetyEvaluatorActor( IActorRef areaActor ) {
         _areaActor = areaActor;
         _areaActor.Tell( new SubscribeToUpdatesRequest( new CorrelationId() ) );
         _subscribers = new HashSet<IActorRef>();
      }

      protected override void OnReceive( object message ) {
         switch ( message ) {
            case SubscribeToNotificationsRequest m:
               _subscribers.Add( Sender );
               Sender.Tell( new SubscribeToNotificationsResponse( m.CorrelationId ) );
               break;

            case TemperatureUpdated m:
               if ( m.Temperature > 200 ) {
                  PublishNotification( "Temperature exceeds 200 degrees" );
               }
               break;

            default:
               Unhandled( message );
               break;
         }
      }

      private void PublishNotification( string message ) {
         var warning = new SafetyNotification(  message );
         foreach ( var subscriber in _subscribers ) {
            subscriber.Tell( warning );
         }
      }

      public static Props Props( IActorRef areaActor ) =>
         Akka.Actor.Props.Create( () => new SafetyEvaluatorActor( areaActor ) );
   }
}