namespace Sander.QuickList.Application
{
	internal sealed class Status
	{
		internal static double CurrentValue;
		internal bool IsTerminating { get; set; }
		internal string Text { get; set; }
		internal double Percentage { get; set; }


		internal static Status Get(string text, double percentage, bool isTerminating = false)
		{
			CurrentValue = percentage;
			return new Status
			{
				IsTerminating = isTerminating,
				Text = text,
				Percentage = percentage
			};
		}
	}
}
