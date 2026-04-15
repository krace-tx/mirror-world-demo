using System;

namespace MirrorWorldDemo.Script.Core;

[Flags]
public enum CollisionLayerEnum : uint
{
	None     = (uint) 0,
	World    = (uint) 1 << 0,
	Player   = (uint) 1 << 1,
	Enemy    = (uint) 1 << 2, 
	Object   = (uint) 1 << 3,
	Trigger  = (uint) 1 << 4, 
	Wall     = (uint) 1 << 5  
}
