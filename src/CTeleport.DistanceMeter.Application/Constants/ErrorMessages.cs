namespace CTeleport.DistanceMeter.Application.Constants
{
    public static class ErrorMessages
    {
        public const string ExternalApiErrorMessage = "Unable to calculate a distance.{0}";
        public const string ExternalApiIataNotFound = "There is no place assosiated with '{0}' IATA code.";
        public const string ExternalApiFailed = "External API returns an unsuccessful result.";
        public const string InputValidationErrorMessage = "{PropertyName} is invalid. Should be 3 uppercase letters.";
        public const string InputValidationSameIataErrorMessage =
            "It is pointless to measure the distance having a single object. Please, specify two different IATA codes.";
    }
}