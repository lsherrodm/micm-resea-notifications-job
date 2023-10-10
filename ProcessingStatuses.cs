public enum ProcessingStatus
{
    	Success,              // Message was successfully processed
		Failed,               // Failed to process the message, but can retry
		InvalidFormat,        // The message format was invalid or unexpected
		NotFound,             // A needed resource (like a database record) wasn't found
		Unprocessable,        // Failed to process the message, and it shouldn't be retried
		ExternalServiceError, // There was an error with an external service or dependency
							  // ... Add more states as needed
}