namespace CTeleport.DistanceMeter.Domain.Models
{
    using Geolocation;

    public class IataPoint
    {
        public string Iata { get; }
        public Location Location { get; }

        public IataPoint(string iata, Location location)
        {
            Iata = iata;
            Location = location;
        }

        public double DistanceTo(IataPoint destinationPoint)
        {
            var origin = new Coordinate(Location.Latitude, Location.Longitude);
            var destination = new Coordinate(destinationPoint.Location.Latitude, destinationPoint.Location.Longitude);

            return GeoCalculator.GetDistance(origin, destination);
        }
    }
}