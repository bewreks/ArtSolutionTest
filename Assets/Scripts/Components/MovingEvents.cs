using Leopotam.EcsLite;

namespace Components
{
	public struct StartMovingEvent
	{
		public EcsPackedEntity MovedEntity;
	}
	
	public struct EndMovingEvent
	{
		public EcsPackedEntity MovedEntity;
	}
}