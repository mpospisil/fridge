using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;

namespace Fridge.Auth
{
	/// <summary>
	/// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
	/// actual Lambda function entry point. The Lambda handler field should be set to
	/// 
	/// Fridge.Auth::Fridge.Auth.LambdaEntryPoint::FunctionHandlerAsync
	/// </summary>
	public class LambdaEntryPoint :

			// The base class must be set to match the AWS service invoking the Lambda function. If not Amazon.Lambda.AspNetCoreServer
			// will fail to convert the incoming request correctly into a valid ASP.NET Core request.
			//
			// API Gateway REST API                         -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
			// API Gateway HTTP API payload version 1.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
			// API Gateway HTTP API payload version 2.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
			// Application Load Balancer                    -> Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
			// 
			// Note: When using the AWS::Serverless::Function resource with an event type of "HttpApi" then payload version 2.0
			// will be the default and you must make Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction the base class.

			Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
	{
		/// <summary>
		/// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
		/// needs to be configured in this method using the UseStartup<>() method.
		/// </summary>
		/// <param name="builder"></param>
		protected override void Init(IWebHostBuilder builder)
		{
			builder
					.UseStartup<Startup>();
		}

		/// <summary>
		/// Use this override to customize the services registered with the IHostBuilder. 
		/// 
		/// It is recommended not to call ConfigureWebHostDefaults to configure the IWebHostBuilder inside this method.
		/// Instead customize the IWebHostBuilder in the Init(IWebHostBuilder) overload.
		/// </summary>
		/// <param name="builder"></param>
		protected override void Init(IHostBuilder builder)
		{
		}

		public override Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
		{
			var retentionPolicy = LogGroupRetentionPolicy.OneDay;

			// customer formatter  
			//var formatter = new CustomLogFormatter();

			var options = new CloudWatchSinkOptions
			{
				// the name of the CloudWatch Log group from config  
				LogGroupName = "Fridge.Auth",
				// the main formatter of the log event  
				//TextFormatter = formatter,
				// other defaults  
				MinimumLogEventLevel = LogEventLevel.Verbose,
				BatchSizeLimit = 100,
				QueueSizeLimit = 10000,
				Period = TimeSpan.FromSeconds(10),
				CreateLogGroup = true,
				LogStreamNameProvider = new DefaultLogStreamProvider(),
				RetryAttempts = 5,
				LogGroupRetentionPolicy = retentionPolicy
			};
			// setup AWS CloudWatch client  
			var client = new AmazonCloudWatchLogsClient();

			Log.Logger = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
					.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
					.MinimumLevel.Override("System", LogEventLevel.Warning)
					.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
					.Enrich.FromLogContext()

					//.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
					.WriteTo.AmazonCloudWatch(options, client)
					.CreateLogger();


				Log.Information("Starting host...");
				return base.FunctionHandlerAsync(request, lambdaContext);			
		}
	}
}
