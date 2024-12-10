using System;

namespace Microsoft.Maui.Controls.Handlers
{
	public partial class BoxViewHandler : ShapeViewHandler
	{
		public static new IPropertyMapper<BoxView, IShapeViewHandler> Mapper = new PropertyMapper<BoxView, IShapeViewHandler>(ShapeViewHandler.Mapper)
		{
#if ANDROID
			[nameof(BoxView.Color)] = MapColor,		
#endif
		};

		public BoxViewHandler() : base(Mapper)
		{
		}

		public BoxViewHandler(IPropertyMapper mapper) : base(mapper ?? Mapper)
		{
		}
	}
}