using System.ComponentModel;

namespace WordProcessor.Extensions
{
	public static class InvokingExtensions
	{
		public static void InvokeIfRequired(this Control control, MethodInvoker action)
		{
			if (control.InvokeRequired)
				control.Invoke(action);
			else
				action();
		}

		public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
		{
			if (obj.InvokeRequired)
			{
				var args = Array.Empty<object>();
				obj.Invoke(action, args);
			}
			else
				action();
		}

		public static T InvokeIfRequired<T>(this Control control, Func<T> action)
		{
			if (!control.InvokeRequired)
			{
				return action();
			}

			return (T)control.Invoke(action);
		}

		public static void InvokeIfRequired<T>(this T control, Action<T> action) where T : Control
		{
			control.InvokeIfRequired(() => { action(control); });
		}
	}
}
