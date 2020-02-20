## CheapAwesome.Api

I designed an architecture in which more hotel price supplier can be easily implemented. 
Main service (HotelAvailabilityService) is collecting data from all registered price providers (suppliers) in parallel and returns them combining.

### Caching

I used memory caching, however a distributed cache provider like Redis would be better to use, if this api is going to be deployed to more than one instances.

