using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms.VisualStyles;
using WordProcessor.Data.Entities;

namespace WordProcessor.Controls.ListViewEx
{
	public class ListBoxEx : ListBox
	{
		private const int PrfClient = 0x4;
		private const int WmPrintClient = 0x318;

		private const int ItemPadding = 3;

		private int _mouseIndex = -1;

		private readonly StringFormat _stringFormat = new()
		{
			Alignment = StringAlignment.Near,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.EllipsisCharacter
		};

		[DefaultValue(DrawMode.Normal)]
		[RefreshProperties(RefreshProperties.Repaint)]
		public new DrawMode DrawMode
		{
			get => base.DrawMode;
			set => base.DrawMode = value;
		}

		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(13)]
		public new int ItemHeight
		{
			get => base.ItemHeight;
			set => base.ItemHeight = value;
		}

		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue("")]
		public string WordInFilter { get; set; }

		private void InvalidateItem(int index)
		{
			if (index > -1 && index < Items.Count)
				Invalidate(GetItemRectangle(index));
		}

		protected virtual void DrawItemHighlighted(DrawItemEventArgs eventArgs, bool selected)
		{
			if (VisualStyleRenderer.IsSupported)
			{
				var elementToDraw = selected
					? VisualStyleElement.CreateElement("Explorer::ListView", 1, (int)ListBoxState.HotSelected)
					: VisualStyleElement.CreateElement("Explorer::ListView", 1, (int)ListBoxState.Hot);

				if (VisualStyleRenderer.IsElementDefined(elementToDraw))
				{
					new VisualStyleRenderer(elementToDraw).DrawBackground(eventArgs.Graphics, eventArgs.Bounds);
					return;
				}
			}

			using var backPen = new Pen(Color.FromArgb(25, SystemColors.Highlight));
			eventArgs.Graphics.FillRectangle(
				backPen.Brush,
				0,
				eventArgs.Bounds.Top,
				eventArgs.Bounds.Width - 0,
				eventArgs.Bounds.Height - 0);

			using var highlightPen = new Pen(Color.FromArgb(100, backPen.Color));
			eventArgs.Graphics.DrawRectangle(
				highlightPen,
				0,
				eventArgs.Bounds.Top,
				eventArgs.Bounds.Width - 1,
				eventArgs.Bounds.Height - 1);
		}

		protected virtual void DrawItemSelected(DrawItemEventArgs eventArgs, bool hover)
		{
			if (VisualStyleRenderer.IsSupported)
			{
				var elementToDraw = hover
					? VisualStyleElement.CreateElement("Explorer::ListView", 1, (int)ListBoxState.HotSelected)
					: VisualStyleElement.CreateElement("Explorer::ListView", 1, (int)ListBoxState.Selected);

				if (VisualStyleRenderer.IsElementDefined(elementToDraw))
				{
					new VisualStyleRenderer(elementToDraw).DrawBackground(eventArgs.Graphics, eventArgs.Bounds);
					return;
				}
			}

			using var backPen = new Pen(Color.FromArgb(50, SystemColors.Highlight));
			eventArgs.Graphics.FillRectangle(
				backPen.Brush
				, 0
				, eventArgs.Bounds.Top
				, eventArgs.Bounds.Width - 0
				, eventArgs.Bounds.Height - 0);

			using var highlightPen = new Pen(Color.FromArgb(200, backPen.Color));
			eventArgs.Graphics.DrawRectangle(
				highlightPen,
				0,
				eventArgs.Bounds.Top,
				eventArgs.Bounds.Width - 1,
				eventArgs.Bounds.Height - 1);
		}

		protected virtual void DrawItemText(DrawItemEventArgs eventArgs)
		{
			if (eventArgs.Index < 0)
				return;

			eventArgs.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			eventArgs.Graphics.DrawString(
				Items[eventArgs.Index].ToString(),
				Font,
				SystemBrushes.ControlText,
				new Rectangle(2, eventArgs.Bounds.Top + 2, eventArgs.Bounds.Width - 4, eventArgs.Bounds.Height - 4),
				_stringFormat);
		}

		protected virtual void DrawSeparator(DrawItemEventArgs eventArgs)
		{
			var hRect = new Rectangle(
				0,
				eventArgs.Bounds.Top + (eventArgs.Bounds.Height >> 1),
				Width,
				1);

			using Brush hBrush = new LinearGradientBrush(hRect, SystemColors.ControlDark, BackColor, LinearGradientMode.Horizontal);
			eventArgs.Graphics.FillRectangle(hBrush, hRect);
		}

		protected override void OnDrawItem(DrawItemEventArgs eventArgs)
		{
			if (Items.Count == 0 || eventArgs.Index > Items.Count)
				return;

			eventArgs.Graphics.FillRectangle(SystemBrushes.Window, eventArgs.Bounds);

			var itemSize = eventArgs.Graphics.MeasureString(WordInFilter, Font);
			var highlightedRectangle = eventArgs.Bounds with
			{
				Width = (int)itemSize.Width - ItemPadding,
				Height = (int)itemSize.Height - ItemPadding
			};

			highlightedRectangle.Y += ItemPadding;
			highlightedRectangle.X += ItemPadding;

			eventArgs.Graphics.FillRectangle(Brushes.Yellow, highlightedRectangle);

			var isSelected = (eventArgs.State & DrawItemState.Selected) == DrawItemState.Selected;
			var isHover = _mouseIndex != -1 && eventArgs.Index == _mouseIndex;

			if (isSelected)
			{
				DrawItemSelected(eventArgs, isHover);

				if (Focused && ShowFocusCues)
				{
					ControlPaint.DrawFocusRectangle(eventArgs.Graphics, new Rectangle(
						eventArgs.Bounds.X + 1,
						eventArgs.Bounds.Y + 1,
						eventArgs.Bounds.Width - 2,
						eventArgs.Bounds.Height - 2));
				}
			}

			if (isHover)
			{
				DrawItemHighlighted(eventArgs, isSelected);
				Cursor = Cursors.Hand;
			}

			DrawItemText(eventArgs);
		}

		protected override void OnMouseMove(MouseEventArgs eventArgs)
		{
			base.OnMouseMove(eventArgs);

			var index = IndexFromPoint(eventArgs.Location);
			if (index >= 0 && eventArgs.Y > GetItemRectangle(index).Bottom)
				index = -1;

			if (Equals(_mouseIndex, index))
				return;

			InvalidateItem(_mouseIndex);
			_mouseIndex = index;
			InvalidateItem(_mouseIndex);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			InvalidateItem(_mouseIndex);
			_mouseIndex = -1;
		}

		protected override void OnPaint(PaintEventArgs eventArgs)
		{
			if (GetStyle(ControlStyles.UserPaint))
			{
				var message = new Message
				{
					HWnd = Handle,
					Msg = WmPrintClient,
					WParam = eventArgs.Graphics.GetHdc(),
					LParam = (IntPtr)PrfClient
				};

				DefWndProc(ref message);
				eventArgs.Graphics.ReleaseHdc(message.WParam);
			}

			base.OnPaint(eventArgs);
		}

		public ListBoxEx()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.EnableNotifyMessage, true);

			DrawMode = DrawMode.OwnerDrawVariable;
			WordInFilter = string.Empty;
			Size = new Size(0, 0);
		}
	}
}
