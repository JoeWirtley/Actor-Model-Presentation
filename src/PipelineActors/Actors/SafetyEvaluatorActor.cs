using System;
using System.Collections.Generic;
using Akka.Actor;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineActors.Actors {
   public class SafetyEvaluatorActor: UntypedActor, IWithUnboundedStash {
      private readonly IActorRef _areaActor;
      private readonly HashSet<IActorRef> _subscribers;
      public IStash Stash { get; set; }

      public SafetyEvaluatorActor( IActorRef areaActor, Action createEvaluation ) {
         _areaActor = areaActor;
         _subscribers = new HashSet<IActorRef>();
         if ( createEvaluation == null ) {
            CreateEvaluationActor = () => Context.ActorOf( EvaluateSafetyActor.Props( _areaActor, Self ) );
         } else {
            CreateEvaluationActor = createEvaluation;
         }
      }

      private Action CreateEvaluationActor { get; }

      protected override void PreStart() {
         _areaActor.Tell( new SubscribeToUpdatesRequest( new CorrelationId() ) );
      }

      protected override void OnReceive( object message ) {
         switch ( message ) {
            case SubscribeToNotificationsRequest m:
               AddSubscriber( m );
               break;

            case TemperatureUpdated m:
               Become( Evaluating );
               CreateEvaluationActor( );
               break;
         }
      }

      private void Evaluating( object message ) {
         switch ( message ) {
            case SubscribeToNotificationsRequest m:
               AddSubscriber( m );
               break;

            case TemperatureUpdated m:
               Stash.Stash();
               Become( EvaluatingWithRequest );
               break;

            case SafetyEvaluationResult m:
               HandleEvaluationResult( m );
               break;
         }
      }

      private void EvaluatingWithRequest( object message ) {
         switch ( message ) {
            case SubscribeToNotificationsRequest m:
               AddSubscriber( m );
               break;

            case TemperatureUpdated m:
               break;

            case SafetyEvaluationResult m:
               HandleEvaluationResult( m );
               break;
         }

      }

      private void AddSubscriber( SubscribeToNotificationsRequest m ) {
         _subscribers.Add( Sender );
         Sender.Tell( new SubscribeToNotificationsResponse( m.CorrelationId ) );
      }

      private void HandleEvaluationResult( SafetyEvaluationResult evaluationResult) {
         if ( evaluationResult.RequiresNotification ) {
            var warning = new SafetyNotification( evaluationResult.Message );
            foreach ( var subscriber in _subscribers ) {
               subscriber.Tell( warning );
            }
         }
         Become( OnReceive );
         Stash.UnstashAll();
      }

      public static Props Props( IActorRef areaActor, Action createEvaluation = null ) {
         if ( createEvaluation != null ) {
            return Akka.Actor.Props.Create( () => new SafetyEvaluatorActor( areaActor, createEvaluation ) );
         }
         return Akka.Actor.Props.Create( () => new SafetyEvaluatorActor( areaActor, null ) );
      }
   }
}