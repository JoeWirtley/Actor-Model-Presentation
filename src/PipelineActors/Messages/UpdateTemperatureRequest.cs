using System;
using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class UpdateTemperatureRequest {
      public CorrelationId CorrelationId { get; }
      public double Temperature { get; }
      public DateTime Updated { get; }

      public UpdateTemperatureRequest( CorrelationId correlationID, double temperature, DateTime updated ) {
         CorrelationId = correlationID;
         Temperature = temperature;
         Updated = updated;
      }
   }
}