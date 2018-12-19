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
   public class SafetyEvaluatorTests: TestKit {
      [Test]
      public void ShouldSubscribeToNotifications() {
         var area = CreateTestArea();
         var evaluator = CreateSafetyEvaluator( area );
         var probe = CreateTestProbe();

         CorrelationId expectedCorrelationId = new CorrelationId();
         evaluator.Tell( new SubscribeToNotificationsRequest( expectedCorrelationId ), probe );
         var received = probe.ExpectMsg<SubscribeToNotificationsResponse>();

         received.CorrelationId.Should().Be( expectedCorrelationId );
      }

      [Test]
      public void TemperatureUpdateAbove200ShouldTriggerNotification() {
         var area = CreateTestArea();
         var sensor = AddSensorToArea( area, 1 );
       
         var evaluator = CreateSafetyEvaluator( area );
         var probe = CreateTestProbe();

         evaluator.Tell( new SubscribeToNotificationsRequest( new CorrelationId() ), probe );
         probe.ExpectMsg<SubscribeToNotificationsResponse>();

         double expectedTemperature = 234;
         DateTime expectedUpdated = DateTime.Now;

         sensor.Tell( new UpdateTemperatureRequest( new CorrelationId(), expectedTemperature, expectedUpdated )  );

         var received = probe.ExpectMsg<SafetyNotification>();
         received.Message.Should().Be( "Temperature exceeds 200 degrees" );
      }


      private IActorRef AddSensorToArea( IActorRef area, SensorIdentifier sensorId ) {
         var probe = CreateTestProbe();
         area.Tell( new AddSensorRequest( new CorrelationId(), sensorId ), probe );
         IActorRef addedSensor = null;
         probe.ExpectMsg<AddSensorResponse>( response => addedSensor = response.TemperatureSensor );
         return addedSensor;
      }

      private IActorRef CreateSafetyEvaluator( IActorRef areaActor ) {
         return Sys.ActorOf( SafetyEvaluatorActor.Props( areaActor ) );
      }

      private IActorRef CreateTestArea( AreaIdentifier areaIdentifier = null ) {
         if ( areaIdentifier is null ) {
            areaIdentifier = new AreaIdentifier( "North" );
         }

         return Sys.ActorOf( SensorAreaActor.Props( areaIdentifier ) );
      }

   }
}