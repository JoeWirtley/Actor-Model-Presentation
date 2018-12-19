using System;
using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class TemperatureUpdated {
      public SensorIdentifier SensorId { get; }
      public double Temperature { get; }
      public DateTime Updated { get; }

      public TemperatureUpdated( SensorIdentifier sensorId, double temperature, DateTime updated ) {
         SensorId = sensorId;
         Temperature = temperature;
         Updated = updated;
      }
   }
}