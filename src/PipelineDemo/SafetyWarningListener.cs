using System;
using System.Linq;
using Akka.Actor;
using PipelineActors.Messages;

namespace PipelineDemo {
   public class SafetyWarningListener: UntypedActor {
      protected override void OnReceive( object message ) {
         switch ( message ) {
            case SafetyNotification m:
               ShowResults( m );
               break;
         }
      }

      private void ShowResults( SafetyNotification safetyNotification ) {
         Console.CursorLeft = 0;
         Console.CursorTop = 2;

         Console.WriteLine( safetyNotification.Message );
         foreach ( var temperature in safetyNotification.Temperatures.OrderBy( x => x.Key.ToString() ) ) {
            Console.WriteLine( $"{temperature.Key}\t{temperature.Value.Value:N0}\t{temperature.Value.Update}" );
         }
      }

      public static Props Props() {
         return Akka.Actor.Props.Create( () => new SafetyWarningListener() );
      }
   }
}