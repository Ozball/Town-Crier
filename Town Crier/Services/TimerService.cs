using Microsoft.Extensions.Configuration;
using System;
using System.Timers;

namespace TownCrier.Services
{
	public class TimerService
	{
		Timer Clock { get; set; } = new Timer();

		/// <summary>
		/// This event is fired every 15 minutes after the bot goes online.
		/// </summary>
		/// <param name="sender">Current DateTime that the event was fired at.</param>
		/// <param name="e">Service provider, you can get all available singletons form here.</param>
		public event EventHandler<IServiceProvider> OnClockInterval;

		readonly IServiceProvider provider;
		readonly IConfiguration config;

		public TimerService(IServiceProvider provider, IConfiguration config)
		{
			this.provider = provider;
			this.config = config;

			Clock.Interval = int.Parse(this.config["timerInterval"]);
			Clock.AutoReset = true;
			Clock.Elapsed += TickTock;
			Clock.Start();
		}

		void TickTock(object sender, ElapsedEventArgs e)
		{
			OnClockInterval?.Invoke(DateTime.Now, provider);
		}
	}
}
