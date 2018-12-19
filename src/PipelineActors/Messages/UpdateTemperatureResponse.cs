using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class UpdateTemperatureResponse {
      public CorrelationId CorrelationId { get; }

      public UpdateTemperatureResponse( CorrelationId correlationId ) {
         CorrelationId = correlationId;
      }
   }
}