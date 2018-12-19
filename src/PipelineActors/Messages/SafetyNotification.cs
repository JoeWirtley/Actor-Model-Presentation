using System.Collections.Immutable;
using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class SafetyNotification {
      public string Message { get; }
      public ImmutableDictionary<SensorIdentifier,SafetyEvaluationResult.Temperature> Temperatures { get; }

      public SafetyNotification( string message, ImmutableDictionary<SensorIdentifier,SafetyEvaluationResult.Temperature> temperatures ) {
         Message = message;
         Temperatures = temperatures;
      }
   }
}