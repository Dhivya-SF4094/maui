#nullable disable
using System;
using Microsoft.UI.Xaml.Automation;
namespace Microsoft.Maui.Handlers
{
	public partial class StepperHandler : ViewHandler<IStepper, MauiStepper>
	{
		protected override MauiStepper CreatePlatformView() => new MauiStepper();

		protected override void ConnectHandler(MauiStepper platformView)
		{
			platformView.ValueChanged += OnValueChanged;
			var automationId = VirtualView.AutomationId;
			if (!string.IsNullOrEmpty(automationId))
			{
				//platformView.UpdateAutomationId();
			//	AutomationProperties.SetAutomationId(platformView._plus, $"{automationId}_Plus");
				//AutomationProperties.SetAutomationId(platformView._minus, $"{automationId}_Minus");

				//AutomationProperties.SetName(platformView._plus, $"{automationId}_Plus");
				//AutomationProperties.SetName(platformView._minus, $"{automationId}_Minus");
			}

			base.ConnectHandler(platformView);
			//if (!string.IsNullOrEmpty(automationId))
			//{
			//	//platformView.UpdateAutomationId();
			//	AutomationProperties.SetAutomationId(platformView._plus, $"{automationId}_Plus");
			//	AutomationProperties.SetAutomationId(platformView._minus, $"{automationId}_Minus");

			//}
		}

		protected override void DisconnectHandler(MauiStepper platformView)
		{
			platformView.ValueChanged -= OnValueChanged;

			base.DisconnectHandler(platformView);
		}

		public static void MapMinimum(IStepperHandler handler, IStepper stepper)
		{
			handler.PlatformView?.UpdateMinimum(stepper);
		}

		public static void MapMaximum(IStepperHandler handler, IStepper stepper)
		{
			handler.PlatformView?.UpdateMaximum(stepper);
		}

		public static void MapIncrement(IStepperHandler handler, IStepper stepper)
		{
			handler.PlatformView?.UpdateIncrement(stepper);
		}

		public static void MapValue(IStepperHandler handler, IStepper stepper)
		{
			handler.PlatformView?.UpdateValue(stepper);
		}

		// This is a Windows-specific mapping
		public static void MapBackground(IStepperHandler handler, IStepper view)
		{
			handler.PlatformView?.UpdateBackground(view);
		}

		void OnValueChanged(object sender, EventArgs e)
		{
			if (VirtualView == null || PlatformView == null)
				return;

			VirtualView.Value = PlatformView.Value;
		}
	}
}