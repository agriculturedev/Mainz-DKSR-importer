using FrostApi.Endpoints;
using FrostApi.Models.Sensor;

namespace FrostApi;

public class FrostApi
{
    public ThingEndpoints Things { get; set;  }
    public DataStreamEndpoints DataStreams { get; set;  }
    public ObservationEndpoints Observations { get; set;  }
    public ObservedPropertyEndpoints ObservedProperties { get; set;  }
    public SensorEndpoints Sensors { get; set;  }
    public FeatureOfInteresEndpoints FeatureOfInterest { get; set;  }
    
    public FrostApi(string baseUrl)
    {
        var endpoints = new Constants.Endpoints(baseUrl);
        Things = new ThingEndpoints(endpoints);
        DataStreams = new DataStreamEndpoints(endpoints);
        Observations = new ObservationEndpoints(endpoints);
        ObservedProperties = new ObservedPropertyEndpoints(endpoints);
        Sensors = new SensorEndpoints(endpoints);
        FeatureOfInterest = new FeatureOfInteresEndpoints(endpoints);
    }
    
}