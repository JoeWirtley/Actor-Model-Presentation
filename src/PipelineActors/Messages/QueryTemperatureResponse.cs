using System;
using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class QueryTemperatureResponse {
      public SensorIdentifier SensorId { get; }
      public CorrelationId CorrelationId { get; }
      public double Temperature { get; }
      public DateTime Updated { get; }

      public QueryTemperatureResponse( CorrelationId correlationID, SensorIdentifier sensorId, double temperature, DateTime updated ) {
         SensorId = sensorId;
         CorrelationId = correlationID;
         Temperature = temperature;
         Updated = updated;
      }
   }
}