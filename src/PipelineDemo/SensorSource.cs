﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineDemo {
   internal class SensorSource {
      private readonly IActorRef _sensorArea;
      private readonly Random _randomTemperatureGenerator;
      private readonly int _sensorId;
      private IActorRef _sensor;
      private Timer _timer;

      public SensorSource( int sensorId, IActorRef sensorArea ) {
         _sensorId = sensorId;
         _sensorArea = sensorArea;
         _randomTemperatureGenerator = new Random();
      }

      public async Task AddSensor() {
         var response = await _sensorArea.Ask<AddSensorResponse>( new AddSensorRequest( new CorrelationId(), _sensorId ) );

         _sensor = response.TemperatureSensor;
      }

      public void Start() {
         _timer = new Timer( UpdateTemperature, null, 0, 500 );
      }

      private void UpdateTemperature( object sender ) {
         var randomTemperature = _randomTemperatureGenerator.NextDouble() * 250;
         _sensor.Ask( new UpdateTemperatureRequest(  new CorrelationId(), randomTemperature, DateTime.Now ) );
      }
   }
}