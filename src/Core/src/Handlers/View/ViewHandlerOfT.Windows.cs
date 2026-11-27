#nullable enable
using System;
using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Microsoft.Maui.Handlers
{
	public abstract partial class ViewHandler<TVirtualView, TPlatformView> : IPlatformViewHandler
	{
		public override void PlatformArrange(Rect rect) =>
		 this.PlatformArrangeHandler(rect);

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint) =>
		 this.GetDesiredSizeFromHandler(widthConstraint, heightConstraint);

		protected override void SetupContainer()
		{
			if (PlatformView is null || ContainerView is not null)
			{
				return;
			}

			var parent = PlatformView.Parent;
			var contentHost = GetDirectContentHost(PlatformView, parent);
			// WinUI requires the element to leave its content slot before it can move into the wrapper.
			if (contentHost is not null)
			{
				contentHost.Content = null;
			}

#pragma warning disable RS0030 // Do not use banned APIs; Panel.Children is banned for performance reasons. MauiPanel might not be used everywhere though.
			var oldParentChildren = parent is MauiPanel mauiPanel
			 ? mauiPanel.CachedChildren
			 : (parent as Panel)?.Children;
#pragma warning restore RS0030 // Do not use banned APIs

			var oldIndex = oldParentChildren?.IndexOf(PlatformView);

			if (oldIndex is int oldIdx && oldIdx >= 0)
			{
				oldParentChildren?.RemoveAt(oldIdx);
			}

			ContainerView ??= new WrapperView();
			((WrapperView)ContainerView).Child = PlatformView;
			MoveTransformation(PlatformView, ContainerView);

			if (contentHost is not null)
			{
				contentHost.Content = ContainerView;
			}
			else if (oldIndex is int idx && idx >= 0)
			{
				oldParentChildren?.Insert(idx, ContainerView);
			}
			else
			{
				oldParentChildren?.Add(ContainerView);
			}
		}

		protected override void RemoveContainer()
		{
			if (PlatformView is null || ContainerView is null ||
				(PlatformView.Parent != ContainerView &&
					(ContainerView is not WrapperView currentWrapper || !currentWrapper.CachedChildren.Contains(PlatformView))))
			{
				CleanupContainerView(ContainerView);
				ContainerView = null;
				return;
			}

			var parent = ContainerView.Parent;
			var contentHost = GetDirectContentHost(ContainerView, parent);
			if (contentHost is not null)
			{
				contentHost.Content = null;
			}

#pragma warning disable RS0030 // Do not use banned APIs; Panel.Children is banned for performance reasons. MauiPanel might not be used everywhere though.
			var oldParentChildren = parent is MauiPanel mauiPanel
			 ? mauiPanel.CachedChildren
			 : (parent as Panel)?.Children;
#pragma warning restore RS0030 // Do not use banned APIs

			var oldIndex = oldParentChildren?.IndexOf(ContainerView);
			if (oldIndex is int oldIdx && oldIdx >= 0)
			{
				oldParentChildren?.RemoveAt(oldIdx);
			}

			MoveTransformation(ContainerView, PlatformView);
			CleanupContainerView(ContainerView);
			ContainerView = null;

			if (contentHost is not null)
			{
				contentHost.Content = PlatformView;
			}
			else if (oldIndex is int idx && idx >= 0)
			{
				oldParentChildren?.Insert(idx, PlatformView);
			}
			else
			{
				oldParentChildren?.Add(PlatformView);
			}

			static void CleanupContainerView(FrameworkElement? containerView)
			{
				if (containerView is WrapperView wrapperView)
				{
					wrapperView.Child = null;
					wrapperView.Dispose();
				}
			}
		}

		static ContentControl? GetDirectContentHost(FrameworkElement child, DependencyObject? parent) =>
		 parent is ContentControl host && ReferenceEquals(host.Content, child) ? host : null;

		static void MoveTransformation(FrameworkElement source, FrameworkElement destination)
		{
			// Transform mappers target the outermost view. Move their local values without copying style defaults.
			var transform = source.ReadLocalValue(UIElement.RenderTransformProperty) as Transform;
			var projection = source.ReadLocalValue(UIElement.ProjectionProperty) as Projection;
			if (transform is null && projection is null)
			{
				return;
			}

			var origin = source.RenderTransformOrigin;
			source.ClearValue(UIElement.RenderTransformProperty);
			source.ClearValue(UIElement.ProjectionProperty);
			source.ClearValue(UIElement.RenderTransformOriginProperty);

			destination.RenderTransformOrigin = origin;
			destination.RenderTransform = transform;
			destination.Projection = projection;
		}
	}
}