using System;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using PipelineActors.Actors;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineActors.Tests {
   [TestFixture]
   public class SensorAreaTests: TestKit {
      [Test]
      public void ShouldAddNewSensorWhenNotPreviouslyAdded() {
         var probe = CreateTestProbe();
         var area = CreateTestArea( "North" );

         CorrelationId correlationId = new CorrelationId();
         SensorIdentifier sensorId = new SensorIdentifier( 2 );

         area.Tell( new AddSensorRequest( correlationId, sensorId ), probe.Ref );

         var received = probe.ExpectMsg<AddSensorResponse>();

         received.CorrelationId.Should().Be( correlationId );
         received.TemperatureSensor.Should().NotBeNull();
      }

      [Test]
      public void ShouldReturnExistingSensorWhenPreviouslyAdded() {
         var probe = CreateTestProbe();
         var area = CreateTestArea( "North" );

         SensorIdentifier sensorId = new SensorIdentifier( 2 );

         area.Tell( new AddSensorRequest( new CorrelationId(), sensorId ), probe.Ref );

         var received = probe.ExpectMsg<AddSensorResponse>();
         IActorRef sensor = received.TemperatureSensor;

         area.Tell( new AddSensorRequest( new CorrelationId(), sensorId ), probe.Ref );
         received = probe.ExpectMsg<AddSensorResponse>();
         received.TemperatureSensor.Should().Be( sensor );
      }

      [Test]
      public void ShouldSubscribeToChanges() {
         var probe = CreateTestProbe();
         var area = CreateTestArea( "South" );

         CorrelationId expectedCorrelationId = new CorrelationId();
         area.Tell( new SubscribeToUpdatesRequest( expectedCorrelationId ), probe.Ref );

         var received = probe.ExpectMsg<SubscribeToUpdatesResponse>();

         received.CorrelationId.Should().Be( expectedCorrelationId );
      }


      [Test]
      public void AfterSubscriptionShouldReceiveTemperatureUpdates() {
         var probe = CreateTestProbe();
         var area = CreateTestArea( "North" );

         SensorIdentifier sensorId = new SensorIdentifier( 2 );
         area.Tell( new AddSensorRequest( new CorrelationId(), sensorId ), probe.Ref );
         IActorRef sensor = probe.ExpectMsg<AddSensorResponse>().TemperatureSensor;

         area.Tell( new SubscribeToUpdatesRequest( new CorrelationId() ), probe.Ref );
         probe.ExpectMsg<SubscribeToUpdatesResponse>();

         double expectedTemperature = 98.4;
         DateTime expectedUpdated = DateTime.Now;

         sensor.Tell( new UpdateTemperatureRequest( new CorrelationId(), expectedTemperature, expectedUpdated )  );

         var received = probe.ExpectMsg<TemperatureUpdated>();
         received.SensorId.Should().Be( sensorId );
         received.Temperature.Should().Be( expectedTemperature );
         received.Updated.Should().Be( expectedUpdated );
      }

      private IActorRef CreateTestArea( AreaIdentifier areaIdentifier = null ) {
         if ( areaIdentifier is null ) {
            areaIdentifier = new AreaIdentifier( "North" );
         }

         return Sys.ActorOf( SensorAreaActor.Props( areaIdentifier ) );
      }
   }
}