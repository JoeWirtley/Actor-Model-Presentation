# Actor Model Presentation
This code supports the Actor Model and why you should be using it presentation.

This code is meant to illustrate how the actor model may be useful. It is not meant to illustrate production ready code, and is not even functionally complete.  These are the functional items that are incomplete:

* TemperatureSensorActor should be watching for subscribers to die and remove them from the list when that happens.
* SensorAreaActor currently subscribes to temperature updates from sensors, but does not see if that subscription is successful.
* SensorAreaActor does not watch for sensors that may die.
* SensorAreaActor has no way to remove sensors