namespace NewTask
{
	public class Manager
	{
		public static string GetMessage(string[] args)
		{
			var result = (args.Length > 0) ? 
				string.Join(" ", args) : "Hello World!";

			return result;
		}
	}
}
