using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using PipelineActors.Actors;
using PipelineActors.Messages;
using PipelineActors.Support;

namespace PipelineActors.Tests {
   [TestFixture]
   public class ExecuteSafetyEvaluationTests: TestKit {
      [Test]
      public void ShouldRequestAllSensorsWhenCreated() {
         var probe = CreateTestProbe();
         CreateEvaluate( probe );

         probe.ExpectMsg<GetAllSensorsRequest>();
      }

      [Test]
      public void ShouldRequestTemperaturesFromSensors() {
         var probe = CreateTestProbe();
         var evaluate = CreateEvaluate( probe );

         var sensor1 = CreateTestProbe( "sensor1" );
         var sensor2 = CreateTestProbe( "sensor2" );

         var sensors = new Dictionary<SensorIdentifier, IActorRef> {
            {new SensorIdentifier( 1 ), sensor1},
            {new SensorIdentifier( 2 ), sensor2}
         };

         evaluate.Tell( new GetAllSensorsResponse( new CorrelationId(), sensors.ToImmutableDictionary() ) );

         sensor1.ExpectMsg<QueryTemperatureRequest>();
         sensor2.ExpectMsg<QueryTemperatureRequest>();
      }

      [Test]
      public void AfterAllTemperaturesRecordedShouldRespondWithResult() {
         var temperatureSource = CreateTestProbe( "TemperatureSource" );
         var replyTo = CreateTestProbe( "ReplyTo" );
         var evaluate = CreateEvaluate( temperatureSource, replyTo );

         SensorIdentifier id1 = new SensorIdentifier( 1 );
         SensorIdentifier id2 = new SensorIdentifier( 2 );

         var sensor1 = CreateTestProbe( "sensor1" );
         var sensor2 = CreateTestProbe( "sensor2" );
         var sensors = new Dictionary<SensorIdentifier, IActorRef> {
            {id1, sensor1},
            {id2, sensor2}
         };

         evaluate.Tell( new GetAllSensorsResponse( new CorrelationId(), sensors.ToImmutableDictionary() ) );
         evaluate.Tell( new QueryTemperatureResponse( new CorrelationId(), id1, 100, DateTime.Now ) );
         evaluate.Tell( new QueryTemperatureResponse( new CorrelationId(), id2, 100, DateTime.Now ) );

         var received = replyTo.ExpectMsg<SafetyEvaluationResult>();
         received.RequiresNotification.Should().BeFalse();
         received.Message.Should().BeEmpty();
      }

      [Test]
      public void WhenTemperatureGreaterThan200ShouldRespondWithResult() {
         var temperatureSource = CreateTestProbe( "TemperatureSource" );
         var replyTo = CreateTestProbe( "ReplyTo" );
         var evaluate = CreateEvaluate( temperatureSource, replyTo );

         SensorIdentifier id1 = new SensorIdentifier( 1 );
         SensorIdentifier id2 = new SensorIdentifier( 2 );

         var sensor1 = CreateTestProbe( "sensor1" );
         var sensor2 = CreateTestProbe( "sensor2" );
         var sensors = new Dictionary<SensorIdentifier, IActorRef> {
            {id1, sensor1},
            {id2, sensor2}
         };

         evaluate.Tell( new GetAllSensorsResponse( new CorrelationId(), sensors.ToImmutableDictionary() ) );
         evaluate.Tell( new QueryTemperatureResponse( new CorrelationId(), id1, 250, DateTime.Now ) );
         evaluate.Tell( new QueryTemperatureResponse( new CorrelationId(), id2, 100, DateTime.Now ) );

         var received = replyTo.ExpectMsg<SafetyEvaluationResult>();
         received.RequiresNotification.Should().BeTrue();
         received.Message.Should().Be( "Temperature exceeds 200 degrees" );
      }

      private IActorRef CreateEvaluate( IActorRef temperatureSource, IActorRef replyTo = null ) {
         return Sys.ActorOf( ExecuteSafetyEvaluationActor.Props( temperatureSource, replyTo ) );
      }
   }
}