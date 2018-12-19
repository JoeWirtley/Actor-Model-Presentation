using System;
using System.Linq;
using Akka.Actor;
using PipelineActors.Messages;
using static System.Console;

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
         CursorLeft = 0;
         CursorTop = 2;

         WriteLine( safetyNotification.Message + "\n" );

         WriteLine("Sensor ID\tTemp\tUpdated");
         foreach ( var temperature in safetyNotification.Temperatures.OrderBy( x => x.Key.ToString() ) ) {
            Write( $"{temperature.Key}\t" );
            double value = temperature.Value.Value;
            using ( new ScopedConsoleColor( value > 200 ? ConsoleColor.Yellow: ConsoleColor.White  )) {
               Write( $"{temperature.Value.Value:N0}\t" );
            }
            WriteLine( $"{temperature.Value.Update}" );
         }
      }

      public static Props Props() {
         return Akka.Actor.Props.Create( () => new SafetyWarningListener() );
      }
   }

   public class ScopedConsoleColor:IDisposable {
      private readonly ConsoleColor _oldColor;

      public ScopedConsoleColor( ConsoleColor newColor ) {
         _oldColor = ForegroundColor;

         ForegroundColor = newColor;
      }

      public void Dispose() {
         ForegroundColor = this._oldColor;
      }
   }

}