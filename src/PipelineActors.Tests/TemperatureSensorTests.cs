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
   public class TemperatureSensorTests: TestKit {
      [Test]
      public void ShouldQueryIdentification() {
         var probe = CreateTestProbe();

         SensorIdentifier expectedIdentifier = new SensorIdentifier( 23 );
         var sensor = CreateTestSensor( expectedIdentifier );

         CorrelationId expectedCorrelationId = new CorrelationId();

         sensor.Tell( new SensorIdentificationRequest( expectedCorrelationId ), probe.Ref );

         var received = probe.ExpectMsg<SensorIdentificationResponse>();

         received.CorrelationId.Should().Be( expectedCorrelationId );
         received.SensorIdentifier.Should().Be( expectedIdentifier );
      }

      [Test]
      public void ShouldUpdateTemperature() {
         var probe = CreateTestProbe();
         var sensor = CreateTestSensor();

         CorrelationId correlationId = new CorrelationId();
         double temperature = 98.4;
         DateTime readingTime = DateTime.Now;

         sensor.Tell( new UpdateTemperatureRequest( correlationId, temperature, readingTime ), probe.Ref );

         var received = probe.ExpectMsg<UpdateTemperatureResponse>();

         received.CorrelationId.Should().Be( correlationId );
      }

      [Test]
      public void ShouldQueryTemperature() {
         var probe = CreateTestProbe();

         SensorIdentifier expectedIdentifier = new SensorIdentifier( 23 );
         var sensor = CreateTestSensor( expectedIdentifier );

         CorrelationId expectedCorrelationId = new CorrelationId();
         double expectedTemperature = 98.4;
         DateTime expectedReadingTime = DateTime.Now;

         sensor.Tell( new UpdateTemperatureRequest( new CorrelationId(), expectedTemperature, expectedReadingTime ) );
         sensor.Tell( new QueryTemperatureRequest( expectedCorrelationId ), probe.Ref );

         var received = probe.ExpectMsg<QueryTemperatureResponse>();

         received.CorrelationId.Should().Be( expectedCorrelationId );
         received.SensorId.Should().Be( expectedIdentifier );
         received.Temperature.Should().Be( 98.4 );
         received.ReadingTime.Should().Be( expectedReadingTime );
      }

      [Test]
      public void ShouldSubscribeToChanges() {
         var probe = CreateTestProbe();

         SensorIdentifier expectedIdentifier = new SensorIdentifier( 23 );
         var sensor = CreateTestSensor( expectedIdentifier );

         CorrelationId expectedCorrelationId = new CorrelationId();
         sensor.Tell( new SubscribeToUpdatesRequest( expectedCorrelationId ), probe.Ref );

         var received = probe.ExpectMsg<SubscribeToUpdatesResponse>();

         received.CorrelationId.Should().Be( expectedCorrelationId );
      }

      [Test]
      public void AfterSubscriptionShouldReceiveUpdates() {
         var probe = CreateTestProbe();

         SensorIdentifier expectedIdentifier = new SensorIdentifier( 1234 );
         var sensor = CreateTestSensor( expectedIdentifier );

         double expectedTemperature = 100.2;
         DateTime expectedReadingTime = DateTime.Now;

         sensor.Tell( new SubscribeToUpdatesRequest( new CorrelationId() ), probe );
         probe.ExpectMsg<SubscribeToUpdatesResponse>();

         sensor.Tell( new UpdateTemperatureRequest( new CorrelationId(), expectedTemperature, expectedReadingTime ) );

         var received = probe.ExpectMsg<TemperatureUpdated>();
         received.SensorId.Should().Be( expectedIdentifier );
         received.Temperature.Should().Be( expectedTemperature );
         received.ReadingTime.Should().Be( expectedReadingTime );
      }


      private IActorRef CreateTestSensor( SensorIdentifier sensorIdentifier = null ) {
         if ( sensorIdentifier is null ) {
            sensorIdentifier = new SensorIdentifier( 1 );
         }

         return Sys.ActorOf( TemperatureSensorActor.Props( sensorIdentifier ) );
      }
   }
}