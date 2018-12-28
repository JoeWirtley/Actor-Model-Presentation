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
         DateTime expectedReadingTime = DateTime.Now;

         sensor.Tell( new UpdateTemperatureRequest( new CorrelationId(), expectedTemperature, expectedReadingTime )  );

         var received = probe.ExpectMsg<TemperatureUpdated>();
         received.SensorId.Should().Be( sensorId );
         received.Temperature.Should().Be( expectedTemperature );
         received.ReadingTime.Should().Be( expectedReadingTime );
      }

      [Test]
      public void ShouldRespondToGetAllSensorsRequest() {
         var probe = CreateTestProbe();
         var area = CreateTestArea( "North" );

         AddSensorToArea( area, 1 );
         AddSensorToArea( area, 2 );
         AddSensorToArea( area, 3 );

         var correlationId = new CorrelationId();
         area.Tell( new GetAllSensorsRequest( correlationId ), probe.Ref );

         var received = probe.ExpectMsg<GetAllSensorsResponse>();
         received.CorrelationId.Should().Be( correlationId );
         received.Sensors.Should().HaveCount( 3 );
         received.Sensors.Keys.Should().BeEquivalentTo( new SensorIdentifier( 1 ), new SensorIdentifier( 2 ), new SensorIdentifier( 3 ) );
      }

      private IActorRef CreateTestArea( AreaIdentifier areaIdentifier = null ) {
         if ( areaIdentifier is null ) {
            areaIdentifier = new AreaIdentifier( "North" );
         }

         return Sys.ActorOf( SensorAreaActor.Props( areaIdentifier ) );
      }

      private IActorRef AddSensorToArea( IActorRef area, SensorIdentifier sensorId ) {
         var probe = CreateTestProbe();
         area.Tell( new AddSensorRequest( new CorrelationId(), sensorId ), probe );
         IActorRef addedSensor = null;
         probe.ExpectMsg<AddSensorResponse>( response => addedSensor = response.TemperatureSensor );
         return addedSensor;
      }

   }
}