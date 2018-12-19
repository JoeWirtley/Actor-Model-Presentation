using System;
using System.Collections.Immutable;
using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class SafetyEvaluationResult {
      public bool RequiresNotification => !string.IsNullOrEmpty( Message );
      public string Message { get; }
      public ImmutableDictionary<SensorIdentifier,Temperature> Temperatures { get; }

      public SafetyEvaluationResult( string message = "", 
         ImmutableDictionary<SensorIdentifier,Temperature> temperatures = null ) {
         Message = message;
         Temperatures = temperatures;
      }

      public class Temperature {
         public double Value { get; }
         public DateTime Update { get; }

         public Temperature( double value, DateTime update ) {
            Value = value;
            Update = update;
         }
      }
   }
}