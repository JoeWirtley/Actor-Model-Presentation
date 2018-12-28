using System;
using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class QueryTemperatureResponse {
      public SensorIdentifier SensorId { get; }
      public CorrelationId CorrelationId { get; }
      public double Temperature { get; }
      public DateTime ReadingTime { get; }

      public QueryTemperatureResponse( CorrelationId correlationID, SensorIdentifier sensorId, double temperature, DateTime readingTime ) {
         SensorId = sensorId;
         CorrelationId = correlationID;
         Temperature = temperature;
         ReadingTime = readingTime;
      }
   }
}