using System;

namespace HomaGames.HomaBelly
{
    public class HomaAnalyticsOptions
    {
        /// <summary>
        /// Print logs when events are sent and responses are received from the server.
        /// </summary>
        public bool VerboseLogs { get; private set; }
        
        /// <summary>
        /// We will do validations before sending the event. Disable it in release builds to improve performance.
        /// </summary>
        public bool EventValidation { get; private set; }
        
        /// <summary>
        /// Used for events requests to the server.
        /// </summary>
        public string EndPointUrl { get; private set; }
        
        /// <summary>
        /// A new session id will be generated if the time since last opened session is major than this value.
        /// </summary>
        public float SecondsToGenerateNewSessionId { get; private set; }
        
        /// <summary>
        /// Homa Belly manifest version
        /// </summary>
        public string ManifestVersionId { get; private set; }

        /// <summary>
        /// Application token to access to Homa Belly manifest
        /// </summary>
        public string TokenIdentifier { get; private set; }
        
        /// <summary>
        /// Set a method to retrieve NTesting ids.
        /// </summary>
        public GetNTestingIdsDelegate GetNTestingIds { get; private set; }
        
        /// <summary>
        /// Send Fps data.
        /// </summary>
        public bool SendFpsEvents { get; private set; }

        /// <summary>
        /// If true, we will save sent events in a CSV file. Useful to debug sent events.
        /// </summary>
        public bool RecordAllEventsInCsv { get; }
        
        /// <summary>
        /// Return true if NTesting is initialized, in that casa you can use N-Testing ID and N_Testing Override ID
        /// </summary>
        public delegate bool GetNTestingIdsDelegate(out string nTestingId,out string nTestingOverrideId);

        public HomaAnalyticsOptions(bool verboseLogs,
            bool eventValidation,
            string endPointUrl,
            float secondsToGenerateNewSessionId,
            string tokenIdentifier,
            string manifestVersionId,
            GetNTestingIdsDelegate getNTestingId,
            bool sendFpsEvents,
            bool recordAllEventsInCsv)
        {
            VerboseLogs = verboseLogs;
            EventValidation = eventValidation;
            EndPointUrl = endPointUrl;
            SecondsToGenerateNewSessionId = secondsToGenerateNewSessionId;
            TokenIdentifier = tokenIdentifier;
            ManifestVersionId = manifestVersionId;
            GetNTestingIds = getNTestingId;
            SendFpsEvents = sendFpsEvents;
            RecordAllEventsInCsv = recordAllEventsInCsv;
        }

        public override string ToString()
        {
            return $"{nameof(VerboseLogs)}: {VerboseLogs}, {nameof(EventValidation)}: {EventValidation}, {nameof(EndPointUrl)}: {EndPointUrl}, {nameof(SecondsToGenerateNewSessionId)}: {SecondsToGenerateNewSessionId}, {nameof(ManifestVersionId)}: {ManifestVersionId}, {nameof(TokenIdentifier)}: {TokenIdentifier}, Fps: {SendFpsEvents}";
        }
    }
}