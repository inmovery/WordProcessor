namespace WordProcessor.Extensions
{
	public static class RichTextBoxExtensions
	{
		public static void UpdateText(this RichTextBox richTextBox, string find, string replace, int from, int to)
		{
			var selectionStart = richTextBox.Find(find.ToCharArray(), from, to);
			if (selectionStart.Equals(-1))
				return; // ToDo: may be return custom exception

			richTextBox.SelectionStart = selectionStart;
			richTextBox.SelectionLength = find.Length;
			richTextBox.SelectedText = replace;
		}

		private static object? ExecuteRichTextBoxRequest<TResponseType>(this RichTextBox richTextBox, Action action)
		{
			return default;
		}
	}
}
