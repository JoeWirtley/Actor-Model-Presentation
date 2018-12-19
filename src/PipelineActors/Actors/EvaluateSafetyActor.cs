using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineActors.Actors {
   public class EvaluateSafetyActor: UntypedActor {
      private readonly IActorRef _temperatureDataSource;
      private readonly IActorRef _replyTo;
      private readonly Dictionary<SensorIdentifier, QueryTemperatureResponse> _results;
      private readonly HashSet<SensorIdentifier> _toQuery;

      public EvaluateSafetyActor( IActorRef temperatureDataSource, IActorRef replyTo ) {
         _temperatureDataSource = temperatureDataSource;
         _replyTo = replyTo;
         _results = new Dictionary<SensorIdentifier, QueryTemperatureResponse>();
         _toQuery = new HashSet<SensorIdentifier>();
      }

      protected override void PreStart() {
         _temperatureDataSource.Tell( new GetAllSensorsRequest( new CorrelationId() ) );
      }

      protected override void OnReceive( object message ) {
         switch ( message ) {
            case GetAllSensorsResponse m:
               foreach ( var sensor in m.Sensors ) {
                  _toQuery.Add( sensor.Key );
                  sensor.Value.Tell( new QueryTemperatureRequest( new CorrelationId() ) );
               }

               break;

            case QueryTemperatureResponse m:
               _results.Add( m.SensorId, m );
               _toQuery.Remove( m.SensorId );
               if ( _toQuery.Count == 0 ) {
                  Evaluate();
               }

               break;
         }
      }

      private void Evaluate() {
         if ( _results.Values.Any( response => response.Temperature > 200 ) ) {
            _replyTo.Tell( new SafetyEvaluationResult( "Temperature exceeds 200 degrees" ) );
         } else {
            _replyTo.Tell( new SafetyEvaluationResult() );
         }

         Context.Stop( Self );
      }

      public static Props Props( IActorRef temperatureSource, IActorRef replyTo ) =>
         Akka.Actor.Props.Create( () => new EvaluateSafetyActor( temperatureSource, replyTo ) );
   }
}