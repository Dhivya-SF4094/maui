using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.Platform
{
	public class MauiSearchBar : UISearchBar, IUIViewLifeCycleEvents
	{
		public MauiSearchBar() : this(RectangleF.Empty)
		{
		}

		public MauiSearchBar(NSCoder coder) : base(coder)
		{
		}

		public MauiSearchBar(CGRect frame) : base(frame)
		{
		}

		protected MauiSearchBar(NSObjectFlag t) : base(t)
		{
		}

		protected internal MauiSearchBar(NativeHandle handle) : base(handle)
		{
		}

		// Native Changed doesn't fire when the Text Property is set in code
		// We use this event as a way to fire changes whenever the Text changes
		// via code or user interaction.
		[UnconditionalSuppressMessage("Memory", "MEM0001", Justification = "Proven safe in test: MemoryTests.HandlerDoesNotLeak")]
		public event EventHandler<UISearchBarTextChangedEventArgs>? TextSetOrChanged;

		public override string? Text
		{
			get => base.Text;
			set
			{
				var old = base.Text;

				base.Text = value;

				if (old != value)
				{
					TextSetOrChanged?.Invoke(this, new UISearchBarTextChangedEventArgs(value ?? String.Empty));
				}
			}
		}

		[UnconditionalSuppressMessage("Memory", "MEM0001", Justification = "Proven safe in test: MemoryTests.HandlerDoesNotLeak")]
		internal event EventHandler? OnMovedToWindow;
		[UnconditionalSuppressMessage("Memory", "MEM0001", Justification = "Proven safe in test: MemoryTests.HandlerDoesNotLeak")]
		internal event EventHandler? EditingChanged;
		[UnconditionalSuppressMessage("Memory", "MEM0001", Justification = "Proven safe in test: MemoryTests.HandlerDoesNotLeak")]
		internal event EventHandler? SelectionChanged;

		public override void WillMoveToWindow(UIWindow? window)
		{
			var editor = this.GetSearchTextField();

			base.WillMoveToWindow(window);

			if (editor != null)
			{
				editor.EditingChanged -= OnEditingChanged;

				if (window != null)
				{
					editor.EditingChanged += OnEditingChanged;

					// Observe selection changes using KVO
					editor.AddObserver(this, new NSString("selectedTextRange"), NSKeyValueObservingOptions.New, IntPtr.Zero);
				}
			}

			if (window != null)
				OnMovedToWindow?.Invoke(this, EventArgs.Empty);
		}

		[UnconditionalSuppressMessage("Memory", "MEM0003", Justification = "Proven safe in test: MemoryTests.HandlerDoesNotLeak")]
		void OnEditingChanged(object? sender, EventArgs e)
		{
			EditingChanged?.Invoke(this, EventArgs.Empty);
		}

		public override void ObserveValue(NSString? keyPath, NSObject? ofObject, NSDictionary? change, IntPtr context)
		{
			if (keyPath?.ToString() == "selectedTextRange")
			{
				SelectionChanged?.Invoke(this, EventArgs.Empty);
			}
			else if (keyPath is not null)
			{
				// Call base only when keyPath is not null (base signature requires non-null)
				base.ObserveValue(keyPath!, ofObject!, change!, context);
			}
		}

		[UnconditionalSuppressMessage("Memory", "MEM0002", Justification = IUIViewLifeCycleEvents.UnconditionalSuppressMessage)]
		EventHandler? _movedToWindow;
		event EventHandler IUIViewLifeCycleEvents.MovedToWindow
		{
			add => _movedToWindow += value;
			remove => _movedToWindow -= value;
		}

		public override void MovedToWindow()
		{
			base.MovedToWindow();
			_movedToWindow?.Invoke(this, EventArgs.Empty);
		}
	}
}