using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Akka.Actor;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineActors.Actors {
   public class ExecuteSafetyEvaluationActor: UntypedActor {
      private readonly IActorRef _temperatureDataSource;
      private readonly IActorRef _replyTo;
      private readonly Dictionary<SensorIdentifier, QueryTemperatureResponse> _results;
      private readonly HashSet<SensorIdentifier> _toQuery;

      public ExecuteSafetyEvaluationActor( IActorRef temperatureDataSource, IActorRef replyTo ) {
         _temperatureDataSource = temperatureDataSource;
         _replyTo = replyTo;
         _results = new Dictionary<SensorIdentifier, QueryTemperatureResponse>();
         _toQuery = new HashSet<SensorIdentifier>();
      }

      protected override void PreStart() {
         _temperatureDataSource.Tell( new GetAllSensorsRequest() );
      }

      protected override void OnReceive( object message ) {
         switch ( message ) {
            case GetAllSensorsResponse m:
               foreach ( var sensor in m.Sensors ) {
                  _toQuery.Add( sensor.Key );
                  sensor.Value.Tell( new QueryTemperatureRequest() );
               }
               break;

            case QueryTemperatureResponse m:
               _results.Add( m.SensorId, m );
               _toQuery.Remove( m.SensorId );
               if ( _toQuery.Count == 0 ) {
                  PerformEvaluation();
               }
               break;
         }
      }

      private void PerformEvaluation() {
         var temperatures =
         _results.ToDictionary( 
            item => item.Key, 
            item => new SafetyEvaluationResult.Temperature( item.Value.Temperature, item.Value.Updated ) )
            .ToImmutableDictionary();

         if ( _results.Values.Any( response => response.Temperature > 200 ) ) {
            _replyTo.Tell( new SafetyEvaluationResult( "Temperature exceeds 200 degrees", temperatures ) );
         } else {
            _replyTo.Tell( new SafetyEvaluationResult() );
         }

         Context.Stop( Self );
      }

      public static Props Props( IActorRef temperatureSource, IActorRef replyTo ) =>
         Akka.Actor.Props.Create( () => new ExecuteSafetyEvaluationActor( temperatureSource, replyTo ) );
   }
}