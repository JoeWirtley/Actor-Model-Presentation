using System;
using System.Threading.Tasks;
using Akka.Actor;
using PipelineActors.Actors;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineDemo {
   class Program {
      static async Task Main() {
         using ( var system = ActorSystem.Create( "pipeline" ) ) {
            IActorRef sensorArea = system.ActorOf( SensorAreaActor.Props( "North"), "north-area" );
            IActorRef safetyEvaluator = system.ActorOf( SafetyEvaluatorActor.Props( sensorArea ) );

            IActorRef listener = system.ActorOf( SafetyWarningListener.Props(), "safety-listener" );
            safetyEvaluator.Tell(  new SubscribeToNotificationsRequest( new CorrelationId() ), listener );

            Console.WriteLine( "Press 'q' to quit" );

            await CreateFakeSensorInput( sensorArea );

            while ( true ) {
               ConsoleKeyInfo cmd = Console.ReadKey();

               if ( cmd.KeyChar == 'q' || cmd.KeyChar == 'Q' ) {
                  Environment.Exit( 0 );
               }
            }
         }
      }

      private static async Task CreateFakeSensorInput( IActorRef sensorArea ) {
         for ( int i = 1; i < 6; i++ ) {
            var source = new SensorSource( i, sensorArea );
            await source.AddSensor();
            source.Start();
         }
      }
   }
}