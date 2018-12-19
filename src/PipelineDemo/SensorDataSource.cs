using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineDemo {
   internal class SensorDataSource {
      private readonly IActorRef _sensorArea;
      private readonly Random _randomTemperatureGenerator;
      private readonly int _sensorId;
      private IActorRef _sensor;
      private Timer _timer;

      public SensorDataSource( int sensorId, IActorRef sensorArea) {
         _sensorId = sensorId;
         _sensorArea = sensorArea;
         _randomTemperatureGenerator = new Random();
      }

      public async Task Start( TimeSpan updateEveryMilliseconds) {
         // Create the sensor
         var response = await _sensorArea.Ask<AddSensorResponse>( new AddSensorRequest( new CorrelationId(), _sensorId ) );
         _sensor = response.TemperatureSensor;
         _timer = new Timer( UpdateTemperature,null,TimeSpan.Zero, updateEveryMilliseconds );
      }


      private void UpdateTemperature( object sender ) {
         var randomTemperature = _randomTemperatureGenerator.NextDouble() * 250;
         _sensor.Ask( new UpdateTemperatureRequest(  new CorrelationId(), randomTemperature, DateTime.Now ) );
      }
   }
}