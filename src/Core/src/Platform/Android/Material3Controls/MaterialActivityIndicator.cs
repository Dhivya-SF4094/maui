using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Google.Android.Material.ProgressIndicator;
namespace Microsoft.Maui.Platform;

internal class MaterialActivityIndicator : CircularProgressIndicator
{
	public MaterialActivityIndicator(Context context) : base(context)
	{
	}
	protected MaterialActivityIndicator(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
	{
	}

	public MaterialActivityIndicator(Context context, IAttributeSet? attrs) : base(MauiMaterialContextThemeWrapper.Create(context), attrs)
	{
	}

	public MaterialActivityIndicator(Context context, IAttributeSet? attrs, int defStyleAttr) : base(MauiMaterialContextThemeWrapper.Create(context), attrs, defStyleAttr)
	{
	}
}
