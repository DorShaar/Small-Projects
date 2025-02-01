namespace AspNetApp.Middlewares;

public class InvalidRouteMiddleware
{
	private readonly RequestDelegate mNextRequest;
	// private readonly ILogger<InvalidRouteMiddleware> mLogger;

	public InvalidRouteMiddleware(RequestDelegate iNextRequest
								  // ILogger<InvalidRouteMiddleware> logger
		)
	{
		mNextRequest = iNextRequest;
		// mLogger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// Proceed with the next middleware in the pipeline
		await mNextRequest(context);

		// If the response status code is 404, log it
		if (context.Response.StatusCode == 404)
		{
			// Log the incorrect endpoint (you can log more details here if needed)
			
			// mLogger.LogWarning("404 Not Found: {RequestMethod} {RequestPath} was not found.", 
			// 				   context.Request.Method, context.Request.Path);
		}
	}
}
