# Actor Model Presentation
This code supports the *Actor Model and Why you should be using it* presentation.  The scenario is a pipeline that has many sensors for temperature, pressure, flow, etc.  This sample handles temperature sensors.  There is a TemperatureSensorActor class with one actor corresponding to each temperature sensor.  In this sample, these are collected into areas (SensorAreaActor).  For each area, there is a SafetyEvaluatorActor whose job is to listen for temperature updates from sensors in that area and send a notification if there is a safety issue.  For the purpose of this sample, a SafetyNotification message is sent if any temperature in an area is above 200 degrees.

Rather than perform the safety evaluation itself, the SafetyEvaluationActor creates another actor do do the evaluation, specifically ExecuteSafetyEvaluationActor.  This created actor exists only to perform the calculation and stops when the calculation is complete.

Running the console application will create an area with a safety evaluator, place five sensors in that area, and then begin generating random temperature values every second.  When any generated temperature is above 200 degrees, the notification will be displayed in the console output.

## Ready for Production Notes
This code is meant to illustrate how the actor model may be useful. It is not production ready code, and is not even functionally complete.  These are the functional items that are incomplete:

### TemperatureSensorActor
* Should be watching for subscribers to die and remove them from the list when that happens.
* Should support an unsubscribe message
### SensorAreaActor 
* Currently subscribes to temperature updates from sensors, but does not see if that subscription is successful.
* Does not watch for sensors that may die.
* Has no way to remove sensors from the area
### SafetyEvaluatorActor
* Needs a way to unsubscribe from notification requests
* Needs to watch subscribers to remove them if they die
### EvaluateSafetyActor
* Needs a timeout in case it does not get temperatures, or times out during the calculation.

## Resources
This sample takes inspiration from the Pluralsight course from [Jason Roberts](https://www.pluralsight.com/authors/jason-roberts): [Representing IoT Systems with the Actor Model and Akka.NET](https://www.pluralsight.com/courses/actor-model-akka-dotnet).  I highly recommend this course and all of Jason's Akka.NET Pluralsight courses.

The main site for Akka.NET is [here](https://getakka.net/).
