using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using WordProcessor.Controls.ListViewEx;
using WordProcessor.Data.Contracts;
using WordProcessor.Data.Entities;
using WordProcessor.Data.Enum;
using WordProcessor.Extensions;

namespace WordProcessor
{
	[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
	public partial class MainForm : Form
	{
		private readonly IWordRepository _wordRepository;

		private readonly string[] _separatorList = { " ", "\v", "\n", "\r", "\r\n" };
		private readonly IList<char> _punctuationSigns = new List<char>() { '.', ',', ':', ';', '!', '?', '@', '(', ')', '-', '–', '—', '=', '_', '*', '/', '\\', '|', '«', '»' };

		private const char WhitespaceSymbol = ' ';
		private const int ListBoxVerticalOffset = 20;
		private const int ListBoxItemHorizontalOffset = 10;
		private const int ListBoxItemVerticalOffset = 5;
		private const int HintWordsQuantity = 5;

		private string _wordInProcess;

		private bool _hintItemApplied;
		private bool _hintItemSelectedByEnter;
		private bool _isLastInputWhitespace;
		private bool _isLastInputBackspace;

		private KeyboardAction _lastKeyboardAction;

		private int CursorPosition => RichTextBox.SelectionStart;

		private readonly ListBoxEx _hintList;

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		private static extern int SHGetKnownFolderPath(ref Guid id, int flags, IntPtr token, out IntPtr path);

		private static Guid _folderDownloads = new("374DE290-123F-4565-9164-39C4925E467B");

		public MainForm(IWordRepository wordRepository)
		{
			_wordRepository = wordRepository;
			_wordInProcess = string.Empty;
			_hintList = new ListBoxEx();

			// To compensate slow first query
			var _ = _wordRepository.GetCloseWords(_wordInProcess, HintWordsQuantity).FirstOrDefault();
			
			_wordRepository.RemoveAll();
			_wordRepository.CommitChangesAsync();

			InitializeComponent();

			// To compensate slow display first symbol together with loading data from database inside main thread
			RichTextBox.Controls.Add(_hintList);
			_hintList.Hide();
		}

		/// <summary>
		/// Perform text process
		/// </summary>
		private void ProcessText()
		{
			_wordInProcess = GetWordInProcess();

			var isSuitableWordLength = _wordInProcess.Length >= 3;
			if (!isSuitableWordLength)
			{
				_hintList.Hide();
				return;
			}

			// Check whole word for belonging to alphabetic nature/essence
			var isLetterWord = GetSummaryOfWordValidation(_wordInProcess);
			if (!isLetterWord)
			{
				_hintList.Hide();
				return;
			}

			var closeWords = _wordRepository.GetCloseWords(_wordInProcess, HintWordsQuantity).ToList();
			var isWordHintsExists = closeWords.Any();
			if (!isWordHintsExists)
			{
				_hintList.Hide();
				return;
			}

			ShowWordHints(closeWords);
		}

		/// <summary>
		/// Get word in process
		/// </summary>
		/// <returns></returns>
		private string GetWordInProcess()
		{
			var textBeforeCursorPosition = RichTextBox.Text[..CursorPosition];

			var separators = _punctuationSigns.Select(symbol => symbol.ToString()).ToList();
			separators.AddRange(_separatorList);

			var words = textBeforeCursorPosition.Split(separators.ToArray(), StringSplitOptions.None);

			return words.Last();
		}

		/// <summary>
		/// Replace specified word with own location in work area (RichTextBox)
		/// </summary>
		/// <param name="wordToReplace"></param>
		private void ReplaceWordInProcess(string wordToReplace)
		{
			if (string.IsNullOrEmpty(_wordInProcess))
				_wordInProcess = GetWordInProcess();

			_hintItemApplied = true;
			_hintList.Items.Clear();

			RichTextBox.UpdateText(
				find: _wordInProcess,
				replace: wordToReplace,
				from: CursorPosition - _wordInProcess.Length,
				to: CursorPosition);
		}

		/// <summary>
		/// Get count of whitespaces that preceding cursor position
		/// </summary>
		/// <returns></returns>
		private int GetWhitespaceCountPrecedingCursorPosition()
		{
			var whitespaceCount = 0;
			var cursorPositionIndex = CursorPosition - 1;
			var precedingText = RichTextBox.Text[..cursorPositionIndex];
			foreach (var symbol in precedingText.Reverse())
			{
				if (symbol.Equals(' '))
					whitespaceCount++;
				else
					break;
			}

			return whitespaceCount;
		}

		/// <summary>
		/// Validate symbol after cursor position on whitespace or existence
		/// </summary>
		/// <param name="afterCursorPosition"></param>
		/// <returns></returns>
		private bool CheckWhetherSymbolIsEmptyOrLast(char afterCursorPosition)
		{
			var isAfterWhitespaceOrEmpty = afterCursorPosition.Equals(WhitespaceSymbol) || afterCursorPosition.Equals('\0');
			var whetherCursorPositionIsLast = CheckWhetherCursorPositionIsLast();

			return isAfterWhitespaceOrEmpty || whetherCursorPositionIsLast;
		}

		/// <summary>
		/// Check whether symbol related to punctuation
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		private bool CheckWhetherSymbolIsPunctuation(char symbol)
		{
			return _punctuationSigns.Contains(symbol);
		}

		/// <summary>
		/// Check whether cursor position is last position
		/// </summary>
		/// <returns></returns>
		private bool CheckWhetherCursorPositionIsLast()
		{
			var textExists = RichTextBox.Text.Any();
			if (!textExists)
				return true;

			var lastSymbol = RichTextBox.Text.Last();
			return RichTextBox.Text[CursorPosition - 1].Equals(lastSymbol);
		}

		/// <summary>
		/// Get summary of specified symbol validation
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		private bool GetSummaryOfSymbolValidation(char symbol)
		{
			// ToDo: to imagine some approach to validation single symbol
			var isLatin = true;
			var isCyrillic = true;

			return isLatin && isCyrillic;
		}

		/// <summary>
		/// Get summary of whole word symbols validation
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		private bool GetSummaryOfWordValidation(string word)
		{
			return word.Aggregate(true, (current, symbol) => current & GetSummaryOfSymbolValidation(symbol));
		}

		/// <summary>
		/// Get size of specified word
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		private SizeF GetWordSize(string word)
		{
			var graphics = Graphics.FromHwnd(RichTextBox.Handle);
			var size = graphics.MeasureString(word, RichTextBox.Font);

			return size;
		}

		/// <summary>
		/// Show list with word hints
		/// </summary>
		/// <param name="words"></param>
		private void ShowWordHints(IList<Word> words)
		{
			var isWordListEmpty = !words.Any();
			if (isWordListEmpty)
				return;

			var hintListPosition = RichTextBox.GetPositionFromCharIndex(CursorPosition - _wordInProcess.Length);
			hintListPosition.Y += ListBoxVerticalOffset;

			var longestWord = words.MaxBy(word => word.Content.Length)?.Content ?? string.Empty;
			var longestWordSize = GetWordSize(longestWord);

			var listItemWidth = (int)(longestWordSize.Width);
			var listItemHeight = (int)(longestWordSize.Height);

			var listWidth = listItemWidth + ListBoxItemHorizontalOffset;
			var listHeight = listItemHeight * words.Count() + ListBoxItemVerticalOffset;

			_hintList.Font = RichTextBox.Font;
			_hintList.ItemHeight = listItemHeight;
			_hintList.Location = hintListPosition;
			_hintList.WordInFilter = _wordInProcess;

			_hintList.Size = new Size(listWidth, listHeight);
			_hintList.SelectedValueChanged += ListOnSelectedValueChanged;

			_hintList.Items.Clear();
			foreach (var word in words)
				_hintList.Items.Add(word.Content);

			var isHintListInsideRichTextBox = RichTextBox.Controls.Contains(_hintList);
			if (isHintListInsideRichTextBox)
				_hintList.Show();
			else
				RichTextBox.Controls.Add(_hintList);
		}

		/// <summary>
		/// Handle selected value event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ListOnSelectedValueChanged(object? sender, EventArgs eventArgs)
		{
			var needPerformKeyboardAction = _lastKeyboardAction is KeyboardAction.Enter or KeyboardAction.None;
			if (!needPerformKeyboardAction)
				return;

			var listBoxEx = sender as ListBoxEx;
			var selectedWord = listBoxEx?.SelectedItem?.ToString();
			if (selectedWord is null)
				return;

			var formattedInsertion = $"{selectedWord} ";
			ReplaceWordInProcess(formattedInsertion);
		}

		/// <summary>
		/// Handle TextChanged event for work area (RichTextBox)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RichTextBox_TextChanged(object sender, EventArgs eventArgs)
		{
			// Where was selected hint item
			if (_hintItemApplied)
			{
				_hintItemApplied = false;
				_hintList.Hide();
				return;
			}

			// Where was entered Whitespace key or applied selected item from hint list
			if (_isLastInputWhitespace)
			{
				_hintList.Hide();
				return;
			}

			var text = RichTextBox.Text;
			if (string.IsNullOrEmpty(text) || CursorPosition == 0)
			{
				_hintList.Hide();
				return;
			}

			var symbolBeforeCursorPosition = text[CursorPosition - 1];
			var symbolAfterCursorPosition = '\0';

			var isLastOrSingleSymbol = text.Length == CursorPosition;
			if (!isLastOrSingleSymbol)
				symbolAfterCursorPosition = text[CursorPosition];

			var whetherSymbolBeforeCursorPositionIsEndOfLine = _separatorList[1..].Contains(symbolBeforeCursorPosition.ToString());

			// Where was entered Backspace key
			if (_isLastInputBackspace)
			{
				// Validate symbol before selection center
				var isBeforeWhitespace = symbolBeforeCursorPosition.Equals(WhitespaceSymbol);
				if (isBeforeWhitespace)
				{
					_hintList.Hide();
					return;
				}

				// Validate symbol after cursor position
				var isAfterPunctuation = CheckWhetherSymbolIsPunctuation(symbolAfterCursorPosition);
				var whetherSymbolIsEmptyOrLast = CheckWhetherSymbolIsEmptyOrLast(symbolAfterCursorPosition);
				var needProcessText = (whetherSymbolIsEmptyOrLast || isAfterPunctuation) && !whetherSymbolBeforeCursorPositionIsEndOfLine;
				if (needProcessText)
					ProcessText();
			}
			else
			{
				var isDotOrCommaAsLastEntered = symbolBeforeCursorPosition.Equals('.') || symbolBeforeCursorPosition.Equals(',');

				var precedingWhitespaceCount = GetWhitespaceCountPrecedingCursorPosition();
				var isSinglePrecedingWhitespace = precedingWhitespaceCount == 1;

				if (isDotOrCommaAsLastEntered && isSinglePrecedingWhitespace)
					RichTextBox.UpdateText($" {symbolBeforeCursorPosition}", symbolBeforeCursorPosition.ToString(), CursorPosition - 2, CursorPosition);

				var needProcessText = CheckWhetherSymbolIsEmptyOrLast(symbolAfterCursorPosition) && !whetherSymbolBeforeCursorPositionIsEndOfLine;
				if (needProcessText)
					ProcessText();
				else
					_hintList.Hide();
			}
		}

		/// <summary>
		/// Handle PreviewKeyDown event for work area (RichTextBox)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RichTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs eventArgs)
		{
			_isLastInputWhitespace = eventArgs.KeyCode.Equals(Keys.Space);
			_isLastInputBackspace = eventArgs.KeyCode.Equals(Keys.Back);

			_lastKeyboardAction = eventArgs.KeyCode switch
			{
				Keys.Down => KeyboardAction.Down,
				Keys.Up => KeyboardAction.Up,
				Keys.Left => KeyboardAction.Left,
				Keys.Right => KeyboardAction.Right,
				Keys.Enter => KeyboardAction.Enter,
				_ => KeyboardAction.None
			};

			int newPointerPosition;
			switch (_lastKeyboardAction)
			{
				case KeyboardAction.Up:
					newPointerPosition = (_hintList.SelectedIndex + _hintList.Items.Count - 1) % _hintList.Items.Count;
					_hintList.SelectedItem = _hintList.SelectedIndex < 0
						? _hintList.Items[^1]
						: _hintList.Items[newPointerPosition];
					break;

				case KeyboardAction.Down:
					newPointerPosition = (_hintList.SelectedIndex + 1) % _hintList.Items.Count;
					_hintList.SelectedItem = _hintList.Items[newPointerPosition];
					break;

				case KeyboardAction.Enter:
					var selectedIndex = _hintList.SelectedIndex;
					if (selectedIndex.Equals(-1))
						break;

					var selectedItem = _hintList.SelectedItem?.ToString();
					if (string.IsNullOrEmpty(selectedItem))
						break;

					_hintItemSelectedByEnter = true;
					ReplaceWordInProcess(selectedItem);
					break;

				default:
				case KeyboardAction.None:
				case KeyboardAction.Left:
				case KeyboardAction.Right:
					break;
			}
		}

		/// <summary>
		/// Handle key down event for work area (RichTextBox)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RichTextBox_KeyDown(object sender, KeyEventArgs eventArgs)
		{
			eventArgs.SuppressKeyPress = _lastKeyboardAction is KeyboardAction.Enter;

			if (_hintItemSelectedByEnter)
			{
				_hintItemSelectedByEnter = false;
				return;
			}

			eventArgs.SuppressKeyPress = _lastKeyboardAction is KeyboardAction.Down or KeyboardAction.Up;
		}

		/// <summary>
		/// Handle click event on work area
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RichTextBox_Click(object sender, EventArgs eventArgs)
		{
			_hintList.Hide();
		}

		/// <summary>
		/// Handle Leave cursor event for work area (RichTextBox)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RichTextBox_Leave(object sender, EventArgs eventArgs)
		{
			_hintList.Hide();
		}

		/// <summary>
		/// Handle selection changed event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RichTextBox_SelectionChanged(object sender, EventArgs eventArgs)
		{
			// Where was entered left or right arrow
			var isLastLeftOrRightKeyboardAction = _lastKeyboardAction is KeyboardAction.Left or KeyboardAction.Right;
			if (isLastLeftOrRightKeyboardAction)
				_hintList.Hide();
		}

		/// <summary>
		/// Get download path in file system
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotSupportedException"></exception>
		public static string? GetDownloadsPath()
		{
			if (Environment.OSVersion.Version.Major < 6)
				throw new NotSupportedException();

			var pathPtr = IntPtr.Zero;
			try
			{
				SHGetKnownFolderPath(ref _folderDownloads, 0, IntPtr.Zero, out pathPtr);
				return Marshal.PtrToStringUni(pathPtr);
			}
			finally
			{
				Marshal.FreeCoTaskMem(pathPtr);
			}
		}

		/// <summary>
		/// Get filename from <see cref="OpenFileDialog" />
		/// </summary>
		/// <returns></returns>
		private string GetFilename()
		{
			var dialog = new OpenFileDialog();
			dialog.Title = @"Open Text File";
			dialog.Filter = @"TXT files (*.txt) | *.txt";
			dialog.DefaultExt = ".txt";
			dialog.InitialDirectory = GetDownloadsPath();

			var dialogResult = dialog.ShowDialog();

			return dialogResult switch
			{
				DialogResult.OK => dialog.FileName,
				DialogResult.Cancel => string.Empty,
				_ => string.Empty
			};
		}

		/// <summary>
		/// Create dummy word
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		private Word CreateDummyWord(string content) => new() { Content = content, };

		/// <summary>
		/// Configure words with considering already configured words
		/// </summary>
		/// <param name="items"></param>
		/// <param name="configuredWords"></param>
		/// <returns></returns>
		private IEnumerable<Word> ConfigureWords(IEnumerable<string> items, IEnumerable<Word>? configuredWords = null)
		{
			var words = new List<Word>();

			if (configuredWords is not null)
				words.AddRange(configuredWords);

			words.AddRange(items.Select(CreateDummyWord));

			var uniqueWords = words.DistinctBy(word => word.Content);
			foreach (var word in uniqueWords)
			{
				var count = items.Count(item => item == word.Content);

				var isConfiguredWord = word.Count > 0;
				if (isConfiguredWord)
					count += word.Count;

				word.Count = count;
				word.Frequency = (double)count / items.Count() * 1000;
			}

			var suitableWords = uniqueWords.Where(word => word.Count >= 3);

			return suitableWords;
		}

		/// <summary>
		/// Parse words from txt file
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		private async Task<IEnumerable<string>> ParseWordsFromFile(string filename)
		{
			var text = await File.ReadAllTextAsync(filename, Encoding.UTF8);

			// Decode html symbols
			text = HttpUtility.HtmlDecode(text);
			text = Regex.Replace(text, @"(\s*)?&nbsp;(\s*)?", string.Empty);

			var separators = _punctuationSigns.Select(symbol => symbol.ToString()).ToList();
			separators.AddRange(_separatorList);

			var digitalSymbols = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
			separators.AddRange(digitalSymbols);

			var items = text.ToLower().Split(separators.ToArray(), StringSplitOptions.RemoveEmptyEntries);

			return items;
		}

		/// <summary>
		/// Handle create word dictionary command
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private async void CreateDictionaryCommand_Click(object sender, EventArgs eventArgs)
		{
			var filename = GetFilename();
			if (string.IsNullOrEmpty(filename))
				return;

			var words = await ParseWordsFromFile(filename);
			var validationSummary = words.Aggregate(true, (current, word) => current & !string.IsNullOrEmpty(word));
			if (!validationSummary)
				return;

			_wordRepository.RemoveAll();

			var configuredWords = ConfigureWords(words);

			await _wordRepository.AddRangeAsync(configuredWords);
			await _wordRepository.CommitChangesAsync();

			MessageBox.Show(@"Words dictionary is loaded.");
		}

		/// <summary>
		/// Handle update word dictionary command
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private async void UpdateDictionaryCommand_Click(object sender, EventArgs eventArgs)
		{
			var currentWordsContent = _wordRepository.GetAll();

			var filename = GetFilename();
			var wordsFromFile = await ParseWordsFromFile(filename);

			var correctedWords = ConfigureWords(wordsFromFile, currentWordsContent);

			_wordRepository.UpdateRange(correctedWords);
			await _wordRepository.CommitChangesAsync();

			MessageBox.Show(@"Words dictionary is updated.");
		}

		/// <summary>
		/// Handle clear word dictionary command
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private async void ClearDictionaryCommand_Click(object sender, EventArgs eventArgs)
		{
			_wordRepository.RemoveAll();
			await _wordRepository.CommitChangesAsync();

			MessageBox.Show(@"Words dictionary was cleared.");
		}
	}
}