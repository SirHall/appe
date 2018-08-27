
using Excessives;

//{TODO} Remove this!

[System.Obsolete]
public partial class AffectorFactory : FactorE<AffectorBase>
{
	partial void RegisterToFactory();

	partial void RegisterToFactory()
	{
		AddItem(typeof(Affector_Coriolis));
		AddItem(typeof(Affector_Gravity));
		AddItem(typeof(Affector_SpinTwist));
		AddItem(typeof(Affector_WindDrag));
		AddItem(typeof(Affector_WindDragCubed));
	}
}