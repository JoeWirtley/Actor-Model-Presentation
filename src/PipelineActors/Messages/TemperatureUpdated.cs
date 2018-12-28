using System;
using PipelineActors.Support;

namespace PipelineActors.Messages {
   public class TemperatureUpdated {
      public SensorIdentifier SensorId { get; }
      public double Temperature { get; }
      public DateTime ReadingTime { get; }

      public TemperatureUpdated( SensorIdentifier sensorId, double temperature, DateTime readingTime ) {
         SensorId = sensorId;
         Temperature = temperature;
         ReadingTime = readingTime;
      }
   }
}