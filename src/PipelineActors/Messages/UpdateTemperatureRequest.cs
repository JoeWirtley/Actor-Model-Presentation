using System;
using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class UpdateTemperatureRequest {
      public CorrelationId CorrelationId { get; }
      public double Temperature { get; }
      public DateTime ReadingTime { get; }

      public UpdateTemperatureRequest( CorrelationId correlationID, double temperature, DateTime readingTime ) {
         CorrelationId = correlationID;
         Temperature = temperature;
         ReadingTime = readingTime;
      }
   }
}